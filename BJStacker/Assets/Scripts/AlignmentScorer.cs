using UnityEngine;

public class AlignmentScorer : MonoBehaviour
{
    public static AlignmentScorer Instance { get; private set; }

    [SerializeField] AlignmentUI alignmentUI;

    Vector2 lastReferenceXZ;
    bool hasReference;

    public static int TotalPoints { get; private set; }

    int perfectStreak;
    int excellentStreak;

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
    void Start()
    {
        TotalPoints = 0;
        perfectStreak = 0;
        excellentStreak = 0;
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
        string rating = GetRating(score);

        if (rating == "PERFECT!!!")
        {
            perfectStreak++;
            excellentStreak = 0;
        }
        else if (rating == "Excellent!")
        {
            excellentStreak++;
            perfectStreak = 0;
        }
        else
        {
            perfectStreak = 0;
            excellentStreak = 0;
        }

        float multiplier = 1f;
        if (perfectStreak >= 2) multiplier = 1.5f;
        else if (excellentStreak >= 2) multiplier = 1.25f;

        int points = Mathf.RoundToInt(GetPoints(rating) * multiplier);
        TotalPoints += points;

        string comboLog = multiplier > 1f ? $" x{multiplier} COMBO" : "";
        Debug.Log($"Piece {controller.StackCount} alignment: {score:F1}% — {rating} (+{points}){comboLog} | Total: {TotalPoints}");

        int streak = perfectStreak > 0 ? perfectStreak : excellentStreak;
        alignmentUI?.UpdateUI(rating, points, multiplier, TotalPoints, streak);

        lastReferenceXZ = pieceXZ;
    }

    string GetRating(float score)
    {
        if (score >= 99f) return "PERFECT!!!";
        if (score >= 90f) return "Excellent!";
        if (score >= 75f) return "Good";
        if (score >= 51f) return "okay";
        if (score >= 25f) return "bad.";
        return "terrible...";
    }

    int GetPoints(string rating)
    {
        switch (rating)
        {
            case "PERFECT!!!":  return 200;
            case "Excellent!":  return 100;
            case "Good":        return 75;
            case "okay":        return 50;
            case "bad.":        return 25;
            default:            return 10;
        }
    }
}
