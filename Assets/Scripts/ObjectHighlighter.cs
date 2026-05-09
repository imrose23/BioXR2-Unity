using UnityEngine;

public class ObjectHighlighter : MonoBehaviour
{
    [Header("Highlight Settings")]
    public Color highlightColor = Color.yellow;

    private Renderer currentRenderer;
    private Color originalColor;
    private bool hasOriginalColor = false;

    public void Highlight(GameObject target)
    {
        Renderer renderer = target.GetComponent<Renderer>();

        if (renderer == null)
        {
            renderer = target.GetComponentInChildren<Renderer>();
        }

        if (renderer == null)
        {
            Debug.LogWarning("No renderer found on selected object: " + target.name);
            return;
        }

        ClearHighlight();

        currentRenderer = renderer;

        Material mat = currentRenderer.material;

        if (mat.HasProperty("_BaseColor"))
        {
            originalColor = mat.GetColor("_BaseColor");
            mat.SetColor("_BaseColor", highlightColor);
            hasOriginalColor = true;
        }
        else if (mat.HasProperty("_Color"))
        {
            originalColor = mat.GetColor("_Color");
            mat.SetColor("_Color", highlightColor);
            hasOriginalColor = true;
        }
        else
        {
            Debug.LogWarning("Material has no color property: " + mat.name);
        }
    }

    public void ClearHighlight()
    {
        if (currentRenderer == null || !hasOriginalColor) return;

        Material mat = currentRenderer.material;

        if (mat.HasProperty("_BaseColor"))
        {
            mat.SetColor("_BaseColor", originalColor);
        }
        else if (mat.HasProperty("_Color"))
        {
            mat.SetColor("_Color", originalColor);
        }

        currentRenderer = null;
        hasOriginalColor = false;
    }
}