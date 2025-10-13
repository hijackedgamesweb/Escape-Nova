using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
using UnityEngine.EventSystems;

public class AstrariumUI : MonoBehaviour
{

    //VARIABLES
    private Dictionary<int, string> entryTypes = new Dictionary<int, string>();
    
    private string filePath;
    
    [SerializeField] private GameObject entriesTab;
    [SerializeField] private GameObject entrieButtonPrefab;
    
    
    //METODOS
    private void Awake()
    {
        //Definicion del diccionario
        entryTypes[0] = "Planet";
        entryTypes[1] = "Specie";
        entryTypes[2] = "Resource";
        entryTypes[3] = "Satelite";
        entryTypes[4] = "Constellation";
        
        //Ubicacion del archvio que almacena el progreso del Astrario
        filePath = Application.dataPath + "/Scripts/Code/Systems/Astrarium/Astrarium.txt";
    }
    
    
    public void CategoryButtonPressed(int idx)
    {
        
        foreach(Transform child in entriesTab.transform)
        {
            Destroy(child.gameObject);
        }
        
        List<string> names = ReadAstrarium(entryTypes[idx]);
        
        foreach(string name in names)
        {
            Instantiate(entrieButtonPrefab, new Vector3(0, 0, 0), Quaternion.identity).transform.SetParent(entriesTab.transform);
        }
    }
    
    
    /* El metodo ReadAstrarium permite a la interfaz del Astrario recorrer todos los elementos del Astrario y buscar
     solamente aquellos que tienen la variable de "Encontrado" establecida a 1, lo que significa que el jugador lo 
     ha encontrado y, por ende, debe aparecer reflejado en el Astrario */
    public List<string> ReadAstrarium(string type)
    {
        List<string> names = new List<string>();
        
        using (var reader = new StreamReader(filePath))
        {
            string line;
            
            while ((line = reader.ReadLine()) != null)
            {
                string[] div = line.Split(','); //Dividmos la cadena en base a las comas
                
                if (div[0] == type && div[2] == "1") //Todos los elementos que tengan 1 en la ultima variable y con el nombre correct
                {
                    names.Add(div[0]);
                }
            }
        }
        return names;
    }
    
    
    /* El metodo UpdateAstrarium requiere de un nombre que emplea como codigo para encontrar un elemento en el Astrario
    Y actualizar su estado de "Encontrado". Este meotodo se debe llamar cuando el jugador ha descubierto un nuevo
    elemento que ahora debe aparecer en su Astrario. Es importante que se actualize el .txt para que quede guardado
    para cuando vuelva al juego en otra sesion */
    public void UpdateAstrarium(string codeName)
    {
        string[] arrLine = null;
        int idx = 0;
        
        using (var reader = new StreamReader(filePath))
        {
            string line;
            
            while ((line = reader.ReadLine()) != null)
            {
                if (line.Contains(codeName))
                {
                    string[] div = line.Split(','); //Dividmos la cadena en base a las comas
                    
                    arrLine = File.ReadAllLines(filePath);
                    arrLine[idx] = div[0] + ",1";
                }
                idx++;
            }
        }
        if(arrLine != null){ File.WriteAllLines(filePath, arrLine);}
    }
}