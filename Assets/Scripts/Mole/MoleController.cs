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

    // === Pop in/out ===
    [Header("Pop Settings")]
    [SerializeField] private float popOutCooldown;
    [SerializeField] private float popInDelay;
    [SerializeField] private float popAnimationDuration;
    private bool canPopOut = true;
    private bool isProcessingPopIn = false;
    private Vector3 originalPosition;
    private Vector3 popOutOffset = new(0, 0.25f, -0.1f);

    // === Pop state ===
    public enum PopStates { Hidden, Visible };
    private PopStates currentPopState = PopStates.Hidden;

    // === Power up ===
    [Header("Power Up Manager")]
    [SerializeField] private MolePowerUpManager molePowerUpManager;

    // === Coroutines ===
    private Coroutine popOutCoroutine;
    private Coroutine popInCoroutine;

    // === Events ===
    public event Action OnMoleHit;

    // === Properties ===
    public PopStates CurrentPopState => currentPopState;
    public MolePowerUpManager MolePowerUpManager => molePowerUpManager;

    private void Awake()
    {
        holeNavigationScript = GetComponent<HoleNavigation>();
        originalPosition = transform.position;
        Debug.Log("Topo: " + originalPosition);

        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["MoveMole"];
        popOutAction = playerInput.actions["PopOut"];


        popOutAction.performed += ctx => TryPopOut();
        popOutAction.canceled += ctx => TryPopIn();
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
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Hammer"))
        {
            OnMoleHit?.Invoke(); // Notify CollisionManager
            TryPopIn();
        }
    }

    private void TryPopIn()
    {
        if (currentPopState != PopStates.Visible) return;

        PopIn();
        molePowerUpManager.HideClone();
    }

    private void TryPopOut()
    {
        if (!canPopOut || isProcessingPopIn || currentPopState == PopStates.Visible) return;

        PopOut();
        molePowerUpManager.TryShowClone(holeNavigationScript.CurrentHole);
    }

    private void PopIn()
    {
        if (popInCoroutine != null) StopCoroutine(popInCoroutine);

        canPopOut = false;
        isProcessingPopIn = true;
        popInCoroutine = StartCoroutine(PopInDelayRoutine());
    }

    private void PopOut()
    {
        Vector3 newPosition = holeNavigationScript.CurrentHole.transform.position + popOutOffset;
        StartCoroutine(MoveToPosition(transform.position, newPosition, popAnimationDuration));
        currentPopState = PopStates.Visible;
        canMove = false;
    }

    private IEnumerator PopInDelayRoutine()
    {
        yield return new WaitForSeconds(popInDelay);

        StartCoroutine(MoveToPosition(transform.position, originalPosition, popAnimationDuration));
        currentPopState = PopStates.Hidden;
        canMove = true;

        // Apply cooldown to be able to pop out again
        if (popOutCoroutine != null) StopCoroutine(popOutCoroutine);
        popOutCoroutine = StartCoroutine(PopOutCooldownRoutine());

        isProcessingPopIn = false;
        popInCoroutine = null;
    }

    private IEnumerator PopOutCooldownRoutine()
    {
        yield return new WaitForSeconds(popOutCooldown);
        canPopOut = true;
        popOutCoroutine = null;
    }

    // Pop in/out animation
    private IEnumerator MoveToPosition(Vector3 previous, Vector3 newPosition, float duration)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(previous, newPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = newPosition;
    }
}