using UnityEngine;

// Instantiates stack piece prefabs in sequence.
public class PieceSpawner : MonoBehaviour
{
    public static PieceSpawner Instance { get; private set; }

    [Header("References")]
    [SerializeField] GameObject[] piecePrefabs;

    [Header("Movement")]
    [SerializeField] float moveLimitX = 3f;
    [SerializeField] float moveSpeed = 2f;

    public float MoveLimitX => moveLimitX;

    int nextPrefabIndex;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public GameObject SpawnPiece(Vector3 position)
    {
        // If the piece prefabs are not assigned, return null
        if (piecePrefabs == null || piecePrefabs.Length == 0)
        {
            Debug.LogError("PieceSpawner: assign at least one piece prefab.");
            return null;
        }

        // Get the next prefab
        GameObject prefab = piecePrefabs[nextPrefabIndex]; // Random.Range(0,piecePrefabs.Length) for random drops
        nextPrefabIndex = (nextPrefabIndex + 1) % piecePrefabs.Length;

        // Instantiate the prefab
        GameObject instance = Instantiate(prefab, position, Quaternion.identity);

        var mover = instance.GetComponent<ActiveMovingPiece>();
        // If the ActiveMovingPiece component is not found, add it
        if (mover == null)
            mover = instance.AddComponent<ActiveMovingPiece>();

        // Configure the ActiveMovingPiece component
        float speed = DifficultyManager.Instance != null ? DifficultyManager.Instance.CurrentSpeed : moveSpeed;
        mover.Configure(moveLimitX, speed);

        var piece = instance.GetComponent<StackPiece>();
        // If the StackPiece component is not found, add it
        if (piece == null)
            piece = instance.AddComponent<StackPiece>();

        piece.PrepareForMovement();

        return instance;
    }
}
