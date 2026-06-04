using System.Collections;
using UnityEngine;
using Utilities;

// Runs the stack loop: spawn → move → drop → settle → score or game over.
public class StackGameController : MonoBehaviour
{
    public static StackGameController Instance { get; private set; }

    [Header("References")]
    [SerializeField] Transform stackBase;

    [Header("Spawn")]
    [SerializeField] float spawnHeightAboveStack = 2f;

    [Header("Timer")]
    [SerializeField] float gameDurationSeconds = 30f;

    [Header("Game Over")]
    [SerializeField] float fallKillY = -2f;
    [SerializeField] float maxStackTiltDegrees = 55f;
    [SerializeField] float stackCheckInterval = 0.25f;

    public int StackCount { get; private set; }
    public bool IsGameOver { get; private set; }
    public float HighestStackY => highestStackY;
    public int RemainingSeconds => gameTimer?.RemainingSeconds ?? 0;
    public Transform StackBase => stackBase;

    PieceSpawner pieceSpawner;
    Timer gameTimer;

    float highestStackY;
    ActiveMovingPiece activePiece;
    Coroutine stackMonitorRoutine;


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        pieceSpawner = GetComponent<PieceSpawner>();
    }

    void Start()
    {
        // Initialize the highest stack Y
        if (stackBase != null)
            highestStackY = stackBase.position.y;
        else
            highestStackY = 0f;

        gameTimer = new Timer(gameDurationSeconds);
        gameTimer.Start();

        // Begin the next piece
        BeginNextPiece();
        stackMonitorRoutine = StartCoroutine(MonitorStackRoutine());
    }

    void Update()
    {
        if (IsGameOver || gameTimer == null || !gameTimer.IsRunning)
            return;

        gameTimer.Tick(Time.deltaTime);
        if (gameTimer.IsFinished)
            EndGame("Time's up!");
    }

    void OnDestroy()
    {
        // Set the instance to null
        if (Instance == this)
            Instance = null;
    }

    public void OnPieceDropped(ActiveMovingPiece piece)
    {
        // Set the active piece
        activePiece = piece;
    }

    public void OnPieceSettled(StackPiece piece)
    {
        // If the game is over or the piece is null, return
        if (IsGameOver || piece == null)
            return;

        // If the piece is not valid, end the game
        if (!ValidatePlacedPiece(piece))
        {
            EndGame("Piece missed the stack.");
            return;
        }

        StackCount++;
        DifficultyManager.Instance?.OnPieceSuccessfullyPlaced();
        UpdateHighestStackY(piece);
        activePiece = null;
        BeginNextPiece();
    }

    bool ValidatePlacedPiece(StackPiece piece)
    {
        // If the piece is below the fall kill Y, return false
        if (piece.transform.position.y < fallKillY)
            return false;

        // If the stack base is not null, check the horizontal distance
        if (stackBase != null)
        {
            float horizontalDistance = Vector2.Distance(
                new Vector2(piece.transform.position.x, piece.transform.position.z),
                new Vector2(stackBase.position.x, stackBase.position.z));

            float maxReach = pieceSpawner != null ? pieceSpawner.MoveLimitX * 2.5f : 10f;
            if (horizontalDistance > maxReach)
                return false;
        }

        return true;
    }

    void UpdateHighestStackY(StackPiece piece)
    {
        // Get the top world Y of the piece
        float top = piece.GetTopWorldY();
        if (top > highestStackY)
            highestStackY = top;
    }

    void BeginNextPiece()
    {
        // If the game is over or the piece spawner is null, return
        if (IsGameOver || pieceSpawner == null)
            return;

        float spawnY = highestStackY + spawnHeightAboveStack;
        float spawnZ = stackBase != null ? stackBase.position.z : 0f;
        Vector3 spawnPos = new Vector3(0f, spawnY, spawnZ);
        GameObject spawned = pieceSpawner.SpawnPiece(spawnPos);
        if (spawned == null)
            return;

        activePiece = spawned.GetComponent<ActiveMovingPiece>();
    }

    // Monitor the stack for collapse
    IEnumerator MonitorStackRoutine()
    {
        var wait = new WaitForSeconds(stackCheckInterval);
        while (!IsGameOver)
        {
            if (CheckStackCollapsed())
                EndGame("The stack fell.");

            yield return wait;
        }
    }

    // Check if the stack has collapsed
    bool CheckStackCollapsed()
    {
        // Find all the stack pieces
        var pieces = FindObjectsByType<StackPiece>(FindObjectsSortMode.None);
        foreach (var piece in pieces)
        {
            // If the piece is not placed, continue
            if (!piece.IsPlaced)
                continue;

            // If the piece is below the fall kill Y, return true
            if (piece.transform.position.y < fallKillY)
                return true;

            // If the piece is tilted more than the max stack tilt degrees, return true
            float tilt = piece.GetMaxTiltDegrees();
            if (tilt > maxStackTiltDegrees)
                return true;
        }

        return false;
    }

    public void EndGame(string reason)
    {
        // If the game is already over, return
        if (IsGameOver)
            return;

        IsGameOver = true;
        Debug.Log($"Game Over — Stack height: {StackCount}. {reason}");

        // Disable the active piece
        if (activePiece != null)
            activePiece.enabled = false;

        // Stop the stack monitor routine
        if (stackMonitorRoutine != null)
            StopCoroutine(stackMonitorRoutine);
    }
}
