using UnityEngine;

public class BallController : MonoBehaviour
{
    public float maxForce;
    public float speedThreshold;
    public GameObject levelPassedTrigger;

    private Vector3 initialMousePosition;
    private Vector3 currentMousePosition;
    private bool isDragging = false;
    private bool hasAppliedForce = false;
    private Rigidbody rb;
    private float timeSinceLastMove;
    private float timeMovingAway = 0f; // Czas, przez który piłka się oddala

    public ArrowController arrowController;
    public GameManager levelManager;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !hasAppliedForce)
        {
            initialMousePosition = Input.mousePosition;
            isDragging = true;
            arrowController.ShowArrow();
        }

        if (isDragging && Input.GetMouseButton(0))
        {
            currentMousePosition = Input.mousePosition;
            arrowController.UpdateArrow(initialMousePosition, currentMousePosition, maxForce);
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            arrowController.HideArrow();
            ApplyForce();
            hasAppliedForce = true;
        }

        if (hasAppliedForce)
        {
            CheckBallMovement();
            CheckDirectionToTrigger();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            levelManager.ResetLevel();
        }
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

        float forceMagnitude = Mathf.Clamp(arrowController.GetCurrentArrowScale(), 0, maxForce);

        rb.AddForce(direction * forceMagnitude, ForceMode.Impulse);
    }

    void CheckBallMovement()
    {
        if (rb.linearVelocity.magnitude < speedThreshold)
        {
            timeSinceLastMove += Time.deltaTime;
            if (timeSinceLastMove >= levelManager.resetTime)
            {
                levelManager.ResetLevel();
            }
        }
        else
        {
            timeSinceLastMove = 0f;
        }
    }

    void CheckDirectionToTrigger()
    {
        if (levelPassedTrigger != null)
        {
            Vector3 directionToTrigger = levelPassedTrigger.transform.position - transform.position;
            float dotProduct = Vector3.Dot(rb.linearVelocity.normalized, directionToTrigger.normalized);

            if (dotProduct < 0) // Piłka oddala się od obiektu
            {
                timeMovingAway += Time.deltaTime;
                if (timeMovingAway >= 1.5f)
                {
                    levelManager.ResetLevel();
                }
            }
            else
            {
                timeMovingAway = 0f; // Zresetuj licznik, jeśli piłka zmierza w kierunku obiektu
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RetryTrigger"))
        {
            levelManager.InvokeResetLevel(1.5f);
        }
        else if (other.CompareTag("LevelPassedTrigger"))
        {
            levelManager.InvokeResetLevel(1.5f);
        }
    }
}
