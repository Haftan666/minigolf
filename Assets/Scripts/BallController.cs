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
    private float timeMovingAway = 0f;
    private bool retryTriggerHit = false;

    public ArrowController arrowController;
    public LastAttemptArrowController lastAttemptArrowController;
    public GameManager gameManager;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        lastAttemptArrowController.HideLastAttemptArrow();
    }

    void Update()
    {
        if (gameManager.getGameEnded())
        {
            return;
        }
        if (Input.GetMouseButtonDown(0) && !gameManager.GetHasAppliedForce())
        {
            initialMousePosition = Input.mousePosition;
            isDragging = true;
            retryTriggerHit = false;
            arrowController.ShowArrow();
            lastAttemptArrowController.ShowLastAttemptArrow();
            if(gameManager.GetAttempts() == 0)
            {

               lastAttemptArrowController.HideLastAttemptArrow();
            }
            
            Debug.Log("Mouse button down, arrow shown");
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
            lastAttemptArrowController.SaveLastArrowState(
                arrowController.transform.localPosition,
                arrowController.transform.localScale,
                arrowController.GetComponent<Renderer>().material.color,
                arrowController.transform.rotation
            );
            lastAttemptArrowController.HideLastAttemptArrow();
            ApplyForce();
            gameManager.SetHasAppliedForce(true);
            Debug.Log("Mouse button up, force applied");
        }

        if (gameManager.GetHasAppliedForce() && !retryTriggerHit)
        {
            CheckBallMovement();
            CheckDirectionToTrigger();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            gameManager.ResetLevel(false);
            Debug.Log("Reset level key pressed");
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
                gameManager.ResetLevel(false);
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

            if (dotProduct < 0)
            {
                timeMovingAway += Time.deltaTime;
                if (timeMovingAway >= gameManager.resetTime)
                {
                    gameManager.ResetLevel(false);
                }
            }
            else
            {
                timeMovingAway = 0f;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RetryTrigger"))
        {
            gameManager.InvokeResetLevel(gameManager.resetTime, false);
            retryTriggerHit = true;
            Debug.Log("RetryTrigger hit");
        }
        else if (other.CompareTag("LevelPassedTrigger"))
        {
            gameManager.NextLevel();
            lastAttemptArrowController.HideLastAttemptArrow();
            Debug.Log("LevelPassedTrigger hit");
        }
    }

    public void ResetTimeSinceLastMove()
    {
        timeSinceLastMove = 0f;
    }
}
