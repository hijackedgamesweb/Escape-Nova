using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillDisplay : MonoBehaviour
{
    public SkillSO skill;

    public TextMeshProUGUI skillName;
    public TextMeshProUGUI skillDescription;
    public Image skillIcon;
    public TextMeshProUGUI skillAbilityPointsNeeded;
    public TextMeshProUGUI skillAttribute;
    public TextMeshProUGUI skillAttrAmount;

    [Tooltip("Asigna aquí el PlayerStats (preferred). Si no, el script buscará el PlayerHandler en la escena.")]
    [SerializeField]
    private PlayerStats _playerStats;

    private Button _button;

    void Awake()
    {
        _button = GetComponent<Button>();
    }

    void Start()
    {
        // Si no se asignó en el inspector intentamos buscar el PlayerHandler en la escena
        if (_playerStats == null)
        {
            PlayerHandler handler = FindObjectOfType<PlayerHandler>();
            if (handler != null) _playerStats = handler.player;
        }

        // Suscribimos al evento si existe
        if (_playerStats != null)
        {
            _playerStats.onAbilityPointChange += ReactToChange;
        }

        if (skill != null) skill.SetValues(this.gameObject);
        EnableSkills();
    }

    private void OnEnable()
    {
        EnableSkills();
    }

    public void EnableSkills()
    {
        if (_playerStats == null || skill == null)
        {
            // Si falta cualquiera, dejamos el botón no interactuable por seguridad
            if (_button != null) _button.interactable = false;
            return;
        }

        // Si ya lo tiene
        if (skill.EnableSkill(_playerStats))
        {
            TurnOnSkillIcon();
        }
        // Si puede permitírselo
        else if (skill.CheckSkills(_playerStats))
        {
            if (_button != null) _button.interactable = true;
            var disabled = transform.Find("IconParent")?.Find("Disabled")?.gameObject;
            if (disabled != null) disabled.SetActive(false);
            var available = transform.Find("IconParent")?.Find("Available")?.gameObject;
            if (available != null) available.SetActive(true);
        }
        else
        {
            TurnOffSkillIcon();
        }
    }

    public void GetSkill()
    {
        if (_playerStats == null || skill == null) return;
        if (skill.GetSkill(_playerStats)) TurnOnSkillIcon();
    }

    private void TurnOnSkillIcon()
    {
        if (_button != null) _button.interactable = false;
        transform.Find("IconParent")?.Find("Available")?.gameObject.SetActive(false);
        transform.Find("IconParent")?.Find("Disabled")?.gameObject.SetActive(false);
    }

    private void TurnOffSkillIcon()
    {
        if (_button != null) _button.interactable = false;
        transform.Find("IconParent")?.Find("Available")?.gameObject.SetActive(true);
        transform.Find("IconParent")?.Find("Disabled")?.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        if (_playerStats != null) _playerStats.onAbilityPointChange -= ReactToChange;
    }

    void ReactToChange()
    {
        EnableSkills();
    }
}
