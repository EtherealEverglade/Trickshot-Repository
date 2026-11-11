
using UnityEngine;
using UnityEngine.InputSystem;

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

    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");

        LTurnAction = InputSystem.actions.FindAction("LeftTurn");
        RTurnAction = InputSystem.actions.FindAction("RightTurn");
    }


    void LateUpdate()
    {
        if (gameManager.CurrentState == GameManager.PlayerState.Aim)
        {
            transform.position = respawnPoint.position;
        }

        if (gameManager.CurrentState == GameManager.PlayerState.Flight)
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
}