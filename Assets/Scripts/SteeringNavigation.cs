using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class SteeringNavigation : MonoBehaviour
{
    public Transform headCamera;

    public InputActionProperty moveAction;      // XRI Left Locomotion / Move
    public InputActionProperty verticalAction;  // XRI Right Locomotion / Move

    public float moveSpeed = 2f;
    public float verticalSpeed = 2f;
    public float turnSpeed = 60f;

    public bool flyInLookDirection = true;

    [Header("Dataset Rotation")]
    public Transform datasetRoot;
    public float datasetSpinSpeed = 60f;

    private void OnEnable()
    {
        moveAction.action?.Enable();
        verticalAction.action?.Enable();
    }

    private void Update()
    {
        if (headCamera == null) return;

        Vector2 left = moveAction.action != null
            ? moveAction.action.ReadValue<Vector2>()
            : Vector2.zero;

        Vector2 rightStick = verticalAction.action != null
            ? verticalAction.action.ReadValue<Vector2>()
            : Vector2.zero;

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

        RotateDatasetWithGrip(rightStick);
    }

   private void RotateDatasetWithGrip(Vector2 rightStick)
{
    if (datasetRoot == null)
        return;

    UnityEngine.XR.InputDevice rightDevice =
        UnityEngine.XR.InputDevices.GetDeviceAtXRNode(
            UnityEngine.XR.XRNode.RightHand
        );

    bool rightGrip;

    if (!rightDevice.TryGetFeatureValue(
            UnityEngine.XR.CommonUsages.gripButton,
            out rightGrip))
        return;

    if (!rightGrip)
        return;

    float spin =
        rightStick.x *
        datasetSpinSpeed *
        Time.deltaTime;

    datasetRoot.Rotate(
        Vector3.up,
        spin,
        Space.World
    );
}
}