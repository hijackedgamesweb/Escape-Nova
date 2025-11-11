using System;
using Code.Scripts.Core.Systems.Construction;
using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class EntityConstructionButton : MonoBehaviour
{
    //VARIABLES
    public ConstructibleDataSO entityData;
    
    [SerializeField] private TMP_Text entityNameTxt;
    [SerializeField] private TMP_Text timeToBuildTxt;
    
    //METODOS

    private void Start()
    {
    }
    
    
    public void Initialize(ConstructibleDataSO data)
    {
        entityData = data;
        
        //Actualizamos la informacion de la entrada en base a la informacion del scriptableobject
        entityNameTxt.text = entityData.constructibleName;
        timeToBuildTxt.text = entityData.timeToBuild.ToString();
    }
    
    
    public void EntityButtonPressed()
    {
        GameObject.Find("ConstructionUI").GetComponent<ConstructionUI>().EntityPressed(this);
    }
}