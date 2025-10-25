using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    public PlayerStats player;

    [SerializeField]
    private Canvas canvas;
    private bool seeCanvas = false;

    void Update()
    {
        // Toggle correcto del canvas con Tab
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            seeCanvas = !seeCanvas;
            if (canvas != null)
                canvas.gameObject.SetActive(seeCanvas);
        }
    }
}
