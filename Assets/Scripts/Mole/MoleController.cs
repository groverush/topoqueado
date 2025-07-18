using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoleController : MonoBehaviour
{
    // === Scripts ===
    [HideInInspector] public HoleNavigation holeNavigationScript;

    // === Input ===
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction popOutAction;

    // === Movement ===
    [SerializeField] private float movementCooldown;
    private float movementTimer = 0f;
    private bool canMove = true;
    private Vector2 movementInput;

    // === Pop out ===
    [SerializeField] private float popOutCooldown;
    private float popOutTimer = 0f;
    private bool canPopOut = true;
    private Vector3 originalPosition;
    private bool wasPopOutPressedLastFrame = false;

    // === Pop state ===
    public enum PopStates { Hidden, Visible };
    private PopStates currentPopState = PopStates.Hidden;
    
    // === Power up ===
    [Header("Power Up Manager")]
    [SerializeField] private MolePowerUpManager molePowerUpManager;

    // === Events ===
    public event Action OnMoleHit;

    // === Properties ===
    public PopStates CurrentPopState => currentPopState;
    public MolePowerUpManager MolePowerUpManager => molePowerUpManager;

    private void Awake ()
    {
        holeNavigationScript = GetComponent<HoleNavigation>();
        originalPosition = transform.position;

        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["MoveMole"];
        popOutAction = playerInput.actions["PopOut"];
    }

    void Update()
    {
        // Movement with cooldown
        movementTimer += Time.deltaTime;
        movementInput = moveAction.ReadValue<Vector2>();

        if (canMove && movementTimer >= movementCooldown && movementInput != Vector2.zero)
        {
            holeNavigationScript.SelectHole(movementInput, Vector3.left, Vector3.back);
            movementTimer = 0;
        }

        // Pop in / out management
        if (canPopOut && popOutAction.ReadValue<float>() > 0 && !wasPopOutPressedLastFrame)
        {
            PopOut();
            molePowerUpManager.TryShowClone(holeNavigationScript.CurrentHole.transform.position);
        }
        else if (popOutAction.ReadValue<float>() == 0 && wasPopOutPressedLastFrame)
        {
            PopIn();
            molePowerUpManager.HideClone();
        }

        wasPopOutPressedLastFrame = popOutAction.ReadValue<float>() > 0;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Hammer"))
        {
            OnMoleHit?.Invoke(); // Notify CollisionManager

            PopIn();

            molePowerUpManager.HideClone();

            // Código provisional (se debe optimizar)
            canPopOut = false;
            popOutTimer = 0f;
            StartCoroutine(PopOutCooldownRoutine());
        }
    }

    private void PopIn()
    {
        transform.position = originalPosition;
        currentPopState = PopStates.Hidden;
        canMove = true;

        Debug.Log("Mole in position" + transform.position);
    }

    private void PopOut()
    {
        transform.position = holeNavigationScript.CurrentHole.transform.position + new Vector3 (0, 0.25f, -0.1f);
        currentPopState = PopStates.Visible;
        canMove = false;
    }

    private IEnumerator PopOutCooldownRoutine ()
    {
        while (popOutTimer < popOutCooldown)
        {
            popOutTimer += Time.deltaTime;
            yield return null;
        }
        canPopOut = true;
    }
}