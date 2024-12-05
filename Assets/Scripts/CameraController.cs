using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector3 initialOffset;
    private Quaternion initialRotation;

    void Start()
    {
        initialOffset = transform.position - transform.parent.position; // Zapisz początkowy offset kamery względem piłki
        initialRotation = transform.rotation; // Zapisz początkową rotację kamery
    }

    void LateUpdate()
    {
        transform.position = transform.parent.position + initialOffset; // Ustaw pozycję kamery na podstawie początkowego offsetu
        transform.rotation = initialRotation; // Resetuj rotację kamery do początkowej wartości
    }
}
