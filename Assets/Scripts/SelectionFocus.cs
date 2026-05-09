using UnityEngine;
using UnityEngine.XR;

public class SelectionFocus : MonoBehaviour
{
    [Header("References")]
    public Transform rayOrigin;

    [Header("Ray")]
    public float rayDistance = 100f;

    [Header("Marker")]
    public GameObject markerPrefab;
    public float markerScale = 0.03f;

    private GameObject currentMarker;
    private bool wasPressedLastFrame = false;

    void Update()
    {
        if (rayOrigin == null) return;

        InputDevice rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        bool triggerPressed = false;
        rightHand.TryGetFeatureValue(CommonUsages.triggerButton, out triggerPressed);

        if (triggerPressed && !wasPressedLastFrame)
        {
            SelectPoint();
        }

        wasPressedLastFrame = triggerPressed;
    }

    void SelectPoint()
    {
        Ray ray = new Ray(rayOrigin.position, rayOrigin.forward);

        RaycastHit[] hits = Physics.RaycastAll(ray, rayDistance);

        if (hits.Length == 0)
        {
            Debug.Log("No hit.");
            return;
        }

        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            // ignore parent box collider
            if (hit.collider is BoxCollider)
                continue;

            // use only mesh collider hits
            if (hit.collider is MeshCollider)
            {
                Debug.Log(
                    "Selected point on: " + hit.collider.name +
                    " | position: " + hit.point
                );

                PlaceMarker(hit.point);

                return;
            }
        }

        Debug.Log("Hit something, but no mesh collider selected.");
    }

    void PlaceMarker(Vector3 position)
    {
        if (currentMarker == null)
        {
            if (markerPrefab != null)
            {
                currentMarker = Instantiate(markerPrefab);
            }
            else
            {
                currentMarker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Destroy(currentMarker.GetComponent<Collider>());
            }
        }

        currentMarker.transform.position = position;
        currentMarker.transform.localScale = Vector3.one * markerScale;
    }
}