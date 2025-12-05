using Code.Scripts.Core.Managers;
using Code.Scripts.Patterns.ServiceLocator;
using UnityEngine;

namespace Code.Scripts.Core.World
{
    public class PlanetLightController : MonoBehaviour
    {
        [SerializeField] private Transform sun;
        private Material mat;

        void Start()
        {
            if(sun == null)
                sun = ServiceLocator.GetService<SolarSystem>()?.GetSunTransform();
            mat = GetComponent<SpriteRenderer>().material;
        }

        void Update()
        {
            if (!sun) return;
            Vector3 worldLightPos = sun.position;
            mat.SetVector("_LightPos", worldLightPos);
        }
    
    }
}