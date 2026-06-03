using UnityEngine;

public class AlignmentScorer : MonoBehaviour
{
    public static AlignmentScorer Instance { get; private set; }

    Vector2 lastReferenceXZ;
    bool hasReference;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void OnPieceSettled(StackPiece piece)
    {
        var controller = StackGameController.Instance;
        if (controller == null) return;

        Collider col = piece.GetComponent<Collider>();
        if (col == null) col = piece.GetComponentInChildren<Collider>();

        Vector3 center = col != null ? col.bounds.center : piece.transform.position;
        Vector2 pieceXZ = new Vector2(center.x, center.z);

        if (!hasReference)
        {
            Transform stackBase = controller.StackBase;
            lastReferenceXZ = stackBase != null
                ? new Vector2(stackBase.position.x, stackBase.position.z)
                : Vector2.zero;
            hasReference = true;
        }

        float offset = Vector2.Distance(pieceXZ, lastReferenceXZ);

        float halfWidth = col != null ? Mathf.Min(col.bounds.size.x, col.bounds.size.z) * 0.5f : 1f;

        float score = Mathf.Clamp01(1f - (offset / halfWidth)) * 100f;

        Debug.Log($"Piece {controller.StackCount} alignment: {score:F1}% — {GetRating(score)}");

        lastReferenceXZ = pieceXZ;
    }

    string GetRating(float score)
    {
        if (score >= 90f) return "Excellent";
        if (score >= 75f) return "Good";
        if (score >= 50f) return "Okay";
        if (score >= 25f) return "Bad";
        return "Terrible";
    }
}
