using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using static UnityEngine.GameObject;

public class PlacingLocation : MonoBehaviour
{
    //VARIABLES

    [SerializeField] private Button PlacingBtn;
    [Header("Coordenadas de Ubicación")]
    public int orbitIndex;
    public int positionIndex;
    
    private PlacingUI _placingUI;
    
    //METODOS
    
    private void Awake()
    {
        _placingUI = Find("PlacingUI").GetComponent<PlacingUI>();
        
        PlacingBtn.onClick.AddListener(OnLocationPressed);
    }
    
    private void OnLocationPressed()
    {
        // Le decimos al PlacingUI DÓNDE hemos hecho clic
        if (_placingUI != null)
        {
            _placingUI.PlacingLocationPressed(orbitIndex, positionIndex);
        }
        
        // Lógica visual (esto se queda igual)
        gameObject.GetComponent<RawImage>().color = new Color(1, 0, 0, 1f);
        PlacingBtn.interactable = false;
    }
    
    public void PlacingLocationPressed()
    {
        gameObject.GetComponent<RawImage>().color = new Color(1, 0, 0, 1f);
        PlacingBtn.interactable = false;
    }
}