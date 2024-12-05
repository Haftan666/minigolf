using UnityEngine;

public class BallController : MonoBehaviour
{
    public float maxForce;
    public float speedThreshold;

    private Vector3 initialMousePosition;
    private Vector3 currentMousePosition;
    private bool isDragging = false;
    private Rigidbody rb;
    private float timeSinceLastMove;
    private float timeMovingAway = 0f; // Czas, przez który piłka się oddala

    public ArrowController arrowController;
    public GameManager gameManager;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !gameManager.GetHasAppliedForce())
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
            //Debug.Log("force applied");
            gameManager.SetHasAppliedForce(true);
        }

        if (gameManager.GetHasAppliedForce())
        {
            CheckBallMovement();
            CheckDirectionToTrigger();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            gameManager.ResetLevel();
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
            if (timeSinceLastMove >= gameManager.resetTime)
            {
                //Debug.Log("Resetting level");
                gameManager.ResetLevel();
            }
        }
        else
        {
            timeSinceLastMove = 0f;
        }
    }


    void CheckDirectionToTrigger()
    {
        GameObject levelPassedTrigger = gameManager.GetCurrentLevelPassedTrigger();
        if (levelPassedTrigger != null)
        {
            Vector3 directionToTrigger = levelPassedTrigger.transform.position - transform.position;
            float dotProduct = Vector3.Dot(rb.linearVelocity.normalized, directionToTrigger.normalized);

            if (dotProduct < 0) // Piłka oddala się od obiektu
            {
                timeMovingAway += Time.deltaTime;
                if (timeMovingAway >= gameManager.resetTime)
                {
                    gameManager.ResetLevel();
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
            gameManager.InvokeResetLevel(gameManager.resetTime);
        }
        else if (other.CompareTag("LevelPassedTrigger"))
        {
            gameManager.NextLevel();
        }
    }

    public void ResetTimeSinceLastMove()
    {
        timeSinceLastMove = 0f;
    }
}
