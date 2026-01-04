using UnityEngine;
using Code.Scripts.Core.Managers;
using Code.Scripts.Core.Systems.Diplomacy.AI.Behaviour.USBehaviour;

public class WarTester : MonoBehaviour
{
    public CivilizationManager civManager; 

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (civManager == null) civManager = FindFirstObjectByType<CivilizationManager>();

            var civs = civManager.GetCivilizations;
            if (civs.Count > 0)
            {
                var alien = civs[0];
                var ai = alien.AIController as BaseBehaviour;
                
                if (ai != null)
                {
                    ai.DEBUG_ForceWarSituation();
                }
            }
        }
    }
}