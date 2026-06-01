using UnityEngine;
using UnityEngine.InputSystem;

// Oscillates along X until the player left-clicks, then drops straight down via physics.
[RequireComponent(typeof(StackPiece))]
public class ActiveMovingPiece : MonoBehaviour
{
    // State: limitX, speed, direction, isDropping
    float limitX;
    float speed;
    int direction = 1;
    bool isDropping;

    StackPiece stackPiece;

    void Awake()
    {
        stackPiece = GetComponent<StackPiece>();
    }

    public void Configure(float moveLimitX, float moveSpeed)
    {
        // Configure the ActiveMovingPiece component
        limitX = moveLimitX;
        speed = moveSpeed;
    }

    void Update()
    {
        // If the piece is dropping or the game is over, return
        if (isDropping || StackGameController.Instance == null || StackGameController.Instance.IsGameOver)
            return;

        if (WasDropPressedThisFrame())
            Drop();

        // If the piece is not dropping, move it side to side
        if (!isDropping)
            MoveSideToSide();
    }

    void MoveSideToSide()
    {
        // Move the piece side to side
        Vector3 position = transform.position;
        position.x += direction * speed * Time.deltaTime;

        // If the piece is at the limit, reverse the direction
        if (position.x >= limitX)
        {
            position.x = limitX;
            direction = -1;
        }
        else if (position.x <= -limitX)
        {
            position.x = -limitX;
            direction = 1;
        }

        transform.position = position;
    }

    static bool WasDropPressedThisFrame()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            return true;

        // Touch / pen fallback when no mouse
        return Touchscreen.current != null
            && Touchscreen.current.primaryTouch.press.wasPressedThisFrame;
    }

    void Drop()
    {
        // If the piece is already dropping, return
        if (isDropping)
            return;

        // Set the dropping flag and release the piece
        isDropping = true;
        stackPiece.ReleaseForDrop();
        // Notify the game controller that the piece is dropping
        StackGameController.Instance?.OnPieceDropped(this);
        // Disable the script
        enabled = false;
    }
}
