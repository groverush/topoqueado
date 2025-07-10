using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class HammerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveStep = 1.5f; // Distance the hammer moves per input step
    [SerializeField] private float moveDuration = 0.1f; // Duration of each movement
    [SerializeField] private float repeatDelay = 0.2f; // Delay between repeated movements
    [SerializeField] private float moveCooldown = 0.1f; // Delay before fade out

    [Header("Hammer Settings")]
    [SerializeField] private Transform hammerHead;
    [SerializeField] private GameObject hammerVisual; 
    [SerializeField] private Vector3 hammerRestPosition = new Vector3(0f, 2f, 0f); // Ajusta valores por defecto
    [SerializeField] private Vector3 hitOffset = new Vector3(0f, 0f, 0f); // ← Ajustable desde el editor
    [SerializeField] private float initialHammerAngle = -90f; // Starting rotation angle
    [SerializeField] private float hitAngle = -180f; // Rotation angle when hitting
    [SerializeField] private float hitDownDuration = 0.08f; // Time to swing down
    [SerializeField] private float hitPause = 0.05f; // Pause after hitting
    [SerializeField] private float hitCooldown = 0.1f; // Delay before fade out

    private bool isHitting = false;
    private float moveTimer = 0f;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private HoleNavigation holeNavigation;

    private void Awake ()
    {
        hammerHead.position = hammerRestPosition;
        hammerHead.localRotation = Quaternion.Euler(initialHammerAngle, 0f, 0f);

        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["MoveHammer"];
        holeNavigation = GetComponent<HoleNavigation>();
    }


    private void Update ()
    {
        moveTimer += Time.deltaTime;
        Vector2 movementHammer = moveAction.ReadValue<Vector2>();

        if (movementHammer != Vector2.zero && moveTimer >= moveCooldown)
        {
            holeNavigation.SelectHole(movementHammer, Vector3.right, Vector3.forward);
            moveTimer = 0f;
        }
    }

    // Public method to trigger hammer hit
    public void OnHit ()
    {
        if (!isHitting)
            StartCoroutine(HitAnimation());
    }

    // Full hit animation flow, including fade in/out
    private IEnumerator HitAnimation ()
    {
        isHitting = true;

        Vector3 holePos = holeNavigation.GetCurrentHole().transform.position;
        Vector3 startPos = hammerRestPosition;
        Vector3 targetPos = new Vector3(holePos.x, startPos.y, holePos.z) + hitOffset;

        // 1. Moverse hacia el agujero
        float moveToTime = 0.1f;
        float elapsed = 0f;
        while (elapsed < moveToTime)
        {
            hammerHead.position = Vector3.Lerp(startPos, targetPos, elapsed / moveToTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        hammerHead.position = targetPos;

        // 2. Golpear (rotar)
        Quaternion startRot = Quaternion.Euler(initialHammerAngle, 0f, 0f);
        Quaternion downRot = Quaternion.Euler(hitAngle, 0f, 0f);
        elapsed = 0f;
        while (elapsed < hitDownDuration)
        {
            hammerHead.localRotation = Quaternion.Slerp(startRot, downRot, elapsed / hitDownDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        hammerHead.localRotation = downRot;

        yield return new WaitForSeconds(hitPause);

        // 3. Volver rotación
        elapsed = 0f;
        while (elapsed < hitDownDuration)
        {
            hammerHead.localRotation = Quaternion.Slerp(downRot, startRot, elapsed / hitDownDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        hammerHead.localRotation = startRot;

        // 4. Volver a la posición base
        elapsed = 0f;
        while (elapsed < moveToTime)
        {
            hammerHead.position = Vector3.Lerp(targetPos, startPos, elapsed / moveToTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        hammerHead.position = startPos;

        isHitting = false;
    }


}