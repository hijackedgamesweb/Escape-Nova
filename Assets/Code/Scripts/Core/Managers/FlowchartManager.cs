using System;
using Code.Scripts.Core.Entity.Civilization;
using Code.Scripts.Core.Events;
using Code.Scripts.Core.SaveLoad.Interfaces;
using Code.Scripts.Core.Systems.Crafting;
using Code.Scripts.Patterns.ServiceLocator;
using Fungus;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Code.Scripts.Core.Managers
{
    public class FlowchartManager : MonoBehaviour, ISaveable
    {
        [SerializeField] private Flowchart _mippipFlowchart;
        [SerializeField] private Flowchart _akkiFlowchart;
        [SerializeField] private Flowchart _halxiFlowchart;
        [SerializeField] private Flowchart _skulgFlowchart;
        [SerializeField] private Flowchart _handoullFlowchart;
        
        CraftingSystem _craftingSystem;

        private void Start()
        {
            ServiceLocator.GetService<CivilizationManager>().OnNewCivilizationDiscovered += OnNewCivilizationDiscovered;
            _craftingSystem = ServiceLocator.GetService<CraftingSystem>();
            _craftingSystem.OnCraftingCompleted += UnlockTranslations;
            DiplomacyEvents.OnTradeProposed += OnTradeOffered;
        }

        private void OnDestroy()
        {
            ServiceLocator.GetService<CivilizationManager>().OnNewCivilizationDiscovered -= OnNewCivilizationDiscovered;
            _craftingSystem.OnCraftingCompleted -= UnlockTranslations;
            DiplomacyEvents.OnTradeProposed -= OnTradeOffered;
        }

        private void OnNewCivilizationDiscovered(Civilization obj, bool isNew)
        {
            if(!isNew) return;
            switch (obj.CivilizationData.Name)
            {
                case "Mippip":
                    _mippipFlowchart.SendFungusMessage("CivilizationDiscovered");
                    break;
                case "Akki":
                    _akkiFlowchart.SendFungusMessage("CivilizationDiscovered");
                    break;
                case "Halxi":
                    _halxiFlowchart.SendFungusMessage("CivilizationDiscovered");
                    break;
                case "Skulg":
                    _skulgFlowchart.SendFungusMessage("CivilizationDiscovered");
                    break;
                case "Handoull":
                    _handoullFlowchart.SendFungusMessage("CivilizationDiscovered");
                    break;
            }
        }

        public void OnTalkWithCivilization(Civilization civilization)
        {
            switch (civilization.CivilizationData.Name)
            {
                case "Mippip":
                    _mippipFlowchart.SetStringVariable("Mood", civilization.CivilizationState.GetMoodDescription());
                    _mippipFlowchart.SendFungusMessage("Talk");
                    break;
                case "Akki":
                    _akkiFlowchart.SetStringVariable("Mood", civilization.CivilizationState.GetMoodDescription());
                    _akkiFlowchart.SendFungusMessage("Talk");
                    break;
                case "Halxi":
                    _halxiFlowchart.SetStringVariable("Mood", civilization.CivilizationState.GetMoodDescription());
                    _halxiFlowchart.SendFungusMessage("Talk");
                    break;
                case "Skulg":
                    _skulgFlowchart.SetStringVariable("Mood", civilization.CivilizationState.GetMoodDescription());
                    _skulgFlowchart.SendFungusMessage("Talk");
                    break;
                case "Handoull":
                    _handoullFlowchart.SetStringVariable("Mood", civilization.CivilizationState.GetMoodDescription());
                    _handoullFlowchart.SendFungusMessage("Talk");
                    break;
            }
            
        }
        
        private void UnlockTranslations(string civilizationId)
        {
            switch (civilizationId)
            {
                case "receta_mippip_translator":
                    _mippipFlowchart.SetBooleanVariable("Translator", true);
                    break;
                case "receta_skulg_translator":
                    _skulgFlowchart.SetBooleanVariable("Translator", true);
                    break;
                case "receta_hanadoull_translator":
                    _handoullFlowchart.SetBooleanVariable("Translator", true);
                    break;
                case "receta_akki_translator":
                    _akkiFlowchart.SetBooleanVariable("Translator", true);
                    break;
                case "receta_halxi_translator":
                    _halxiFlowchart.SetBooleanVariable("Translator", true);
                    break;
            }
        }
        
        private void OnTradeOffered(Civilization civilization, bool response)
        {
            switch (civilization.CivilizationData.Name)
            {
                case "Mippip":
                    _mippipFlowchart.SetBooleanVariable("TradeAccepted", response);
                    _mippipFlowchart.SendFungusMessage("Trade");
                    break;
                case "Akki":
                    _akkiFlowchart.SetBooleanVariable("TradeAccepted", response);
                    _akkiFlowchart.SendFungusMessage("Trade");
                    break;
                case "Halxi":
                    _halxiFlowchart.SetBooleanVariable("TradeAccepted", response);
                    _halxiFlowchart.SendFungusMessage("Trade");
                    break;
                case "Skulg":
                    _skulgFlowchart.SetBooleanVariable("TradeAccepted", response);
                    _skulgFlowchart.SendFungusMessage("Trade");
                    break;
                case "Handoull":
                    _handoullFlowchart.SetBooleanVariable("TradeAccepted", response);
                    _handoullFlowchart.SendFungusMessage("Trade");
                    break;
            }
        }

        public string GetSaveId()
        {
            return "FlowchartManager";
        }

        public JToken CaptureState()
        {
            var state = new JObject()
            {
                ["MippipTranslator"] = _mippipFlowchart.GetBooleanVariable("Translator"),
                ["AkkiTranslator"] = _akkiFlowchart.GetBooleanVariable("Translator"),
                ["HalxiTranslator"] = _halxiFlowchart.GetBooleanVariable("Translator"),
                ["SkulgTranslator"] = _skulgFlowchart.GetBooleanVariable("Translator"),
                ["HandoullTranslator"] = _handoullFlowchart.GetBooleanVariable("Translator"),
            };
            return state;
        }

        public void RestoreState(JToken state)
        {
            JObject obj = state as JObject;
            if (obj != null)
            {
                _mippipFlowchart.SetBooleanVariable("Translator", (bool)obj["MippipTranslator"]);
                _akkiFlowchart.SetBooleanVariable("Translator", (bool)obj["AkkiTranslator"]);
                _halxiFlowchart.SetBooleanVariable("Translator", (bool)obj["HalxiTranslator"]);
                _skulgFlowchart.SetBooleanVariable("Translator", (bool)obj["SkulgTranslator"]);
                _handoullFlowchart.SetBooleanVariable("Translator", (bool)obj["HandoullTranslator"]);
            }
        }
    }
}