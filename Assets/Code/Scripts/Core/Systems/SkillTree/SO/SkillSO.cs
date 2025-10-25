using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "Create/Skill")]
public class SkillSO : ScriptableObject
{
    [TextArea] public string description;
    public Sprite icon;
    public int abilityPointsNeeded;

    public List<PlayerAttributes> affectedAttributes = new List<PlayerAttributes>();

    // Rellena los campos de UI (SkillDisplay) de forma segura.
    public void SetValues(GameObject skillDisplayObject)
    {
        if (skillDisplayObject == null) return;

        SkillDisplay SD = skillDisplayObject.GetComponent<SkillDisplay>();
        if (SD == null) return;

        if (SD.skillName) SD.skillName.text = this.name; // nombre del ScriptableObject
        if (SD.skillDescription) SD.skillDescription.text = description;
        if (SD.skillAbilityPointsNeeded) SD.skillAbilityPointsNeeded.text = abilityPointsNeeded.ToString();
        if (SD.skillIcon && icon != null) SD.skillIcon.sprite = icon;

        // Validación de lista antes de acceder al primer elemento
        if (affectedAttributes != null && affectedAttributes.Count > 0)
        {
            var first = affectedAttributes[0];
            if (first != null)
            {
                if (SD.skillAttribute) SD.skillAttribute.text = first.attribute != null ? first.attribute.attributeName : "N/A";
                if (SD.skillAttrAmount) SD.skillAttrAmount.text = "+" + first.amount.ToString();
            }
        }
        else
        {
            if (SD.skillAttribute) SD.skillAttribute.text = "";
            if (SD.skillAttrAmount) SD.skillAttrAmount.text = "";
        }
    }

    public bool CheckSkills(PlayerStats player)
    {
        if (player == null) return false;
        return player.abilityPoints >= abilityPointsNeeded;
    }

    public bool EnableSkill(PlayerStats player)
    {
        if (player == null || player.playerSkills == null) return false;
        // Comprobamos si ya lo tiene (por referencia)
        return player.playerSkills.Contains(this);
    }

    public bool GetSkill(PlayerStats player)
    {
        if (player == null) return false;

        // Requisitos mínimos
        if (player.abilityPoints < abilityPointsNeeded) return false;
        if (player.playerSkills == null) player.playerSkills = new List<SkillSO>();
        if (player.attributesList == null) return false;

        // Evitar añadir dos veces la misma skill
        if (player.playerSkills.Contains(this)) return false;

        int matches = 0;

        if (affectedAttributes != null && affectedAttributes.Count > 0)
        {
            foreach (var attr in affectedAttributes)
            {
                if (attr == null || attr.attribute == null) continue;

                foreach (var playerAttr in player.attributesList)
                {
                    if (playerAttr == null || playerAttr.attribute == null) continue;

                    // Comparación por referencia (más segura), si prefieres comparar por nombre usa .attributeName
                    if (attr.attribute == playerAttr.attribute)
                    {
                        playerAttr.amount += attr.amount;
                        matches++;
                    }
                }
            }
        }

        if (matches > 0)
        {
            player.abilityPoints -= abilityPointsNeeded;
            player.playerSkills.Add(this);
            return true;
        }

        return false;
    }
}
