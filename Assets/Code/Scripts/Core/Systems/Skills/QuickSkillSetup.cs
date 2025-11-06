using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class QuickSkillSetup : MonoBehaviour
{
    [ContextMenu("Setup Basic Skill Tree")]
    public void SetupBasicSkillTree()
    {
#if UNITY_EDITOR
        // Crear constelación ARES
        var aresConstellation = ScriptableObject.CreateInstance<SkillConstellation>();
        aresConstellation.constellationName = "ARES";

        // Crear nodos básicos para ARES
        var node1C1 = CreateSkillNode("1C1", "Aumenta capacidad básica de almacenamiento", 1);
        var node1A = CreateSkillNode("1A", "Mejora producción de metal", 2);
        node1A.prerequisiteNodes.Add(node1C1);

        aresConstellation.nodes.Add(node1C1);
        aresConstellation.nodes.Add(node1A);

        // Guardar assets
        AssetDatabase.CreateAsset(aresConstellation, "Assets/ARES_Constellation.asset");
        AssetDatabase.CreateAsset(node1C1, "Assets/SkillNode_1C1.asset");
        AssetDatabase.CreateAsset(node1A, "Assets/SkillNode_1A.asset");
        AssetDatabase.SaveAssets();

        Debug.Log("Configuración básica creada!");
#endif
    }

    private SkillNodeData CreateSkillNode(string name, string description, int cost)
    {
        var node = ScriptableObject.CreateInstance<SkillNodeData>();
        node.nodeName = name;
        node.description = description;
        node.skillPointCost = cost;

        // Añadir mejora de almacenamiento
        var improvement = new SkillImprovement();
        improvement.improvementType = SkillImprovement.ImprovementType.StorageCapacity;
        improvement.value = 50f;
        node.improvements.Add(improvement);

        return node;
    }
}