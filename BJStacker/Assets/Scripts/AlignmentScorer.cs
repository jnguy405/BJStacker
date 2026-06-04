using UnityEngine;

public class AlignmentScorer : MonoBehaviour
{
    public static AlignmentScorer Instance { get; private set; }

    Vector2 lastReferenceXZ;
    bool hasReference;

    public int TotalPoints { get; private set; }

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

        if (rating == "Perfect")
        {
            perfectStreak++;
            excellentStreak = 0;
        }
        else if (rating == "Excellent")
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

        string comboText = multiplier > 1f ? $" x{multiplier} COMBO" : "";
        Debug.Log($"Piece {controller.StackCount} alignment: {score:F1}% — {rating} (+{points}){comboText} | Total: {TotalPoints}");

        lastReferenceXZ = pieceXZ;
    }

    string GetRating(float score)
    {
        if (score >= 99f) return "Perfect";
        if (score >= 90f) return "Excellent";
        if (score >= 75f) return "Good";
        if (score >= 51f) return "Okay";
        if (score >= 25f) return "Bad";
        return "Terrible";
    }

    int GetPoints(string rating)
    {
        switch (rating)
        {
            case "Perfect":   return 200;
            case "Excellent": return 100;
            case "Good":      return 75;
            case "Okay":      return 50;
            case "Bad":       return 25;
            default:          return 10;
        }
    }
}
