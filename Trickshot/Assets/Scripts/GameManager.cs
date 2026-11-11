using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    private InputAction shootAction;

    private InputAction myLookAction;

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


    private float myLookSpeed = 50f;
    private float myLookAngle = 0f;

    [SerializeField] private Transform respawnPoint;
    private bool cameraSet = false;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] private Transform camera;
    [SerializeField] private Vector3 cameraOffset;

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
        myLookAction = InputSystem.actions.FindAction("Look");

        currentState = PlayerState.Aim;
    }

    void Update()
    {
        ShootPressed = shootAction.WasPressedThisFrame();

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
        Vector2 LookDirection = myLookAction.ReadValue<Vector2>();

        // Rotate the camera vertically
        myLookAngle += LookDirection.y * myLookSpeed * Time.deltaTime;
        myLookAngle = Mathf.Clamp(myLookAngle, -70f, 70f);

        camera.localRotation = Quaternion.Euler(-myLookAngle, 0f, 0f);

        // Keep the camera at its intended position
        camera.position = transform.position + transform.TransformDirection(cameraOffset);
    }


    void AimState(bool ShootPressed)
    {
        if (!cameraSet)
        {
            // Set camera position and rotation here

            cameraSet = true;
        }

        LookInput();


        if (ShootPressed)
        {
            cameraSet = false;
            currentState = PlayerState.Flight;
            bulletSpawned = true;
        }

    }

    void FlightState(bool ShootPressed)
    {
        
        if (bulletSpawned)
        {
            bulletSpawned = false;

            GameObject newBullet = Instantiate(bulletPrefab, respawnPoint);
            cameraTarget.SetBullet(newBullet.transform);
        }
        
        if (bulletHit)
        {
            currentState = PlayerState.Hit;
        }
    }

    void HitState(bool ShootPressed)
    {
        bulletHit = false;
        currentState = PlayerState.Aim;
    }
}
