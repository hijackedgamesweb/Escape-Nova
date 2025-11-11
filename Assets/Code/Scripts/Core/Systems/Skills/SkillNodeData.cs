using System;
using UnityEngine;
using System.Collections.Generic;
using Code.Scripts.Core.Systems.Skills.SkillImprovements;
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
    [SerializeReference, SubclassSelector]
    public List<SkillImprovement> improvements = new List<SkillImprovement>();
}