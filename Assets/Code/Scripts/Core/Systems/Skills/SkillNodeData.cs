using UnityEngine;
using System.Collections.Generic;
using ResourceType = Code.Scripts.Core.Systems.Resources.ResourceType;

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

    [Header("Improvement Type")]
    public ImprovementType improvementType;

    [Header("Target Settings")]
    [Tooltip("Select which resource this improvement affects")]
    public ResourceType targetResource;

    [Tooltip("Apply this improvement to all resources?")]
    public bool applyToAllResources = false;

    [Header("Improvement Value")]
    public float value;

    [Header("Custom Effect")]
    [Tooltip("Only used for UnlockFeature and CustomEffect types")]
    public string customEffectId;
}