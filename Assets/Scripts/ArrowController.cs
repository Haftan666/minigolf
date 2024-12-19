using UnityEngine;

public class ArrowController : MonoBehaviour
{
    public float arrowScaleFactor;
    public float maxArrowScale;

    private float currentArrowScale;
    private Renderer arrowRenderer;

    void Start()
    {
        gameObject.SetActive(false);
        transform.localPosition = Vector3.zero;
        arrowRenderer = GetComponent<Renderer>();
        arrowRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }

    public void ShowArrow()
    {
        gameObject.SetActive(true);
        transform.localPosition = Vector3.zero;
    }

    public void HideArrow()
    {
        gameObject.SetActive(false);
    }

    public void UpdateArrow(Vector3 initialMousePosition, Vector3 currentMousePosition, float maxForce)
    {
        Vector3 direction = initialMousePosition - currentMousePosition;
        direction.z = direction.y;
        direction.y = 0;

        float angle = Vector3.SignedAngle(Vector3.forward, direction, Vector3.up);
        if (angle < -60f || angle > 60f)
        {
            float clampedAngle = Mathf.Clamp(angle, -60f, 60f);
            direction = Quaternion.Euler(0, clampedAngle - angle, 0) * direction;
        }

        float forceMagnitude = Mathf.Clamp(direction.magnitude, 0, maxForce);

        currentArrowScale = Mathf.Min(direction.magnitude * arrowScaleFactor, maxArrowScale);

        transform.localScale = new Vector3(1.7f, 0.1f, currentArrowScale);

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        UpdateArrowColor(currentArrowScale / maxArrowScale);
    }

    private void UpdateArrowColor(float t)
    {
        Color color;
        if (t < 0.2f)
        {
            color = Color.Lerp(new Color(0.8f, 1f, 0.8f), Color.green, t * 5);
        }
        else if (t < 0.4f)
        {
            color = Color.Lerp(Color.green, new Color(1f, 1f, 0f), (t - 0.2f) * 5);
        }
        else if (t < 0.6f)
        {
            color = Color.Lerp(new Color(1f, 1f, 0f), new Color(1f, 0.5f, 0f), (t - 0.4f) * 5);
        }
        else if (t < 0.8f)
        {
            color = Color.Lerp(new Color(1f, 0.5f, 0f), new Color(1f, 0.25f, 0f), (t - 0.6f) * 5);
        }
        else
        {
            color = Color.Lerp(new Color(1f, 0.25f, 0f), Color.red, (t - 0.8f) * 5);
        }

        arrowRenderer.material.color = color;
    }

    public float GetCurrentArrowScale()
    {
        return currentArrowScale;
    }
}
