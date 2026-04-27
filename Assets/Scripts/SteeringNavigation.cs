using UnityEngine;
using UnityEngine.InputSystem;

public class SteeringNavigation : MonoBehaviour
{
    public Transform headCamera;

    public InputActionProperty moveAction;      // XRI Left Locomotion / Move
    public InputActionProperty verticalAction;  // XRI Right Locomotion / Move

    public float moveSpeed = 2f;
    public float verticalSpeed = 2f;
    public float turnSpeed = 60f;

    public bool flyInLookDirection = true;

    private void OnEnable()
    {
        moveAction.action?.Enable();
        verticalAction.action?.Enable();
    }

    private void Update()
    {
        if (headCamera == null) return;

        Vector2 left = moveAction.action != null ? moveAction.action.ReadValue<Vector2>() : Vector2.zero;
        Vector2 rightStick = verticalAction.action != null ? verticalAction.action.ReadValue<Vector2>() : Vector2.zero;

        // Debug: check Console while moving right joystick up/down
        if (Mathf.Abs(rightStick.y) > 0.1f)
            Debug.Log("RIGHT STICK Y = " + rightStick.y);

        Vector3 forward = headCamera.forward;
        Vector3 right = headCamera.right;

        if (!flyInLookDirection)
        {
            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();
        }

        Vector3 move =
            forward * left.y * moveSpeed +
            right * left.x * moveSpeed +
            Vector3.up * rightStick.y * verticalSpeed;

        transform.position += move * Time.deltaTime;

        float turn = rightStick.x * turnSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up, turn);
    }
}