using UnityEngine;
using UnityEngine.UI;

// Optional UI label showing the countdown timer. Assign a Text in the inspector.
public class TimerUI : MonoBehaviour
{
    [SerializeField] Text timerLabel;

    void Update()
    {
        if (timerLabel == null || StackGameController.Instance == null)
            return;

        int seconds = StackGameController.Instance.RemainingSeconds;
        timerLabel.text = seconds.ToString();
    }
}
