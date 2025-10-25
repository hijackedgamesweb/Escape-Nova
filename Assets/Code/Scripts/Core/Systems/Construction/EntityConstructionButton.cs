using System;
using Code.Scripts.Core.Systems.Construction;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class EntityConstructionButton : MonoBehaviour
{
    //VARIABLES
    private EntityScriptableObjectScript scriptableObject;
    public string entityType;
    public string entityName;
    
    [SerializeField] private TMP_Text entityNameTxt;
    [SerializeField] private TMP_Text timeToBuildTxt;
    
    //METODOS

    private void Start()
    {
        //Encontrar la referencia al ScriptableObject que representa para tomar su informacion
        var scriptableObjectReference = new AssetReference("Assets/Level/ScriptableObjects/Entities/" + entityType + "/" + entityName + ".asset");
        var asyncOperationHandler = scriptableObjectReference.LoadAssetAsync<EntityScriptableObjectScript>();
        asyncOperationHandler.Completed += LoadInfo;
    }
    
    
    private void LoadInfo(AsyncOperationHandle<EntityScriptableObjectScript> obj)
    {
        //Almacenamos el resultado de la operacion asincrona en la variable de tipo EntityScriptableObjectScript
        scriptableObject = obj.Result;
        
        //Actualizamos la informacion de la entrada en base al ainformacion del scriptableobject
        entityNameTxt.text = scriptableObject.entityName;
        timeToBuildTxt.text = scriptableObject.timeToBuild.ToString();
    }
    
    
    public void EntityButtonPressed()
    {
        GameObject.Find("ConstructionUI").GetComponent<ConstructionUI>().EntityPressed();
    }
}