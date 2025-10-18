using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Player Stats")]
    public string playerName;

    [SerializeField]
    private int _abilityPoints = 0;
    public int abilityPoints
    {
        get { return _abilityPoints; }
        set
        {
            _abilityPoints = value;
            if (onAbilityPointChange != null) onAbilityPointChange();
        }
    }

    [Header("Player Attributes")]
    public List<PlayerAttributes> attributesList = new List<PlayerAttributes>();

    [Header("Player Skills Enabled")]
    public List<SkillSO> playerSkills = new List<SkillSO>();

    public delegate void OnAbilityPointChange();
    public event OnAbilityPointChange onAbilityPointChange;

    public void UpdateAbilityPoint(int amount)
    {
        abilityPoints += amount;
    }
}
