using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class FocusBringNear : MonoBehaviour
{
    [Header("References")]
    public Transform xrCamera;

    [Header("Focus Settings")]
    public float focusDistance = 0.9f;
    public float moveSpeed = 8f;
    public float rotateSpeed = 8f;
    public float focusScaleMultiplier = 1.5f;

    [Header("Back Button")]
    public bool useLeftPrimaryButtonForBack = true;   // Usually X on left controller
    public bool useLeftSecondaryButtonForBack = false; // Set true if you want Y instead

    private XRGrabInteractable grabInteractable;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 originalScale;

    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private Vector3 targetScale;

    private bool isFocused = false;
    private bool returning = false;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        if (grabInteractable == null)
        {
            Debug.LogError("FocusBringNear requires XRGrabInteractable on the same object.");
            enabled = false;
            return;
        }

        grabInteractable.selectEntered.AddListener(OnSelected);
        grabInteractable.selectExited.AddListener(OnDeselected);

        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalScale = transform.localScale;

        targetPosition = originalPosition;
        targetRotation = originalRotation;
        targetScale = originalScale;
    }

    void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnSelected);
            grabInteractable.selectExited.RemoveListener(OnDeselected);
        }
    }

    void Update()
    {
        if (xrCamera == null)
            return;

        if (isFocused && LeftBackButtonPressed())
        {
            ReturnToOriginal();
        }

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotateSpeed);
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * moveSpeed);
    }

    private void OnSelected(SelectEnterEventArgs args)
    {
        if (!IsLeftHandInteractor(args))
            return;

        if (!isFocused && !returning)
        {
            originalPosition = transform.position;
            originalRotation = transform.rotation;
            originalScale = transform.localScale;
        }

        Vector3 camForward = xrCamera.forward;
        Vector3 camPos = xrCamera.position;

        targetPosition = camPos + camForward * focusDistance;

        // Make object face the user more nicely
        Vector3 toCamera = (camPos - transform.position).normalized;
        if (toCamera.sqrMagnitude < 0.001f)
            toCamera = -camForward;

        Vector3 flatForward = new Vector3(toCamera.x, 0f, toCamera.z).normalized;
        if (flatForward.sqrMagnitude < 0.001f)
            flatForward = -new Vector3(camForward.x, 0f, camForward.z).normalized;

        targetRotation = Quaternion.LookRotation(flatForward, Vector3.up);
        targetScale = originalScale * focusScaleMultiplier;

        isFocused = true;
        returning = false;
    }

    private void OnDeselected(SelectExitEventArgs args)
    {
        // Keep it near after release.
        // Return happens only via left controller button.
    }

    public void ReturnToOriginal()
    {
        targetPosition = originalPosition;
        targetRotation = originalRotation;
        targetScale = originalScale;

        isFocused = false;
        returning = true;
    }

    private bool IsLeftHandInteractor(SelectEnterEventArgs args)
    {
        if (args == null || args.interactorObject == null)
            return false;

        Transform t = args.interactorObject.transform;
        string n = t.name.ToLower();

        if (n.Contains("left"))
            return true;

        if (t.root != null && t.root.name.ToLower().Contains("left"))
            return true;

        Transform p = t.parent;
        while (p != null)
        {
            if (p.name.ToLower().Contains("left"))
                return true;
            p = p.parent;
        }

        return false;
    }

    private bool LeftBackButtonPressed()
    {
        InputDevice leftDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        if (!leftDevice.isValid)
            return false;

        bool primaryPressed = false;
        bool secondaryPressed = false;

        if (useLeftPrimaryButtonForBack)
            leftDevice.TryGetFeatureValue(CommonUsages.primaryButton, out primaryPressed);

        if (useLeftSecondaryButtonForBack)
            leftDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out secondaryPressed);

        return primaryPressed || secondaryPressed;
    }
}