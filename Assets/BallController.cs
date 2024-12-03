using UnityEngine;
using UnityEngine.SceneManagement;

public class BallController : MonoBehaviour
{
    public GameObject arrow; 
    public float maxForce;
    public float arrowScaleFactor;
    public float maxArrowScale;
    public float resetTime;
    public float speedThreshold;

    private Vector3 initialMousePosition;
    private Vector3 currentMousePosition;
    private bool isDragging = false;
    private bool hasAppliedForce = false;
    private Rigidbody rb;
    private float timeSinceLastMove;
    private float currentArrowScale;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        arrow.SetActive(false);
        arrow.transform.localPosition = Vector3.zero;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !hasAppliedForce)
        {
            initialMousePosition = Input.mousePosition;
            isDragging = true;
            arrow.SetActive(true);
            arrow.transform.localPosition = Vector3.zero;
        }

        if (isDragging && Input.GetMouseButton(0))
        {
            currentMousePosition = Input.mousePosition;
            UpdateArrow();
        }

        if (Input.GetMouseButtonUp(0) && isDragging) 
        {
            isDragging = false;
            arrow.SetActive(false);
            ApplyForce();
            hasAppliedForce = true; 
        }

        if (hasAppliedForce)
        {
            CheckBallMovement();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetLevel();
        }
    }

    void UpdateArrow()
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

        
        arrow.transform.localScale = new Vector3(1.2f, 0.5f, currentArrowScale);
        arrow.transform.rotation = Quaternion.LookRotation(direction);
    }

    void ApplyForce()
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

        direction.Normalize();


        float forceMagnitude = Mathf.Clamp(currentArrowScale, 0, maxForce);

        rb.AddForce(direction * forceMagnitude, ForceMode.Impulse);
    }

    void CheckBallMovement()
    {
        if (rb.linearVelocity.magnitude < speedThreshold)
        {
            timeSinceLastMove += Time.deltaTime;
            if (timeSinceLastMove >= resetTime)
            {
                ResetLevel();
            }
        }
        else
        {
            timeSinceLastMove = 0f;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RetryTrigger"))
        {
            Invoke("ResetLevel", 1.5f);
        }
    }

    void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
