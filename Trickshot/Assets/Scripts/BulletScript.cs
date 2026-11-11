using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class BulletScript : MonoBehaviour
{
    private Rigidbody rb;
    private InputAction myMoveAction;
    private InputAction LTurnAction;
    private InputAction RTurnAction;
    private InputAction dodgeAction;
    private InputAction dashAction;
    //private InputAction shootAction;

    private bool LTurnPressed;
    private bool LTurnHeld;
    private bool LTurnReleased;

    private bool RTurnPressed;
    private bool RTurnHeld;
    private bool RTurnReleased;

    private bool DashHeld;

    //private bool ShootPressed;

    private float bulletSpeed = 40f;
    private float normalSpeed = 40f;
    private float dashSpeed = 80f;
    private float horiTurnSpeed = 150f;
    private float vertTurnSpeed = 100f;
    private float triggerTurnMultiply = 1.8f;

    [Header("Tilt Limits")]
    public float minTilt = -45f;
    public float maxTilt = 45f;

    public float currentYaw;
    private float currentTilt;



    [Header("Dodge")]
    private float dodgeSpeed = 100f;
    private float dodgeDuration = 0.1f;
    private float dodgeCooldown = 0.5f;

    private bool canDodge = true;

    [SerializeField] private GameManager gameManager;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        myMoveAction = InputSystem.actions.FindAction("Move");
        LTurnAction = InputSystem.actions.FindAction("LeftTurn");
        RTurnAction = InputSystem.actions.FindAction("RightTurn");
        dodgeAction = InputSystem.actions.FindAction("Dodge");
        dashAction = InputSystem.actions.FindAction("Dash");
        //shootAction = InputSystem.actions.FindAction("Shoot");


        Vector3 angles = transform.eulerAngles;
        currentYaw = angles.y;
        currentTilt = angles.x;

        if (currentTilt > 180f)
            currentTilt -= 360f;


        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<GameManager>();
        }
    }

    void Update()
    {
        Vector2 moveInput = myMoveAction.ReadValue<Vector2>();
        LTurnHeld = LTurnAction.IsPressed();
        RTurnHeld = RTurnAction.IsPressed();
        DashHeld = dashAction.IsPressed();
        //ShootPressed = dashAction.IsPressed();


        // horizontal turn w/ triggers to turn harder
        if (LTurnHeld)
        {
            currentYaw -= horiTurnSpeed * triggerTurnMultiply * Time.deltaTime;
        }

        if (RTurnHeld)
        {
            currentYaw -= -horiTurnSpeed * triggerTurnMultiply * Time.deltaTime;
        }

        if (!LTurnHeld && !RTurnHeld)
        {
            currentYaw += moveInput.x * horiTurnSpeed * Time.deltaTime;
        }

        // vertical tilt
        currentTilt -= moveInput.y * vertTurnSpeed * Time.deltaTime;

        // Keep tilt within limits
        currentTilt = Mathf.Clamp(currentTilt, minTilt, maxTilt);

        // Apply the rotation from tracked angles
        transform.rotation = Quaternion.Euler(
            currentTilt,
            currentYaw,
            0f
        );

        if (dodgeAction.WasPressedThisFrame() && canDodge)
        {
            float dodgeDirection = dodgeAction.ReadValue<float>();
            StartCoroutine(Dodge(dodgeDirection));
        }

        if (DashHeld)
        {
            bulletSpeed = dashSpeed;
        }
        if (!DashHeld)
        {
            bulletSpeed = normalSpeed;
        }

        // Always move forward
        transform.position += transform.forward * bulletSpeed * Time.deltaTime;
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Target"))
        {

            Rigidbody otherRb = other.GetComponentInParent<Rigidbody>();

            if (otherRb != null)
            {

                Vector3 impactDirection = transform.forward;
                Vector3 impactPoint = other.ClosestPoint(transform.position);

                otherRb.AddForceAtPosition(
                    impactDirection * 200f,
                    impactPoint,
                    ForceMode.Impulse
                );
            }

        }

        if (other.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
            gameManager.bulletHit = true;
        }
    }

    IEnumerator Dodge(float direction)
    {
        canDodge = false;

        float timer = 0f;

        while (timer < dodgeDuration)
        {
            transform.position += transform.right * direction
                * dodgeSpeed * Time.deltaTime;

            timer += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(dodgeCooldown);

        canDodge = true;
    }

}