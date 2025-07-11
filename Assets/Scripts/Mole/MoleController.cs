using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoleController : MonoBehaviour
{
    // === Scripts ===
    private HoleNavigation holeNavigationScript;

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

    // === Pop in / out transition ===
    // [SerializeField] private float popOutHeight = 0.5f;
    // [SerializeField] private float transitionDuration = 0.2f;
    // private Coroutine popRoutine;

    void Awake()
    {
        holeNavigationScript = GetComponent<HoleNavigation>();

        originalPosition = transform.position;

        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["MoveMole"];
        popOutAction = playerInput.actions["PopOut"];

        popOutAction.canceled += OnPopOutReleased;
    }

    void OnDestroy()
    {
        popOutAction.canceled -= OnPopOutReleased;
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
        if (canPopOut && popOutAction.ReadValue<float>() > 0)
        {
            PopOut();
        }
        else
        {
            PopIn();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Hammer"))
        {
            Debug.Log("Ouch!");
            PopIn();

            // Código provisional (se debe optimizar)
            canPopOut = false;
            popOutTimer = 0f;
            StartCoroutine(PopOutCooldownRoutine());
        }
    }

    private void PopIn()
    {
        transform.position = originalPosition;
        canMove = true;

        // if (popRoutine != null) StopCoroutine(popRoutine);

        // popRoutine = StartCoroutine(MoveToPosition(originalPosition));
        // canMove = true;
    }

    private void PopOut()
    {
        transform.position = holeNavigationScript.CurrentHole.transform.position;
        canMove = false;

        // if (popRoutine != null) StopCoroutine(popRoutine);

        // Vector3 targetPosition = holeNavigationScript.CurrentHole.transform.position + Vector3.up * popOutHeight;
        // popRoutine = StartCoroutine(MoveToPosition(targetPosition));

        // canMove = false;
    }

    private void OnPopOutReleased(InputAction.CallbackContext context)
    {
        // When the player hides, the cooldown begins
        canPopOut = false;
        popOutTimer = 0f;
        StartCoroutine(PopOutCooldownRoutine());
    }

    private IEnumerator PopOutCooldownRoutine()
    {
        while (popOutTimer < popOutCooldown)
        {
            popOutTimer += Time.deltaTime;
            yield return null;
        }

        canPopOut = true;
    }

    // private IEnumerator MoveToPosition(Vector3 targetPosition)
    // {
    //     Vector3 start = transform.position;
    //     float elapsed = 0f;
        
    //     while (elapsed < transitionDuration)
    //     {
    //         transform.position = Vector3.Lerp(start, targetPosition, elapsed / transitionDuration);
    //         elapsed += Time.deltaTime;
    //         yield return null;
    //     }

    //     transform.position = targetPosition;
    // }
}