using UnityEngine;
using UnityEngine.UI;

// Optional UI label showing current stack height. Assign a Text or TMP_Text in the inspector.
public class StackHeightUI : MonoBehaviour
{
    [SerializeField] Text heightLabel;

    void Update()
    {
        if (heightLabel == null || StackGameController.Instance == null)
            return;

        if (StackGameController.Instance.IsGameOver)
        {
            heightLabel.text = $"Height: {StackGameController.Instance.StackCount} — Game Over";
            return;
        }

        heightLabel.text = $"Height: {StackGameController.Instance.StackCount}";
    }
}
