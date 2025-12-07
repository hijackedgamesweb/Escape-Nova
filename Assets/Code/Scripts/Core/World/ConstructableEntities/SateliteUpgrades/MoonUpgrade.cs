using System;
using Code.Scripts.Core.Systems.Skills;
using Code.Scripts.Core.Managers;
using UnityEngine;
using Code.Scripts.Patterns.ServiceLocator;

namespace Code.Scripts.Core.World.ConstructableEntities.SateliteUpgrades
{
    [Serializable]
    public class MoonUpgrade : Upgrade
    {
        [Header("Skill Point Settings")]
        [SerializeField] private int skillPointsPerCycle = 1; // Puntos por ciclo
        [SerializeField] private int cyclesRequired = 50; // Cada cuántos ciclos
        [SerializeField] private string upgradeId = "moon_upgrade"; // ID único para este upgrade

        private int cyclesCompleted = 0;
        private bool isActive = false;
        private SkillTreeManager skillTreeManager;

        public override void ApplyUpgrade(Planet planet)
        {
            if (planet == null) return;

            // Inicializar el sistema de seguimiento
            InitializeTrackingSystem();

            // Activar el upgrade
            ActivateUpgrade();

            // Notificación
            NotificationManager.Instance?.CreateNotification(
                $"¡Satélite Luna activado! Ganarás {skillPointsPerCycle} punto(s) de habilidad cada {cyclesRequired} ciclos solares.",
                NotificationType.Info);
        }

        private void InitializeTrackingSystem()
        {
            // Obtener el SkillTreeManager
            skillTreeManager = ServiceLocator.GetService<SkillTreeManager>();

            if (skillTreeManager == null)
            {
                Debug.LogWarning("MoonUpgrade: SkillTreeManager no encontrado. Intentando encontrar manualmente...");
                skillTreeManager = GameObject.FindObjectOfType<SkillTreeManager>();
            }

            // Cargar el progreso guardado si existe
            LoadProgress();

            //Debug.Log($"MoonUpgrade inicializado. Ciclos completados: {cyclesCompleted}");
        }

        private void ActivateUpgrade()
        {
            if (isActive) return;

            // Obtener GameTime
            var gameTime = ServiceLocator.GetService<Code.Scripts.Core.Managers.Interfaces.IGameTime>();

            if (gameTime != null)
            {
                // Suscribirse a eventos de ciclo completado
                gameTime.OnCycleCompleted += OnCycleCompleted;
                isActive = true;

                //Debug.Log($"MoonUpgrade activado. Escuchando ciclos. Ciclos necesarios: {cyclesRequired}");
            }
            else
            {
                Debug.LogError("MoonUpgrade: No se pudo obtener IGameTime");
            }
        }

        private void DeactivateUpgrade()
        {
            if (!isActive) return;

            // Obtener GameTime
            var gameTime = ServiceLocator.GetService<Code.Scripts.Core.Managers.Interfaces.IGameTime>();

            if (gameTime != null)
            {
                gameTime.OnCycleCompleted -= OnCycleCompleted;
                isActive = false;

                //Debug.Log("MoonUpgrade desactivado");
            }
        }

        private void OnCycleCompleted(int currentCycle)
        {
            if (!isActive || skillTreeManager == null) return;

            cyclesCompleted++;

            // Verificar si se ha alcanzado el número requerido de ciclos
            if (cyclesCompleted >= cyclesRequired)
            {
                AwardSkillPoints();
                cyclesCompleted = 0;

                // Guardar progreso
                SaveProgress();

            }

            // También podríamos mostrar un progreso cada ciertos ciclos
            if (cyclesCompleted % 10 == 0) // Cada 10 ciclos
            {
                int remainingCycles = cyclesRequired - cyclesCompleted;
                //Debug.Log($"MoonUpgrade: {cyclesCompleted}/{cyclesRequired} ciclos completados. {remainingCycles} ciclos restantes para el próximo punto.");
            }
        }

        private void AwardSkillPoints()
        {
            if (skillTreeManager == null)
            {
                Debug.LogWarning("MoonUpgrade: SkillTreeManager no disponible para otorgar puntos");
                return;
            }

            skillTreeManager.AddSkillPoints(skillPointsPerCycle);
            //Debug.Log($"MoonUpgrade: Otorgados {skillPointsPerCycle} punto(s) de habilidad");
        }

        private void SaveProgress()
        {
            try
            {
                // Usar PlayerPrefs para guardar el progreso (simple pero efectivo)
                // Para un sistema más robusto, usarías tu sistema de guardado existente
                PlayerPrefs.SetInt($"{upgradeId}_cyclesCompleted", cyclesCompleted);
                PlayerPrefs.SetInt($"{upgradeId}_isActive", isActive ? 1 : 0);
                PlayerPrefs.Save();

                //Debug.Log($"MoonUpgrade: Progreso guardado - Ciclos: {cyclesCompleted}, Activo: {isActive}");
            }
            catch (Exception e)
            {
                Debug.LogError($"MoonUpgrade: Error al guardar progreso: {e.Message}");
            }
        }

        private void LoadProgress()
        {
            try
            {
                if (PlayerPrefs.HasKey($"{upgradeId}_cyclesCompleted"))
                {
                    cyclesCompleted = PlayerPrefs.GetInt($"{upgradeId}_cyclesCompleted", 0);
                    isActive = PlayerPrefs.GetInt($"{upgradeId}_isActive", 0) == 1;

                    //Debug.Log($"MoonUpgrade: Progreso cargado - Ciclos: {cyclesCompleted}, Activo: {isActive}");
                }
                else
                {
                    //Debug.Log("MoonUpgrade: No se encontró progreso guardado. Iniciando desde 0.");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"MoonUpgrade: Error al cargar progreso: {e.Message}");
                cyclesCompleted = 0;
                isActive = false;
            }
        }

        // Método para reiniciar el progreso (útil para testing)
        [ContextMenu("Reset Progress")]
        public void ResetProgress()
        {
            cyclesCompleted = 0;
            PlayerPrefs.DeleteKey($"{upgradeId}_cyclesCompleted");
            PlayerPrefs.DeleteKey($"{upgradeId}_isActive");
            //Debug.Log("MoonUpgrade: Progreso reiniciado");
        }

        // Método para obtener información del estado actual
        public string GetStatusInfo()
        {
            if (!isActive) return "Satélite Luna: Inactivo";

            int remainingCycles = cyclesRequired - cyclesCompleted;
            return $"Satélite Luna: {cyclesCompleted}/{cyclesRequired} ciclos ({remainingCycles} restantes)";
        }

        // Propiedades públicas para acceder a la configuración
        public int CurrentCycles => cyclesCompleted;
        public int RequiredCycles => cyclesRequired;
        public int SkillPointsPerCycle => skillPointsPerCycle;
        public bool IsActive => isActive;

        // Método para cuando el satélite es destruido o removido
        public void RemoveUpgrade()
        {
            DeactivateUpgrade();
            ResetProgress();

            NotificationManager.Instance?.CreateNotification(
                "Satélite Luna desactivado. Ya no recibirás puntos de habilidad automáticos.",
                NotificationType.Warning);
        }
    }
}