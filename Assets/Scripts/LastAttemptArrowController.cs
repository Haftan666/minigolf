using UnityEngine;

public class LastAttemptArrowController : MonoBehaviour
{
    private Vector3 lastArrowPosition;
    private Vector3 lastArrowScale;
    private Color lastArrowColor;
    private Quaternion lastArrowRotation;
    private Renderer arrowRenderer;

    void Awake()
    {
        arrowRenderer = GetComponent<Renderer>();
    }

    void Start()
    {
        arrowRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }


    public void ShowLastAttemptArrow()
    {
        gameObject.SetActive(true);
        transform.localPosition = new Vector3(lastArrowPosition.x, lastArrowPosition.y-0.001f, lastArrowPosition.z);
        transform.localScale = lastArrowScale;
        transform.rotation = lastArrowRotation;
        arrowRenderer.material.color = MakeColorFaded(lastArrowColor);
    }

    public void HideLastAttemptArrow()
    {
        gameObject.SetActive(false);
    }

    public void SaveLastArrowState(Vector3 position, Vector3 scale, Color color, Quaternion rotation)
    {
        lastArrowPosition = position;
        lastArrowScale = scale;
        lastArrowColor = color;
        lastArrowRotation = rotation;
    }

    private Color MakeColorFaded(Color color)
    {
        Color.RGBToHSV(color, out float h, out float s, out float v);
        s *= 0.3f;
        Color fadedColor = Color.HSVToRGB(h, s, v);
        fadedColor.a = 0.2f;
        return fadedColor;
    }
}
