
using UnityEngine;
using UnityEngine.InputSystem;

public class BulletScript : MonoBehaviour
{
    private Rigidbody rb;
    private InputAction myMoveAction;
    private InputAction LTurnAction;
    private InputAction RTurnAction;

    private bool LTurnPressed;
    private bool LTurnHeld;
    private bool LTurnReleased;

    private bool RTurnPressed;
    private bool RTurnHeld;
    private bool RTurnReleased;


    private float bulletSpeed = 40f;
    private float horiTurnSpeed = 150f;
    private float vertTurnSpeed = 100f;
    private float triggerTurnMultiply = 1.8f;

    [Header("Tilt Limits")]
    public float minTilt = -45f;
    public float maxTilt = 45f;

    public float currentYaw;
    private float currentTilt;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        myMoveAction = InputSystem.actions.FindAction("Move");
        LTurnAction = InputSystem.actions.FindAction("LeftTurn");
        RTurnAction = InputSystem.actions.FindAction("RightTurn");
        

    Vector3 angles = transform.eulerAngles;
        currentYaw = angles.y;
        currentTilt = angles.x;

        if (currentTilt > 180f)
            currentTilt -= 360f;
    }

    void Update()
    {
        Vector2 moveInput = myMoveAction.ReadValue<Vector2>();
        LTurnHeld = LTurnAction.IsPressed();
        RTurnHeld = RTurnAction.IsPressed();

        // horizontal turn w/ triggers to turn harder
        if (LTurnHeld)
        {
            currentYaw -= horiTurnSpeed * triggerTurnMultiply * Time.deltaTime;
        }

        if (RTurnHeld)
        {
            currentYaw -= -horiTurnSpeed * triggerTurnMultiply * Time.deltaTime;
        }

        if(!LTurnHeld && !RTurnHeld)
        {
            currentYaw += moveInput.x * horiTurnSpeed * Time.deltaTime;
        }

        // vertical turn
        currentTilt -= moveInput.y * vertTurnSpeed * Time.deltaTime;

        // Keep tilt within limits
        currentTilt = Mathf.Clamp(currentTilt, minTilt, maxTilt);

        // Apply the rotation from tracked angles
        transform.rotation = Quaternion.Euler(
            currentTilt,
            currentYaw,
            0f
        );

        // Always move forward
        transform.position += transform.forward * bulletSpeed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Target"))
        {
            Destroy(other.gameObject);

        }

        if (other.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }
}
