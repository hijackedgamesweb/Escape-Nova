using UnityEngine;
using System.Collections.Generic;
using Code.Scripts.Core.Systems.Astrarium;

[CreateAssetMenu(fileName = "New Constellation", menuName = "NOVA/Skill Constellation")]
public class SkillConstellation : ScriptableObject, IAstrariumEntry
{
    public string constellationName;
    
    [Header("Astrarium Data")]
    [TextArea] public string description;
    public Sprite icon;

    public List<SkillNodeData> nodes = new List<SkillNodeData>();

    public string GetAstrariumID() => $"constellation_{constellationName.ToLower().Replace(" ", "_")}";
    public string GetDisplayName() => constellationName;
    public string GetDescription() => description;
    public Sprite GetIcon() => icon;
    public AstrariumCategory GetCategory() => AstrariumCategory.Constellation;
    public GameObject Get3DModel() => null;
}