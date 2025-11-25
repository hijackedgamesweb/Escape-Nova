using UnityEngine;
using UnityEngine.InputSystem; // Necesario
using Code.Scripts.Core.Managers;
using Code.Scripts.UI.Windows;
using Code.Scripts.Core.Events;

namespace Code.Scripts.Core.Systems.UI
{
    public class MenuShortcuts : MonoBehaviour
    {
        [Header("Input Configuration")]
        [SerializeField] private InputActionAsset inputActionAsset;
        [SerializeField] private string actionMapName = "Shortcuts";

        private InputAction _diplomacyAction;
        private InputAction _missionsAction;
        private InputAction _researchAction;
        private InputAction _storageAction;
        private InputAction _astrariumAction;
        private InputAction _constellationsAction;
        private InputAction _constructionAction;

        private void Awake()
        {
            var map = inputActionAsset.FindActionMap(actionMapName);

            if (map == null)
            {
                Debug.LogError($"No se encontró el Action Map: {actionMapName}");
                return;
            }

            _diplomacyAction = map.FindAction("OpenDiplomacy");
            _missionsAction = map.FindAction("OpenMissions");
            _researchAction = map.FindAction("OpenResearch");
            _storageAction = map.FindAction("OpenStorage");
            _astrariumAction = map.FindAction("OpenAstrarium");
            _constellationsAction = map.FindAction("OpenConstellations");
            _constructionAction = map.FindAction("OpenConstruction");
        }

        private void OnEnable()
        {
            _diplomacyAction?.Enable();
            _missionsAction?.Enable();
            _researchAction?.Enable();
            _storageAction?.Enable();
            _astrariumAction?.Enable();
            _constellationsAction?.Enable();
            _constructionAction?.Enable();
        }

        private void OnDisable()
        {
            _diplomacyAction?.Disable();
            _missionsAction?.Disable();
            _researchAction?.Disable();
            _storageAction?.Disable();
            _astrariumAction?.Disable();
            _constellationsAction?.Disable();
            _constructionAction?.Disable();
        }

        private void Update()
        {
            // R -> Storage (Action: OpenStorage)
            if (_storageAction != null && _storageAction.WasPerformedThisFrame())
            {
                if (SystemEvents.IsInventoryUnlocked)
                {
                    OpenActionPanel("Storage");
                }
            }

            // E -> Research (Action: OpenResearch)
            if (_researchAction != null && _researchAction.WasPerformedThisFrame())
            {
                if (SystemEvents.IsResearchUnlocked)
                {
                    OpenActionPanel("Research");
                }
            }

            // W -> Missions (Action: OpenMissions)
            if (_missionsAction != null && _missionsAction.WasPerformedThisFrame())
            {
                OpenActionPanel("Objectives");
            }

            // Y -> Constelaciones (Action: OpenConstellations)
            if (_constellationsAction != null && _constellationsAction.WasPerformedThisFrame())
            {
                if (SystemEvents.IsConstellationsUnlocked)
                {
                    OpenActionPanel("Constellations");
                }
            }

            // T -> Astrario (Action: OpenAstrarium)
            if (_astrariumAction != null && _astrariumAction.WasPerformedThisFrame())
            {
                OpenActionPanel("Astrarium");
            }

            if (_diplomacyAction != null && _diplomacyAction.WasPerformedThisFrame())
            {
                if (SystemEvents.IsDiplomacyUnlocked)
                {
                    OpenActionPanel("Diplomacy");
                }
            }
            
            // U -> Construcción (Action: OpenConstruction)
            if (_constructionAction != null && _constructionAction.WasPerformedThisFrame())
            {
                UIManager.Instance.ShowScreen<PerfectViewScreen>();
            }
        }
        private void OpenActionPanel(string panelId)
        {
            UIManager.Instance.ShowScreen<ActionPanelScreen>(panelId);
        }
    }
}