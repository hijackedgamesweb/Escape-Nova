using Code.Scripts.Core.Managers.Interfaces;
using Code.Scripts.Patterns.ServiceLocator;
using UnityEngine;

namespace Code.Scripts.UI.Menus.MainMenu
{
    public class OrbitControllerMenu : MonoBehaviour
    {
        [SerializeField] private Transform orbitingObject;
        [SerializeField] private Transform[] orbitingPlanets;
        [SerializeField] private float[] orbitsSpeed;
        [SerializeField] private float[] orbitsRadius;
        private IGameTime _gameTime;
        void Start()
        {
            _gameTime = ServiceLocator.GetService<IGameTime>();
        }

        // Update is called once per frame
        void Update()
        {
            for (int i = 0; i < orbitingPlanets.Length; i++)
            {
                if (orbitingPlanets[i] == null) continue;
                float angle = _gameTime.GameTime * orbitsSpeed[i];
                float x = orbitingObject.position.x + Mathf.Cos(angle) * orbitsRadius[i];
                float y = orbitingObject.position.y + Mathf.Sin(angle) * orbitsRadius[i];
                orbitingPlanets[i].position = new Vector3(x, y, orbitingPlanets[i].position.z);
            }
        }
    }
}
