using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoleController : MonoBehaviour
{
    [Header("Power Up Manager")]
    [SerializeField] private MolePowerUpManager molePowerUpManager;

    private HoleNavigation holeNavigationScript;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction popOutAction;

    [SerializeField] private float movementCooldown;
    private float movementTimer = 0f;
    private bool canMove = true;
    private Vector2 movementInput;

    [SerializeField] private float popOutCooldown;
    private float popOutTimer = 0f;
    private bool canPopOut = true;
    private Vector3 originalPosition;

    public enum PopStates { Hidden, Visible };
    private PopStates currentPopState = PopStates.Hidden;

    public event Action OnMoleHit;
    public PopStates CurrentPopState => currentPopState;
    public MolePowerUpManager MolePowerUpManager => molePowerUpManager;

    private void Awake ()
    {
        holeNavigationScript = GetComponent<HoleNavigation>();
        originalPosition = transform.position;

        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["MoveMole"];
        popOutAction = playerInput.actions["PopOut"];
        popOutAction.canceled += OnPopOutReleased;

        if (molePowerUpManager == null)
        {
            Debug.LogWarning("MolePowerUpManager no está asignado en el inspector.");
        }
    }

    private void OnDestroy ()
    {
        popOutAction.canceled -= OnPopOutReleased;
    }

    private void Update ()
    {
        movementTimer += Time.deltaTime;
        movementInput = moveAction.ReadValue<Vector2>();

        if (canMove && movementTimer >= movementCooldown && movementInput != Vector2.zero)
        {
            holeNavigationScript.SelectHole(movementInput, Vector3.left, Vector3.back);
            movementTimer = 0;
        }

        if (canPopOut && popOutAction.ReadValue<float>() > 0)
        {
            PopOut();
        }
        else if (currentPopState == PopStates.Visible)
        {
            PopIn();
        }
    }

    private void OnCollisionEnter ( Collision collision )
    {
        if (collision.gameObject.CompareTag("Hammer"))
        {
            OnMoleHit?.Invoke();
            PopIn();
            CancelPopOutInput();
            canPopOut = false;
            popOutTimer = 0f;
            StartCoroutine(PopOutCooldownRoutine());
        }
    }

    private void PopIn ()
    {
        transform.position = originalPosition;
        currentPopState = PopStates.Hidden;
        canMove = true;
    }

    private void PopOut ()
    {
        transform.position = holeNavigationScript.CurrentHole.transform.position;
        currentPopState = PopStates.Visible;
        canMove = false;
    }

    private void OnPopOutReleased ( InputAction.CallbackContext context )
    {
        canPopOut = false;
        popOutTimer = 0f;
        StartCoroutine(PopOutCooldownRoutine());
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

    private void CancelPopOutInput ()
    {
        popOutAction.Disable();
        popOutAction.Enable();
    }
}
