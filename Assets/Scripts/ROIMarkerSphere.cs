using UnityEngine;
using UnityEngine.XR;

public class ROIMarkerSphere : MonoBehaviour
{
    [Header("References")]
    public Transform rayOrigin;
    public Transform dataRoot;

    [Header("Ray")]
    public float rayDistance = 100f;

    [Header("ROI")]
    public float roiRadius = 0.05f;

    private GameObject roiSphere;
    private bool wasPressedLastFrame = false;

    void Start()
    {
        Physics.queriesHitBackfaces = true;
    }

    void Update()
    {
        if (rayOrigin == null) return;

        InputDevice rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        bool triggerPressed = false;
        rightHand.TryGetFeatureValue(CommonUsages.triggerButton, out triggerPressed);

        if (triggerPressed && !wasPressedLastFrame)
        {
            CreateROI();
        }

        wasPressedLastFrame = triggerPressed;
    }

    void CreateROI()
    {
        Debug.Log("CREATE ROI CALLED");

        Vector3 origin = rayOrigin.position;
        Vector3 direction = rayOrigin.forward.normalized;

        Debug.Log("RayOrigin name: " + rayOrigin.name);
        Debug.Log("Origin: " + origin);
        Debug.Log("Forward: " + direction);

        Debug.DrawLine(origin, origin + direction * rayDistance, Color.green, 5f);

        Ray ray = new Ray(origin, direction);

        RaycastHit[] hits = Physics.RaycastAll(ray, rayDistance);

        if (hits.Length == 0)
        {
            Debug.Log("No hit.");
            return;
        }

        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider == null)
                continue;

            // Ignore the ROI marker itself
            if (roiSphere != null && hit.collider.gameObject == roiSphere)
                continue;

            // Ignore controller/ray-origin related colliders
            if (hit.collider.transform.IsChildOf(rayOrigin))
                continue;

            Debug.Log(
                "VALID HIT: " + hit.collider.name +
                " | collider type: " + hit.collider.GetType().Name +
                " | distance: " + hit.distance +
                " | point: " + hit.point
            );

            PlaceROISphere(hit.point);
            return;
        }

        Debug.Log("Hit something, but no valid collider selected.");
    }

    void PlaceROISphere(Vector3 center)
    {
        if (roiSphere == null)
        {
            roiSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            roiSphere.name = "ROI_Marker_Sphere";

            Collider col = roiSphere.GetComponent<Collider>();
            if (col != null)
                Destroy(col);

            if (dataRoot != null)
                roiSphere.transform.SetParent(dataRoot, true);
        }

        roiSphere.transform.position = center;
        roiSphere.transform.localScale = Vector3.one * roiRadius * 2f;
    }
}