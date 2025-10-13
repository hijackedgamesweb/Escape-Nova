using UnityEngine;

namespace Code.Scripts.Config
{
    [CreateAssetMenu(fileName = "TimeConfig", menuName = "Configs/Time Configuration", order = 0)]
    public class TimeConfig : ScriptableObject
    {
        [Header("Duración base")]
        [Tooltip("Duración en segundos de un ciclo")]
        public float secondsPerCycle = 60f;
        
        [Tooltip("Velocidad base del juego (1 = tiempo real)")]
        public float baseGameSpeed = 1f;
        
        [Header("Escalas de velocidad disponibles")]
        public float[] timeSpeeds = {0f, 1f, 2f, 4f};
        
        [Header("Formato visual")]
        [Tooltip("Nombre de la unidad de tiempo (Día, Ciclo, Turno...)")]
        public string timeUnitName = "Ciclo Solar";
    }
}