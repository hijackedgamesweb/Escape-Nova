using UnityEngine;

namespace Code.Scripts.UI.Windows
{
    public abstract class BaseUIScreen : MonoBehaviour
    {
        public virtual void Show(object parameter = null) => gameObject.SetActive(true);
        public virtual void Hide() => gameObject.SetActive(false);
    }
}