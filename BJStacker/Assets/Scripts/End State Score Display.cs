using UnityEngine;
using TMPro;

public class EndStateScoreDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scoreText.text = "" + AlignmentScorer.TotalPoints;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
