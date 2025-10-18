using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using static UnityEngine.GameObject;

public class PlacingLocation : MonoBehaviour
{
    //VARIABLES

    [SerializeField] private Button PlacingBtn;
    
    //METODOS
    
    private void Awake()
    {
        PlacingBtn.onClick.AddListener(Find("PlacingUI").GetComponent<PlacingUI>().PlacingLocationPressed);
    }
    
    public void PlacingLocationPressed()
    {
        gameObject.GetComponent<RawImage>().color = new Color(1, 0, 0, 1f);
        PlacingBtn.interactable = false;
    }
}