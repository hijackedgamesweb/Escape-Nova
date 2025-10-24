using Code.Scripts.Core.Systems.Production;
using Code.Scripts.Patterns.ServiceLocator;
using UnityEngine;
public class GameTester : MonoBehaviour
{
    private ProductionNodeSystem _productionSystem;
    private void Start()
    {
        _productionSystem = ServiceLocator.GetService<ProductionNodeSystem>();
        
        // Crear nodo de prueba
        var testNode = new ProductionNode
        {
            NodeId = "test_mine",
            Category = NodeCategory.Construcción,
            Position = new Vector2Int(1, 1)
        };
        
        // Configurar qué produce
        _productionSystem.ConfigureNodeProduction(testNode);
        
        // Agregar al sistema
        _productionSystem.AddNode(testNode);
        
        // cada 5 segundos se vera en consola:
        // "ciclo de producción: X recursos generados de 1 nodos"
        // y los recursos en StorageSystem aumentarán automáticamente
    }
}
