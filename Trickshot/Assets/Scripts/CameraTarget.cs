using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class CameraTarget : MonoBehaviour
{
    public Transform bullet;

    [Header("Camera Shift")]
    public float maxShift = 2f;
    public float shiftSpeed = 3f;

    public float maxTilt = 2f;
    public float tiltSpeed = 3f;

    private InputAction moveAction;
    private float currentShift;
    private float currentTilt;

    private InputAction LTurnAction;
    private InputAction RTurnAction;

    private bool LTurnHeld;
    private bool RTurnHeld;

    [Header("Trigger Camera Movement")]
    public float triggerShift = 5f;
    public float triggerTilt = 6f;
    public float triggerSpeed = 6f;

    private float targetShift;
    private float targetTilt;
    private float currentSpeed;

    [SerializeField] private GameManager gameManager;
    [SerializeField] private Transform respawnPoint;

    private Vector3 hitCameraOffset;
    private Quaternion hitCameraRotation;
    private bool hitFollowStarted = false;
    private Vector3 frozenCameraPosition;

    [Header("Hit Camera Zoom")]
    [SerializeField] private CinemachineThirdPersonFollow thirdPersonFollow;

    [SerializeField] private float hitZoomOut = 10f;
    [SerializeField] private float zoomSpeed = 2f;

    private float originalCameraDistance;
    private float targetCameraDistance;

    [Header("Hit Camera Position")]
    [SerializeField] private float hitCameraBackDistance = 12f;
    [SerializeField] private float hitCameraMoveSpeed = 3f;

    private Vector3 hitCameraTargetPosition;
    private Vector3 hitHorizontalDirection;

    [SerializeField] private ScoreManager scoreManager;


    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");

        LTurnAction = InputSystem.actions.FindAction("LeftTurn");
        RTurnAction = InputSystem.actions.FindAction("RightTurn");

        originalCameraDistance = thirdPersonFollow.CameraDistance;
        targetCameraDistance = originalCameraDistance;
    }


    void LateUpdate()
    {

        if (gameManager.CurrentState == GameManager.PlayerState.Flight && bullet != null)
        {

            LTurnHeld = LTurnAction.IsPressed();
            RTurnHeld = RTurnAction.IsPressed();

            transform.position = bullet.position;

            // Copy the bullet's direction, but remove roll
            Vector3 direction = bullet.forward;
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);


            if (LTurnHeld)
            {
                TriggerCameraOffset(-1.5f);
            }

            if (RTurnHeld)
            {
                TriggerCameraOffset(1.5f);
            }

            if (!LTurnHeld && !RTurnHeld)
            {
                HoriCameraOffset();
            }

            VertCameraOffset();
        }

        else if (gameManager.CurrentState == GameManager.PlayerState.Aim)
        {
            transform.position = respawnPoint.position;
            transform.rotation = Quaternion.LookRotation(respawnPoint.forward, Vector3.up);
        }

        else if (gameManager.CurrentState == GameManager.PlayerState.Hit)
        {
            if (hitFollowStarted && bullet != null)
            {
                // Move camera toward its backed-up position
                transform.position = Vector3.Lerp(
                    transform.position,
                    hitCameraTargetPosition,
                    hitCameraMoveSpeed * Time.deltaTime
                );

                // Look directly toward the bullet, including vertical movement
                Vector3 direction = bullet.position - transform.position;

                if (direction.sqrMagnitude > 0.001f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(
                        direction,
                        Vector3.up
                    );

                    transform.rotation = Quaternion.Slerp(
                        transform.rotation,
                        targetRotation,
                        5f * Time.deltaTime
                    );
                }
            }
        }

        if (thirdPersonFollow != null)
        {
            thirdPersonFollow.CameraDistance = Mathf.Lerp(
                thirdPersonFollow.CameraDistance,
                targetCameraDistance,
                zoomSpeed * Time.deltaTime
            );
        }

    }

    void HoriCameraOffset()
    {
        // Read horizontal joystick input
        float horizontalInput = moveAction.ReadValue<Vector2>().x;

        // Shift opposite to the bullet's turning direction
        float targetShift = horizontalInput * maxShift;

        currentShift = Mathf.Lerp(
            currentShift,
            targetShift,
            shiftSpeed * Time.deltaTime
        );

        // Apply the offset along the camera target's local X axis
        transform.position += transform.right * currentShift;
    }

    void VertCameraOffset()
    {
        // Read vertical joystick input
        float verticalInput = moveAction.ReadValue<Vector2>().y;

        // Tilr opposite to the bullet's turning direction
        float targetTilt = verticalInput * maxTilt;

        currentTilt = Mathf.Lerp(
            currentTilt,
            targetTilt,
            tiltSpeed * Time.deltaTime
        );

        // Apply the offset along the camera target's local Y axis
        transform.position += transform.up * currentTilt;
    }

    void TriggerCameraOffset(float input)
    {
        // Shift opposite to the bullet's turning direction
        float targetShift = input * maxShift;

        currentShift = Mathf.Lerp(
            currentShift,
            targetShift,
            shiftSpeed * Time.deltaTime
        );

        // Apply the offset along the camera target's local X axis
        transform.position += transform.right * currentShift;
    }

    public void SetBullet(Transform newBullet)
    {
        bullet = newBullet;
    }

    public void BeginHitFollow()
    {
        // Capture the bullet's horizontal direction at impact
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

        Vector3 velocity = bulletRb.linearVelocity;
        velocity.y = 0f;

        if (velocity.sqrMagnitude > 0.001f)
            hitHorizontalDirection = velocity.normalized;
        else
            hitHorizontalDirection = bullet.forward;

        // Move the camera backward from the bullet's impact position
        hitCameraTargetPosition = bullet.position
            - hitHorizontalDirection * hitCameraBackDistance;

        hitCameraTargetPosition.y = transform.position.y;

        targetCameraDistance = originalCameraDistance + hitZoomOut;

        hitFollowStarted = true;
    }
}