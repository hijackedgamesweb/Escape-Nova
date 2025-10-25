using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.Core.Systems.Astrarium
{
    public class AstrariumEntryButton : MonoBehaviour
    {
        //VARIABLES
        [SerializeField] private GameObject entryPrefab;
        [SerializeField] private Button btn;
        [SerializeField] private TMP_Text btnTxt;
    
        private GameObject AstrariumPanel;
    
        public string entryName;
    
    
        //METODOS
        private void Awake()
        {
            AstrariumPanel = GameObject.Find("AstrariumPanel");
        
            btn.onClick.AddListener(delegate{OpenEntry(entryName);});
        }

        private void Start()
        {
            btnTxt.text = entryName;
        }
    
    
        public void OpenEntry(string _name)
        {
            if(GameObject.Find("AstrariumEntry(Clone)") != null) { Destroy(GameObject.Find("AstrariumEntry(Clone)")); }
        
            GameObject go = Instantiate(entryPrefab, new Vector3(250, 0, 0), Quaternion.identity);
            go.transform.SetParent(AstrariumPanel.transform, false);
            go.GetComponent<AstrariumEntry>().entryName = _name;
        }
    }
}