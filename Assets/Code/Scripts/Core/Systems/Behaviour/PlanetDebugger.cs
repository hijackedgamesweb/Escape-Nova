using UnityEngine;
using Code.Scripts.Core.World.ConstructableEntities;
using Code.Scripts.Core.Entity.Civilization;
using Code.Scripts.Core.Systems.Diplomacy.AI.Behaviour.USBehaviour;
using System.Reflection;
using System.Runtime.Serialization;

public class PlanetDebugger : MonoBehaviour
{
    private Planet _planet;
    private Civilization _dummyOwner;
    private Civilization _dummyEnemy;

    void Start()
    {
        _planet = GetComponent<Planet>();

        // 1. Crear Dueño Falso (Humanos)
        _dummyOwner = new Civilization();
        var dummyData = new CivilizationData();
        SetPrivateProperty(dummyData, "Name", "Humanos (Jugador)");
        SetPrivateProperty(_dummyOwner, "CivilizationData", dummyData);
        
        var aiPlayer = (BaseBehaviour)FormatterServices.GetUninitializedObject(typeof(BaseBehaviour));
        SetPrivateProperty(aiPlayer, "CivilizationOwner", _dummyOwner);
        SetPrivateProperty(_dummyOwner, "AIController", aiPlayer);
        
        _dummyEnemy = new Civilization();
        var enemyData = new CivilizationData();
        SetPrivateProperty(enemyData, "Name", "Alienígenas (Enemigo)");
        SetPrivateProperty(_dummyEnemy, "CivilizationData", enemyData);

        var aiEnemy = (BaseBehaviour)FormatterServices.GetUninitializedObject(typeof(BaseBehaviour));
        SetPrivateProperty(aiEnemy, "CivilizationOwner", _dummyEnemy);
        SetPrivateProperty(_dummyEnemy, "AIController", aiEnemy);
    }

    private void SetPrivateProperty(object target, string propertyName, object value)
    {
        var type = target.GetType();
        var prop = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (prop != null && prop.CanWrite)
        {
            prop.SetValue(target, value, null);
            return;
        }

        var field = type.GetField($"<{propertyName}>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
        if (field != null)
        {
            field.SetValue(target, value);
            return;
        }
        
        var normalField = type.GetField(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        if (normalField != null)
        {
            normalField.SetValue(target, value);
        }
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("<color=cyan>[DEBUG] 1. Colonizando Planeta...</color>");
            _planet.EstablishContact(_dummyOwner);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("<color=orange>[DEBUG] 2. Declarando Guerra...</color>");
            
            if (_planet.AssociatedAI != null)
            {
                var ai = _planet.AssociatedAI;
            }
            
            _planet.DeclareWar(_dummyEnemy);
            if(_planet.AssociatedAI != null)
            {
                SetPrivateProperty(_planet.AssociatedAI, "_isAtWarWithPlayer", true);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("<color=green>[DEBUG] 3. Ganamos la guerra (Paz)...</color>");
            if (_planet.AssociatedAI != null) SetPrivateProperty(_planet.AssociatedAI, "_isAtWarWithPlayer", false);
            _planet.WinWar(); 
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Debug.Log("<color=red>[DEBUG] 4. Perdimos... Planeta Conquistado</color>");
             _planet.LoseWar(_dummyEnemy);
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Debug.Log("<color=red>[DEBUG] 5. KA-BOOM! Planeta Destruido</color>");
            _planet.DestroyPlanet();
        }
    }
}