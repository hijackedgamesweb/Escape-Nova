using Code.Scripts.Core.Managers;
using Code.Scripts.Patterns.ServiceLocator;
using UnityEngine;

namespace Code.Scripts.Core.World
{
    public class PlanetLightController : MonoBehaviour
    {
        private Transform sun;
        private Material mat;

        void Start()
        {
            sun = ServiceLocator.GetService<SolarSystem>().GetSunTransform();
            mat = GetComponent<SpriteRenderer>().material;
        }

        void Update()
        {
            if (!sun) return;

            // 1. POSICIÓN DEL SOL EN WORLD SPACE
            Vector3 worldLightPos = sun.position;
            mat.SetVector("_LightPos", worldLightPos);

            // 2. POSICIÓN REAL DEL PLANETA EN WORLD SPACE
            Vector3 planetPos = transform.position;

            // 3. DIBUJAR LA LÍNEA DE LUZ
            Debug.DrawLine(planetPos, worldLightPos, Color.yellow);

            // 4. OPCIONAL: mostrar en consola para confirmar valores
            Debug.Log($"{name}: Planet={planetPos} | Sun={worldLightPos}");
        }
    
    }
}