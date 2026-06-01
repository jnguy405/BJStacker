using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance { get; private set; }

    [Header("Speed Settings")]
    [SerializeField] float startSpeed = 2f;
    [SerializeField] float speedIncrement = 0.2f;
    [SerializeField] float maxSpeed = 6f;

    public float CurrentSpeed { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        CurrentSpeed = startSpeed;
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void OnPieceSuccessfullyPlaced()
    {
        CurrentSpeed = Mathf.Min(CurrentSpeed + speedIncrement, maxSpeed);
    }
}
