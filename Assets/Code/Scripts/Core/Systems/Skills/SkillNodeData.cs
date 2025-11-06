using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Skill Node", menuName = "NOVA/Skill Node")]
public class SkillNodeData : ScriptableObject
{
    [Header("Basic Information")]
    public string nodeName;
    [TextArea(3, 5)]
    public string description;
    public int skillPointCost = 1;

    [Header("Node Connections")]
    public List<SkillNodeData> prerequisiteNodes = new List<SkillNodeData>();

    [Header("Visual Position in Constellation")]
    public Vector2 positionInConstellation;

    [Header("Improvement Effects")]
    public List<SkillImprovement> improvements = new List<SkillImprovement>();
}

[System.Serializable]
public class SkillImprovement
{
    public enum ImprovementType
    {
        StorageCapacity,
        ProductionSpeed,
        ResourceEfficiency,
        UnlockFeature,
        CustomEffect
    }

    public ImprovementType improvementType;
    public string targetResource; // Para específico, vacío para todos
    public float value; // Valor de mejora
    public string customEffectId; // Para efectos personalizados
}