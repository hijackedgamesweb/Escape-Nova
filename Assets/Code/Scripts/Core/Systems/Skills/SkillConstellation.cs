using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Constellation", menuName = "NOVA/Skill Constellation")]
public class SkillConstellation : ScriptableObject
{
    public string constellationName;
    public List<SkillNodeData> nodes = new List<SkillNodeData>();
}