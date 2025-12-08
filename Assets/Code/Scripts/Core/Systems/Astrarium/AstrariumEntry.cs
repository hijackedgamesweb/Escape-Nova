using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Code.Scripts.Core.Systems.Astrarium
{
    public class AstrariumEntry : MonoBehaviour
    {
        //VARIABLES
        private AstrariumEntryScriptableObjectScript scriptableObject;
    
        public string entryName;

        [SerializeField] private TMP_Text nameTxt;
        [SerializeField] private TMP_Text typeTxt;
        [SerializeField] private TMP_Text descriptionTxt;
        [SerializeField] private Image image;
    
    
        //METODOS
        private void Start()
        {
            var scriptableObjectReference = new AssetReference("Assets/Level/ScriptableObjects/Astrarium/" + entryName + ".asset");
            var asyncOperationHandler = scriptableObjectReference.LoadAssetAsync<AstrariumEntryScriptableObjectScript>();
            asyncOperationHandler.Completed += LoadInfo;
        }
    
    
        private void LoadInfo(AsyncOperationHandle<AstrariumEntryScriptableObjectScript> obj)
        {
            //Almacenamos el resultado de la operacion asincrona en la variable de tipo AstrariumEntryScriptableObjectScript
            scriptableObject = obj.Result;
        
            //Actualizamos la informacion de la entrada en base a la informacion almacenada en el ScriptableObject especifico
            nameTxt.text = scriptableObject.name;
            typeTxt.text = scriptableObject.entryType.ToString();
            descriptionTxt.text = scriptableObject.description;
            image.sprite = scriptableObject.image;
        }
    }
}