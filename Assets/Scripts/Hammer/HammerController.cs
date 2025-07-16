using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class HammerController : MonoBehaviour
{
    [SerializeField] private HammerPowerUpManager powerUpManager;
    [SerializeField] private Transform hammerBase;
    [SerializeField] private float moveCooldown = 0.1f;
    [SerializeField] private Vector3 hammerRestPosition = new Vector3(0f, 2f, 0f);
    [SerializeField] private float initialHammerAngle = -90f;
    [SerializeField] private float hitAngle = -180f;
    [SerializeField] private float hitDownDuration = 0.08f;
    [SerializeField] private float hitPause = 0.05f;
    [SerializeField] private Vector3 hitOffset = Vector3.zero;

    public Transform HammerBase => hammerBase;
    public Vector3 HammerRestPosition => hammerRestPosition;
    public float InitialHammerAngle => initialHammerAngle;
    public Action OnHammerHitAttempt;
    public HammerPowerUpManager PowerUpManager => powerUpManager;

    private HoleNavigation holeNavigation;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction hitAction;
    private float moveTimer;
    private bool isHitting = false;

    private void Awake ()
    {
        holeNavigation = GetComponent<HoleNavigation>();
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["MoveHammer"];
        hitAction = playerInput.actions["Hit"];
        hitAction.performed += ctx => OnHit();

        hammerBase.position = hammerRestPosition;
        hammerBase.localRotation = Quaternion.Euler(initialHammerAngle, 0f, 0f);
    }

    private void Update ()
    {
        moveTimer += Time.deltaTime;
        Vector2 movement = moveAction.ReadValue<Vector2>();
        if (movement != Vector2.zero && moveTimer >= moveCooldown)
        {
            holeNavigation.SelectHole(movement, Vector3.right, Vector3.forward);
            moveTimer = 0f;
        }
    }

    private void OnEnable () => hitAction.Enable();
    private void OnDisable () => hitAction.Disable();

    public void OnHit ()
    {
        if (!isHitting)
        {
            OnHammerHitAttempt?.Invoke();
            StartCoroutine(HitSequence());
        }
    }

    private IEnumerator HitSequence ()
    {
        isHitting = true;

        Coroutine hammerRoutine = StartCoroutine(HitAnimation(holeNavigation.CurrentHole));
        Coroutine cloneRoutine = null;

        if (powerUpManager.IsDoubleHitActive && powerUpManager.HasValidHammerClone())
        {
            powerUpManager.ActivateCloneForHit();

            GameObject randomHole = powerUpManager.GetSecondHole(holeNavigation, holeNavigation.CurrentHole);

            if (randomHole != null)
            {
                cloneRoutine = StartCoroutine(
                    powerUpManager.HammerCloneInstance.PlayHitAnimation(
                        randomHole.transform.position,
                        initialHammerAngle,
                        hitAngle,
                        hitDownDuration,
                        hitPause,
                        hitOffset
                    )
                );
            }
        }

        yield return hammerRoutine;
        if (cloneRoutine != null)
            yield return cloneRoutine;

        if (powerUpManager.IsDoubleHitActive && powerUpManager.HasValidHammerClone())
        {
            powerUpManager.HammerCloneInstance.gameObject.SetActive(false);
        }

        isHitting = false;
    }

    private IEnumerator HitAnimation ( GameObject hole )
    {
        Vector3 startPos = hammerRestPosition;
        Vector3 targetPos = new Vector3(hole.transform.position.x, startPos.y, hole.transform.position.z) + hitOffset;
        float moveTime = 0.1f;
        float elapsed = 0f;

        while (elapsed < moveTime)
        {
            hammerBase.position = Vector3.Lerp(startPos, targetPos, elapsed / moveTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        hammerBase.position = targetPos;
        yield return RotateHammer(initialHammerAngle, hitAngle);
        yield return new WaitForSeconds(hitPause);
        yield return RotateHammer(hitAngle, initialHammerAngle);

        elapsed = 0f;
        while (elapsed < moveTime)
        {
            hammerBase.position = Vector3.Lerp(targetPos, startPos, elapsed / moveTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        hammerBase.position = startPos;
    }

    private IEnumerator RotateHammer ( float fromAngle, float toAngle )
    {
        Quaternion startRot = Quaternion.Euler(fromAngle, 0f, 0f);
        Quaternion endRot = Quaternion.Euler(toAngle, 0f, 0f);
        float elapsed = 0f;

        while (elapsed < hitDownDuration)
        {
            hammerBase.localRotation = Quaternion.Slerp(startRot, endRot, elapsed / hitDownDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        hammerBase.localRotation = endRot;
    }
}
