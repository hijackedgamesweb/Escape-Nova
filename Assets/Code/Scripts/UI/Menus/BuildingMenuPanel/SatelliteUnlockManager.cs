using System.Collections.Generic;
using Code.Scripts.Core.Events;
using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;
using Code.Scripts.Patterns.ServiceLocator;
using UnityEngine;

namespace Code.Scripts.UI.Menus.BuildingMenuPanel
{

    namespace Code.Scripts.Core.Systems.Satelites
    {
        public class SateliteUnlockManager : MonoBehaviour
        {
            private List<SateliteDataSO> _unlockedSatelites = new List<SateliteDataSO>();

            public event System.Action<SateliteDataSO> OnSateliteAddedToConstructionList;

            private void Awake()
            {
                ServiceLocator.RegisterService(this);
                ResearchEvents.OnNewSateliteResearched += HandleNewSateliteResearched;
            }

            private void OnDestroy()
            {
                ResearchEvents.OnNewSateliteResearched -= HandleNewSateliteResearched;
            }

            private void HandleNewSateliteResearched(SateliteDataSO satelite)
            {
                if (satelite == null || _unlockedSatelites.Contains(satelite))
                {
                    return;
                }

                _unlockedSatelites.Add(satelite);
            
                OnSateliteAddedToConstructionList?.Invoke(satelite);
            }

            public List<SateliteDataSO> GetUnlockedSatelites()
            {
                return _unlockedSatelites;
            }
        }
    }
}