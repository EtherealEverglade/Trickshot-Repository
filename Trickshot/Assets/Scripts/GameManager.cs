using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    private InputAction shootAction;

    private InputAction myMoveAction;

    private bool LTurnPressed;
    private bool LTurnHeld;
    private bool LTurnReleased;

    private bool RTurnPressed;
    private bool RTurnHeld;
    private bool RTurnReleased;

    private bool DashHeld;

    private bool ShootPressed;


    private bool bulletSpawned = false;
    public bool bulletHit = false;


    [SerializeField] private Transform respawnPoint;
    public bool cameraSet = false;
    [SerializeField] GameObject bulletPrefab;
    //[SerializeField] private Transform camera;
    [SerializeField] private Vector3 cameraOffset;

    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private CinemachineThirdPersonFollow thirdPersonFollow;

    private Quaternion startingSpawnRotation;

    private float aimYaw;
    private float aimTilt;

    [SerializeField] private float aimTurnSpeed = 150f;
    [SerializeField] private float aimTiltSpeed = 100f;
    [SerializeField] private float minTilt = -45f;
    [SerializeField] private float maxTilt = 45f;

    [SerializeField] private float maxFallTime = 3f;
    [SerializeField] private float stopSpeed = 0.2f;

    private float fallTimer;
    private Rigidbody currentBulletRb;
    private bool resultsReady;

    [SerializeField] private GameObject resultsPanel;
    [SerializeField] private GameObject crosshair;

    public GameObject SpeedEffect;

    [SerializeField] private GameObject continueButton;
    private bool blockShootInput;

    public enum PlayerState
    {
        Aim,
        Flight,
        Hit
    }
    public PlayerState currentState;
    public PlayerState CurrentState => currentState;

    [SerializeField] private CameraTarget cameraTarget;


    void Start()
    {
        shootAction = InputSystem.actions.FindAction("Shoot");
        myMoveAction = InputSystem.actions.FindAction("Move");

        startingSpawnRotation = respawnPoint.rotation;
        resultsPanel.SetActive(false);
        crosshair.SetActive(true);

        currentState = PlayerState.Aim;
    }

    void Update()
    {
        if (blockShootInput)
        {
            blockShootInput = false;
            ShootPressed = false;
        }
        else
        {
            ShootPressed = shootAction.WasPressedThisFrame();
        }

        HandleState(ShootPressed);
    }


    void HandleState(bool ShootPressed)
    {
        switch (currentState)
        {
            case PlayerState.Aim:
                AimState(ShootPressed);
                break;

            case PlayerState.Flight:
                FlightState(ShootPressed);
                break;

            case PlayerState.Hit:
                HitState(ShootPressed);
                break;
        }
    }


    void LookInput()
    {
        Vector2 MoveDirection = myMoveAction.ReadValue<Vector2>();

        // Horizontal rotation
        aimYaw += MoveDirection.x * aimTurnSpeed * Time.deltaTime;

        // Vertical rotation
        aimTilt -= MoveDirection.y * aimTiltSpeed * Time.deltaTime;
        aimTilt = Mathf.Clamp(aimTilt, minTilt, maxTilt);

        // Apply rotation relative to the original spawn rotation
        respawnPoint.rotation = startingSpawnRotation *
            Quaternion.Euler(aimTilt, aimYaw, 0f);
    }


    void AimState(bool ShootPressed)
    {
        if (!cameraSet)
        {
            crosshair.SetActive(true);
            aimYaw = 0f;
            aimTilt = 0f;

            respawnPoint.rotation = startingSpawnRotation;

            thirdPersonFollow.Damping = new Vector3(0f, 0f, 0f);

            scoreManager.ScoreRunReset();

            cameraSet = true;
        }

        LookInput();

        if (ShootPressed)
        {
            cameraSet = false;

            thirdPersonFollow.Damping = new Vector3(0f, 0f, 1f);

            crosshair.SetActive(false);
            currentState = PlayerState.Flight;
            bulletSpawned = true;
        }
    }

    void FlightState(bool ShootPressed)
    {
        
        if (bulletSpawned)
        {
            bulletSpawned = false;

            SpeedEffect.SetActive(true);

            GameObject newBullet = Instantiate(bulletPrefab, respawnPoint.position, respawnPoint.rotation);
            
            currentBulletRb = newBullet.GetComponent<Rigidbody>();
            cameraTarget.SetBullet(newBullet.transform);
        }
        
        if (bulletHit)
        {
            fallTimer = 0f;
            resultsReady = false;
            SpeedEffect.SetActive(false);

            cameraTarget.BeginHitFollow();

            scoreManager.ScoreTotalText();

            currentState = PlayerState.Hit;
        }
    }

    void HitState(bool ShootPressed)
    {

        fallTimer += Time.deltaTime;

        bool bulletStopped = currentBulletRb.linearVelocity.sqrMagnitude
            < stopSpeed * stopSpeed;

        if (fallTimer >= maxFallTime || bulletStopped)
        {
            if (!resultsReady)
            {
                resultsReady = true;
                ShowResults();
            }
        }
    }

    void ShowResults()
    {
        resultsPanel.SetActive(true);

        EventSystem.current.SetSelectedGameObject(continueButton);
    }

    public void Continue()
    {
        blockShootInput = true;

        resultsPanel.SetActive(false);

        if (currentBulletRb != null)
        {
            Destroy(currentBulletRb.gameObject);
            currentBulletRb = null;
        }

        bulletHit = false;
        cameraSet = false;
        resultsReady = false;

        currentState = PlayerState.Aim;
    }
}
