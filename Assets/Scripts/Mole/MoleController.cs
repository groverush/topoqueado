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

    [SerializeField] private Transform moleBase;
    private Coroutine currentAnimationRoutine;

    public enum PopStates { Hidden, Visible };
    private PopStates currentPopState = PopStates.Hidden;

    public event Action OnMoleHit;
    public PopStates CurrentPopState => currentPopState;
    public MolePowerUpManager MolePowerUpManager => molePowerUpManager;

    private bool wasPopOutPressedLastFrame = false;

    private void Awake ()
    {
        holeNavigationScript = GetComponent<HoleNavigation>();
        originalPosition = transform.position;

        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["MoveMole"];
        popOutAction = playerInput.actions["PopOut"];
        popOutAction.canceled += OnPopOutReleased;
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

        float popOutValue = popOutAction.ReadValue<float>();
        bool isPopOutPressed = popOutValue > 0;

        if (canPopOut && isPopOutPressed && !wasPopOutPressedLastFrame)
        {
            PopOut();
            molePowerUpManager.TryShowClone(holeNavigationScript.CurrentHole.transform.position);
        }
        else if (!isPopOutPressed && wasPopOutPressedLastFrame)
        {
            PopIn();
            molePowerUpManager.HideClone();
        }

        wasPopOutPressedLastFrame = isPopOutPressed;
    }

    private void OnCollisionEnter ( Collision collision )
    {
        if (collision.gameObject.CompareTag("Hammer"))
        {
            OnMoleHit?.Invoke();
            PopIn();
            molePowerUpManager.HideClone();
            CancelPopOutInput();
            canPopOut = false;
            popOutTimer = 0f;
            StartCoroutine(PopOutCooldownRoutine());
        }
    }

    private void PopOut ()
    {
        if (currentAnimationRoutine != null) StopCoroutine(currentAnimationRoutine);
        currentAnimationRoutine = StartCoroutine(RotateMole(-20f, 20f, 0.2f));

        transform.position = holeNavigationScript.CurrentHole.transform.position;
        currentPopState = PopStates.Visible;
        canMove = false;
    }

    private void PopIn ()
    {
        if (currentAnimationRoutine != null) StopCoroutine(currentAnimationRoutine);
        currentAnimationRoutine = StartCoroutine(RotateMole(20f, -20f, 0.2f));

        transform.position = originalPosition;
        currentPopState = PopStates.Hidden;
        canMove = true;
    }

    private IEnumerator RotateMole ( float fromAngle, float toAngle, float duration )
    {
        Quaternion startRot = Quaternion.Euler(fromAngle, 0f, 0f);
        Quaternion endRot = Quaternion.Euler(toAngle, 0f, 0f);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            moleBase.localRotation = Quaternion.Slerp(startRot, endRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        moleBase.localRotation = endRot;
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
