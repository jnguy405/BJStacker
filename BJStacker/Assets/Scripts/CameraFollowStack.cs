using UnityEngine;

// Keeps the camera framing the growing stack.
public class CameraFollowStack : MonoBehaviour
{
    [SerializeField] float followSmoothing = 3f;
    [SerializeField] float heightOffset = 4f;
    [SerializeField] Vector3 localOffset = new Vector3(0f, 2f, -10f);

    void LateUpdate()
    {
        var controller = StackGameController.Instance;
        if (controller == null || controller.StackBase == null)
            return;

        float targetY = controller.HighestStackY + heightOffset;
        Vector3 desired = new Vector3(
            controller.StackBase.position.x + localOffset.x,
            targetY + localOffset.y,
            controller.StackBase.position.z + localOffset.z);
        transform.position = Vector3.Lerp(transform.position, desired, followSmoothing * Time.deltaTime);
    }
}
