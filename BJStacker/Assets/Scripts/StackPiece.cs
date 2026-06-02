using System.Collections;
using UnityEngine;

// Physics and settle detection for one stack piece.
// Lean: freeze yaw (Y) only so the tower can tip on X and Z.
[RequireComponent(typeof(Rigidbody))]
public class StackPiece : MonoBehaviour
{
    const float SettleVelocityThreshold = 0.08f;
    const float SettleTimeRequired = 0.35f;

    public bool IsPlaced { get; private set; }

    Rigidbody body;
    Collider pieceCollider;
    float settledTimer;

    void Awake()
    {
        // Get the rigidbody component
        body = GetComponent<Rigidbody>();
        pieceCollider = GetComponent<Collider>();
        // If the piece collider is not found, get the collider component from the children
        if (pieceCollider == null)
            pieceCollider = GetComponentInChildren<Collider>();
    }

    public void PrepareForMovement()
    {
        IsPlaced = false;
        settledTimer = 0f;

        body.isKinematic = true;
        body.useGravity = false;
        ApplyLeanConstraints();
    }

    // When player left-clicks, release the piece for drop
    public void ReleaseForDrop()
    {
        IsPlaced = false;
        settledTimer = 0f;

        body.isKinematic = false;
        body.useGravity = true;
        body.linearVelocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        body.constraints = RigidbodyConstraints.None;
        ApplyLeanConstraints();
        StartCoroutine(WaitUntilSettled());
    }

    void ApplyLeanConstraints()
    {
        body.constraints = RigidbodyConstraints.FreezeRotationY;
    }

    // Wait until the piece is settled to notify the game controller
    IEnumerator WaitUntilSettled()
    {
        yield return new WaitForSeconds(0.05f);

        while (true)
        {
            // If the game is over, break the coroutine
            if (StackGameController.Instance != null && StackGameController.Instance.IsGameOver)
                yield break;

            // Check if the piece is slow enough to be settled
            bool slowEnough = body.linearVelocity.magnitude <= SettleVelocityThreshold
                && body.angularVelocity.magnitude <= SettleVelocityThreshold;
            

            // If the piece is slow enough, increment the settled timer
            if (slowEnough)
            {
                Debug.Log("Piece Frozen");
                body.linearVelocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
                body.constraints = RigidbodyConstraints.FreezePositionX;                
                settledTimer += Time.deltaTime;
                if (settledTimer >= SettleTimeRequired)
                {
                    IsPlaced = true;
                    // Notify the game controller that the piece is settled
                    StackGameController.Instance?.OnPieceSettled(this);
                    yield break;
                }
            }
            else
            {
                // Reset the settled timer
                settledTimer = 0f;
            }

            // Wait for the next frame
            yield return null;
        }
    }

    public float GetTopWorldY()
    {
        // If the piece collider is not null, return the top world Y
        if (pieceCollider != null)
            return pieceCollider.bounds.max.y;

        // Return the top world Y
        return transform.position.y + transform.localScale.y * 0.5f;
    }

    public float GetMaxTiltDegrees()
    {
        // Get the Euler angles of the piece
        Vector3 euler = transform.eulerAngles;
        float tiltX = euler.x > 180f ? euler.x - 360f : euler.x;
        float tiltZ = euler.z > 180f ? euler.z - 360f : euler.z;
        return Mathf.Max(Mathf.Abs(tiltX), Mathf.Abs(tiltZ));
    }
}
