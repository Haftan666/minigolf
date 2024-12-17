using UnityEngine;

public class ArrowController : MonoBehaviour
{
    public float arrowScaleFactor;
    public float maxArrowScale;

    private float currentArrowScale;
    private Renderer arrowRenderer;

    private Vector3 lastArrowPosition;
    private Vector3 lastArrowScale;
    private Color lastArrowColor;
    private Quaternion lastArrowRotation;
    private bool hasPreviousAttempt = false;
    private GameObject lastAttemptArrow;
    private Renderer lastAttemptArrowRenderer;

    void Start()
    {
        gameObject.SetActive(false);
        transform.localPosition = Vector3.zero;
        arrowRenderer = GetComponent<Renderer>();
        arrowRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        lastAttemptArrow = Instantiate(gameObject, transform.parent);
        lastAttemptArrow.name = "LastAttemptArrow";
        lastAttemptArrowRenderer = lastAttemptArrow.GetComponent<Renderer>();
        lastAttemptArrowRenderer.material = new Material(arrowRenderer.material);
        lastAttemptArrow.SetActive(false);
    }

    public void ShowArrow()
    {
        gameObject.SetActive(true);
        transform.localPosition = Vector3.zero;

        // Pokaż wyblakłą strzałkę, jeśli była poprzednia próba
        if (hasPreviousAttempt)
        {
            lastAttemptArrow.SetActive(true);
            lastAttemptArrow.transform.localPosition = lastArrowPosition;
            lastAttemptArrow.transform.localScale = lastArrowScale;
            lastAttemptArrow.transform.rotation = lastArrowRotation;
            lastAttemptArrowRenderer.material.color = MakeColorFaded(lastArrowColor);
        }
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
            color = Color.Lerp(new Color(0.8f, 1f, 0.8f), Color.green, t * 5); // Bardzo jasny zielony do zielonego
        }
        else if (t < 0.4f)
        {
            color = Color.Lerp(Color.green, new Color(1f, 1f, 0f), (t - 0.2f) * 5); // Zielony do żółtego
        }
        else if (t < 0.6f)
        {
            color = Color.Lerp(new Color(1f, 1f, 0f), new Color(1f, 0.5f, 0f), (t - 0.4f) * 5); // Żółty do pomarańczowego
        }
        else if (t < 0.8f)
        {
            color = Color.Lerp(new Color(1f, 0.5f, 0f), new Color(1f, 0.25f, 0f), (t - 0.6f) * 5); // Pomarańczowy do ciemnopomarańczowego
        }
        else
        {
            color = Color.Lerp(new Color(1f, 0.25f, 0f), Color.red, (t - 0.8f) * 5); // Ciemnopomarańczowy do czerwonego
        }

        arrowRenderer.material.color = color;
    }

    private Color MakeColorFaded(Color color)
    {
        // Zmniejsz nasycenie koloru i zwiększ przezroczystość
        Color.RGBToHSV(color, out float h, out float s, out float v);
        s *= 0.3f; // Zmniejsz nasycenie o połowę
        Color fadedColor = Color.HSVToRGB(h, s, v);
        fadedColor.a = 0.2f; // Ustaw przezroczystość na 0.3f
        return fadedColor;
    }

    public float GetCurrentArrowScale()
    {
        return currentArrowScale;
    }
    public void HideLastAttemptArrow()
    {
        if (lastAttemptArrow != null)
        {
            lastAttemptArrow.SetActive(false);
        }
    }

    public void SaveLastArrowState()
    {
        lastArrowPosition = transform.localPosition;
        lastArrowScale = transform.localScale;
        lastArrowColor = arrowRenderer.material.color;
        lastArrowRotation = transform.rotation;
        hasPreviousAttempt = true;
    }

    public void ResetArrowState()
    {
        hasPreviousAttempt = false;
        HideLastAttemptArrow();
    }
}
