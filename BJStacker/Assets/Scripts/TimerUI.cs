using UnityEngine;
using TMPro;

public class TimerUI : MonoBehaviour
{
    [SerializeField] TMP_Text timerLabel;

    void Update()
    {
        if (timerLabel == null || StackGameController.Instance == null)
            return;

        int seconds = StackGameController.Instance.RemainingSeconds;
        timerLabel.text = seconds.ToString();
    }
}
