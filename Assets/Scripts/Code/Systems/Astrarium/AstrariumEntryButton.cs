using System;
using UnityEngine;
using UnityEngine.UI;

public class AstrariumEntryButton : MonoBehaviour
{
    //VARIABLES
    [SerializeField] private GameObject entryPrefab;
    [SerializeField] private Button btn;
    
    private GameObject AstrariumPanel;
    
    public string entryName;
    
    
    //METODOS
    private void Awake()
    {
        AstrariumPanel = GameObject.Find("AstrariumPanel");
        
        btn.onClick.AddListener(delegate{OpenEntry(entryName);});
    }
    
    
    public void OpenEntry(string _name)
    {
        GameObject go = Instantiate(entryPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        go.transform.SetParent(AstrariumPanel.transform);
        go.GetComponent<AstrariumEntry>().entryName = _name;
    }
}