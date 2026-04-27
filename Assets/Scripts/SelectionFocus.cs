using UnityEngine;
using UnityEngine.InputSystem;

public class SelectionFocus : MonoBehaviour
{
    [Header("References")]
    public Transform headCamera;
    public Transform rayOrigin;   // Right Controller / Near-Far Interactor / Ray Transform

    [Header("Input")]
    public InputActionProperty focusAction;

    [Header("Ray Settings")]
    public float rayDistance = 100f;
    public LayerMask selectableLayers = ~0;

    [Header("Focus Settings")]
    public float focusDistance = 1.5f;
    public float moveSpeed = 3.0f;

    private bool isMoving = false;
    private Vector3 targetRigPosition;

    void Update()
    {
        if (headCamera == null || rayOrigin == null) return;

        if (focusAction.action.WasPressedThisFrame())
        {
            TryFocus();
        }

        if (isMoving)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                targetRigPosition,
                moveSpeed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, targetRigPosition) < 0.03f)
            {
                isMoving = false;
            }
        }
    }

    void TryFocus()
    {
        Ray ray = new Ray(rayOrigin.position, rayOrigin.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, selectableLayers))
        {
            Vector3 hitPoint = hit.point;
            targetRigPosition = hitPoint - headCamera.forward * focusDistance;
            isMoving = true;

            Debug.Log("Focus target hit: " + hit.collider.name);
        }
        else
        {
            Debug.Log("No focus target hit.");
        }
    }
}