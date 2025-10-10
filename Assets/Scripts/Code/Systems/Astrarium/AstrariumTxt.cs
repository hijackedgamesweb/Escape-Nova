using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class AstrariumTxt : MonoBehaviour
{
    //VARIABLES
    
    //variables para la creacion del txt
    private string filePath;
    private StringBuilder txt = new StringBuilder();
    
    
    //METODOS

    private void Awake()
    {
        filePath = Application.dataPath + "/Scripts/Code/Systems/Astrarium/Astrarium.txt";
    }


    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            UpdateAstrarium("Marte");
        }
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            ReadAstrarium();
        }
    }
    
    /* El metodo ReadAstrarium permite a la interfaz del Astrario recorrer todos los elementos del Astrario y buscar
     solamente aquellos que tienen la variable de "Encontrado" establecida a 1, lo que significa que el jugador lo 
     ha encontrado y, por ende, debe aparecer reflejado en el Astrario */
    public void ReadAstrarium()
    {
        using (var reader = new StreamReader(filePath))
        {
            string line;
            
            while ((line = reader.ReadLine()) != null)
            {
                string[] div = line.Split(','); //Dividmos la cadena en base a las comas
                
                if (div[div.Length - 1] == "1") //Todos los elementos que tengan 1 en la ultima variable
                {
                    Debug.Log("Nombre:" + div[1]);
                }
            }
        }
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

                    string newLine = "";
                    
                    for (int i = 0; i < div.Length - 1; i++) //Ignoramos el ultimo elemento porque es el booleano a actualizar
                    {
                        newLine += div[i] + ",";
                    }

                    newLine += "1"; //Actualizamos el ultimo valor del elemento a 1
                    
                    arrLine = File.ReadAllLines(filePath);
                    arrLine[idx] = newLine;
                }
                idx++;
            }
        }
        if(arrLine != null){ File.WriteAllLines(filePath, arrLine);}
    }
}