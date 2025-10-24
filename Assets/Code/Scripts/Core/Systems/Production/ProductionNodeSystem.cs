using System;
using System.Collections;
using System.Collections.Generic;
using Code.Scripts.Core.Systems.Resources;
using Code.Scripts.Core.Systems.Storage;
using UnityEngine;
using Code.Scripts.Patterns.ServiceLocator;

namespace Code.Scripts.Core.Systems.Production
{
    public enum NodeCategory
    {
        Construcción,
        Sociales,
        Bélicas
    }

    [Serializable]
    public class ProductionNode
    {
        public string NodeId;
        public NodeCategory Category;
        public Vector2Int Position;

        [Header("Producción Base")]
        public Dictionary<ResourceType, float> BaseProduction = new Dictionary<ResourceType, float>();

        [Header("Estado")] public bool IsActive = true;
        public float Efficiency = 1.0f; // 1.0 = 100% eficiencia

        // calculamos produccion actual considerando todos los modificadores
        public Dictionary<ResourceType, int> CalculateCurrentProduction(float globalMultiplier)
        {
            var currentProduction = new Dictionary<ResourceType, int>();

            foreach (var resource in BaseProduction)
            {
                // FÓRMULA DE PRODUCCIÓN ACTUAL:
                // Base x Eficiencia x MultiplicadorGlobal

                // Aquí se añadirá x MultiplicadorPoblación cuando exista PopulationSystem
                // float populationMultiplier = _population.GetProductionMultiplier();
                // float calculated = baseAmount * Efficiency * populationMultiplier * globalMultiplier;

                float baseAmount = resource.Value;
                float calculated = baseAmount * Efficiency * globalMultiplier;

                currentProduction[resource.Key] = Mathf.RoundToInt(calculated);
            }

            return currentProduction;
        }
    }

    public class ProductionNodeSystem : MonoBehaviour
    {
        [Header("Configuración Principal")] [SerializeField]
        private float _productionInterval = 5f; // Cada 5 segundos en tiempo real

        [SerializeField] private float _baseProductionMultiplier = 1.0f;

        private Dictionary<string, ProductionNode> _activeNodes = new Dictionary<string, ProductionNode>();
        private StorageSystem _storage;

        // private PopulationSystem _population;

        // eventos para que otros sistemas sepan lo que esta pasando
        public event Action<string, Dictionary<ResourceType, int>> OnNodeProduction;
        public event Action<float> OnProductionCycleCompleted;

        private void Awake()
        {
            ServiceLocator.RegisterService<ProductionNodeSystem>(this);
        }

        private void Start()
        {
            // obtenemos dependencias (en base a nuestro diagrama de sistemas)
            _storage = ServiceLocator.GetService<StorageSystem>();

            // obtenemos sistema de población cuando exista
            // _population = ServiceLocator.GetService<PopulationSystem>();

            // iniciamos ciclo de producción
            StartCoroutine(ProductionCycle());

            Debug.Log("Sistema de Nodos de Producción iniciado");
        }

        // Corrutina principal: El "latido" de nuestra economía
        private IEnumerator ProductionCycle()
        {
            while (true)
            {
                yield return new WaitForSeconds(_productionInterval);
                GenerateResourcesFromAllNodes();
            }
        }

        // Generamos recursos de todos los nodos activos
        private void GenerateResourcesFromAllNodes()
        {
            if (_activeNodes.Count == 0) return;

            // obtenemos multiplicador de población cuando exista PopulationSystem
            // float populationMultiplier = GetPopulationMultiplier();
            float populationMultiplier = 1.0f; // Valor temporal hasta que exista PopulationSystem

            float totalProduced = 0f;

            foreach (var nodeEntry in _activeNodes)
            {
                var node = nodeEntry.Value;
                if (!node.IsActive) continue;

                // calculamos la produccion de este nodo
                var production = node.CalculateCurrentProduction(_baseProductionMultiplier);

                // enviamos recursos al almacenamiento
                foreach (var resource in production)
                {
                    _storage.AddResource(resource.Key, resource.Value);
                    totalProduced += resource.Value;
                }

                // notificamos que este nodo se ha producido
                OnNodeProduction?.Invoke(node.NodeId, production);
            }

            Debug.Log($"Ciclo de producción: {totalProduced} recursos generados de {_activeNodes.Count} nodos");
            OnProductionCycleCompleted?.Invoke(totalProduced);
        }

        // Este método con PopulationSystem seguro que tiene sentido
        /*
        private float GetPopulationMultiplier()
        {
            if (_population == null) return 1.0f;

            // Ejemplo de cómo podría funcionar:
            // - Poca población: 0.5x (50% de eficiencia)
            // - Población óptima: 1.5x (150% de eficiencia)
            // - Sobrepoblación: 1.0x (100% de eficiencia)
            return _population.GetProductionEfficiency();
        }
        */

        // agregamos nodo: Cuando el sistema de construcción termina un edificio
        public void AddNode(ProductionNode node)
        {
            if (_activeNodes.ContainsKey(node.NodeId))
            {
                Debug.LogWarning($"Nodo {node.NodeId} ya existe. Actualizando.");
            }

            _activeNodes[node.NodeId] = node;
            Debug.Log($"Nodo agregado: {node.NodeId} (Categoría: {node.Category})");
        }

        // borrar ndoo: Cuando se destruye un edificio
        public void RemoveNode(string nodeId)
        {
            if (_activeNodes.Remove(nodeId))
            {
                Debug.Log($"¡Nodo borrado: {nodeId}");
            }
        }

        // configuramos la produccion base según categoría
        public void ConfigureNodeProduction(ProductionNode node)
        {
            node.BaseProduction.Clear();

            switch (node.Category)
            {
                case NodeCategory.Construcción:
                    node.BaseProduction[ResourceType.Metal] = 2f;
                    node.BaseProduction[ResourceType.Madera] = 3f;
                    node.BaseProduction[ResourceType.Piedra] = 1f;
                    break;

                case NodeCategory.Sociales:
                    node.BaseProduction[ResourceType.Comida] = 5f;
                    node.BaseProduction[ResourceType.Energia] = 2f;
                    break;

                case NodeCategory.Bélicas:
                    node.BaseProduction[ResourceType.Cristal] = 2f;
                    node.BaseProduction[ResourceType.Metal] = 1f;
                    break;
            }
        }

        // obtenemois estadisticas (para UI)
        public ProductionStats GetProductionStats()
        {
            var stats = new ProductionStats();
            stats.TotalActiveNodes = _activeNodes.Count;
            stats.ProductionInterval = _productionInterval;

            foreach (var node in _activeNodes.Values)
            {
                stats.TotalBaseProduction += node.BaseProduction.Count;
            }

            return stats;
        }

        // metodo para conectar con PopulationSystem cuando exista
        /*
        public void ConnectPopulationSystem(PopulationSystem populationSystem)
        {
            _population = populationSystem;
            Debug.Log("Sistema de Población conectado al Sistema de Nodos de Producción");
        }
        */
    }
}

// estructura para estadísticas
public struct ProductionStats
{
    public int TotalActiveNodes;
    public int TotalBaseProduction;
    public float ProductionInterval;
}