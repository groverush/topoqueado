using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoleController : MonoBehaviour
{
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

    [SerializeField] private float popOutHeight = 0.5f;
    [SerializeField] private float transitionDuration = 0.2f;
    private Coroutine popRoutine;

    void Awake ()
    {
        holeNavigationScript = GetComponent<HoleNavigation>();
        originalPosition = transform.position;
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["MoveMole"];
        popOutAction = playerInput.actions["PopOut"];
        popOutAction.canceled += OnPopOutReleased;
    }

    void OnDestroy ()
    {
        popOutAction.canceled -= OnPopOutReleased;
    }

    void Update ()
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
        else
        {
            PopIn();
        }
    }

    void OnCollisionEnter ( Collision collision )
    {
        if (collision.gameObject.CompareTag("Hammer"))
        {
            OnMoleHit?.Invoke();
            PopIn();
            canPopOut = false;
            popOutTimer = 0f;
            StartCoroutine(PopOutCooldownRoutine());
        }
    }

    private void PopIn ()
    {
        Vector3 targetPosition = originalPosition;
        ChangePopState(PopStates.Hidden, targetPosition);
    }

    private void PopOut ()
    {
        Vector3 targetPosition = holeNavigationScript.CurrentHole.transform.position + Vector3.up * popOutHeight;
        ChangePopState(PopStates.Visible, targetPosition);
    }

    private void ChangePopState ( PopStates newState, Vector3 targetPosition )
    {
        if (currentPopState != newState)
        {
            currentPopState = newState;
            if (popRoutine != null) StopCoroutine(popRoutine);
            popRoutine = StartCoroutine(MoveToPosition(targetPosition));
            canMove = (newState == PopStates.Hidden);
        }
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

    private IEnumerator MoveToPosition ( Vector3 targetPosition )
    {
        Vector3 start = transform.position;
        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            transform.position = Vector3.Lerp(start, targetPosition, elapsed / transitionDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
    }
}
