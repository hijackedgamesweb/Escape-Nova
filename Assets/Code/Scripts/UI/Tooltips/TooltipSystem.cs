using Code.Scripts.Patterns.Singleton;
using UnityEngine;

namespace Code.Scripts.UI.Tooltips
{
    public class TooltipSystem : MonoBehaviour
    {
        private static TooltipSystem _instance;
        [SerializeField] private Tooltip tooltip;

        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            Hide();
        }

        public static void Show(string content, string header = "")
        {
            if (_instance == null || _instance.tooltip == null) return;

            _instance.tooltip.SetText(content, header);
            _instance.tooltip.gameObject.SetActive(true);
        }

        public static void Hide()
        {
            if (_instance == null || _instance.tooltip == null) return;
            
            _instance.tooltip.gameObject.SetActive(false);
        }
    }
}