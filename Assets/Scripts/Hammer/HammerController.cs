using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class HammerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveCooldown = 0.1f;

    [Header("Hammer Settings")]
    [SerializeField] private Transform hammerBase;
    [SerializeField] private Vector3 hammerRestPosition = new Vector3(0f, 2f, 0f);
    [SerializeField] private Vector3 hitOffset = new Vector3(0f, 0f, 0f);
    [SerializeField] private float initialHammerAngle = -90f;
    [SerializeField] private float hitAngle = -180f;
    [SerializeField] private float hitDownDuration = 0.08f;
    [SerializeField] private float hitPause = 0.05f;

    [Header("PowerUp Settings")]
    [SerializeField] private HammerPowerUps hammerPowerUps;
    [SerializeField] private GameObject hammerClonePrefab;
    private HammerCloneController hammerCloneInstance;

    public HammerPowerUps HammerPowerUps => hammerPowerUps;

    private bool isHitting = false;
    private float moveTimer = 0f;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction hitAction;
    private HoleNavigation holeNavigation;

    private Coroutine moleVisionRoutine;
    private int originalCullingMask;

    private void Awake ()
    {
        hammerBase.position = hammerRestPosition;
        hammerBase.localRotation = Quaternion.Euler(initialHammerAngle, 0f, 0f);

        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["MoveHammer"];
        hitAction = playerInput.actions["Hit"];
        holeNavigation = GetComponent<HoleNavigation>();

        hitAction.performed += ctx => OnHit();

        if (hammerPowerUps == null)
            hammerPowerUps = GetComponent<HammerPowerUps>();
        hammerPowerUps.OnDoubleHitEnd += HandleDoubleHitEnd;

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

    private void OnEnable ()
    {
        hitAction.Enable();
    }

    private void OnDisable ()
    {
        hitAction.Disable();
    }

    public void OnHit ()
    {
        if (!isHitting)
            StartCoroutine(HitSequence());
    }

    private IEnumerator HitSequence ()
    {
        if (isHitting) yield break;
        isHitting = true;

        // Preparación de la instancia del clon si aplica
        if (hammerPowerUps != null && hammerPowerUps.IsDoubleHitActive())
        {
            SpawnHammerClone();
        }

        // Preparar corrutinas
        Coroutine hammerRoutine = StartCoroutine(HitAnimation(holeNavigation.CurrentHole));
        Coroutine cloneRoutine = null;

        if (hammerPowerUps != null && hammerPowerUps.IsDoubleHitActive() && hammerCloneInstance != null)
        {
            GameObject randomHole = hammerPowerUps.GetSecondHole(holeNavigation);
            if (randomHole != null)
            {
                hammerCloneInstance.ActivateClone(hammerRestPosition, initialHammerAngle);

                cloneRoutine = StartCoroutine(
                    hammerCloneInstance.PlayHitAnimation(
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

        // Esperar ambas corrutinas si existe el clon
        if (cloneRoutine != null)
        {
            yield return hammerRoutine;
            yield return cloneRoutine;
        }
        else
        {
            yield return hammerRoutine;
        }

        isHitting = false;
    }



    private IEnumerator HitAnimation ( GameObject hole )
    {
        Vector3 holePos = hole.transform.position;
        Vector3 startPos = hammerRestPosition;
        Vector3 targetPos = new Vector3(holePos.x, startPos.y, holePos.z) + hitOffset;

        float moveToTime = 0.1f;
        float elapsed = 0f;
        while (elapsed < moveToTime)
        {
            hammerBase.position = Vector3.Lerp(startPos, targetPos, elapsed / moveToTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        hammerBase.position = targetPos;

        yield return RotateHammer(initialHammerAngle, hitAngle);
        yield return new WaitForSeconds(hitPause);
        yield return RotateHammer(hitAngle, initialHammerAngle);

        elapsed = 0f;
        while (elapsed < moveToTime)
        {
            hammerBase.position = Vector3.Lerp(targetPos, startPos, elapsed / moveToTime);
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

    private void SpawnHammerClone ()
    {
        if (hammerCloneInstance == null && hammerClonePrefab != null)
        {
            GameObject cloneObj = Instantiate(hammerClonePrefab, hammerRestPosition, Quaternion.Euler(0f, 0f, 0f));
            hammerCloneInstance = cloneObj.GetComponent<HammerCloneController>();

            hammerCloneInstance.ActivateClone(hammerRestPosition, initialHammerAngle);
            Debug.Log("Hammer Clone instanciado y activado correctamente.");
        }
        else if (hammerCloneInstance != null)
        {
            hammerCloneInstance.ActivateClone(hammerRestPosition, initialHammerAngle);
        }
    }

    private void HandleDoubleHitEnd ()
    {
        if (hammerCloneInstance != null)
        {
            Debug.Log("Desactivando el clon porque terminó el double hit.");
            Destroy(hammerCloneInstance.gameObject);
            hammerCloneInstance = null;
        }
    }

    //public void EnableMoleVision ( float duration, LayerMask moleLayer )
    //{
    //    if (moleVisionRoutine != null)
    //        StopCoroutine(moleVisionRoutine);

    //    moleVisionRoutine = StartCoroutine(MoleVisionRoutine(duration, moleLayer));
    //}

    //private IEnumerator MoleVisionRoutine ( float duration, LayerMask moleLayer )
    //{
    //    originalCullingMask = hammerCamera.cullingMask; // hammerCamera ya debería estar referenciado

    //    hammerCamera.cullingMask |= moleLayer.value;
    //    float remainingTime = duration;

    //    while (remainingTime > 0f)
    //    {
    //        Debug.Log($"Visión topo activa: {remainingTime:F1}s restantes");
    //        remainingTime -= 1f;
    //        yield return new WaitForSeconds(1f);
    //    }

    //    hammerCamera.cullingMask = originalCullingMask;
    //    Debug.Log("Visión topo desactivada");
    //}


    void OnCollisionEnter ( Collision collision )
    {
        if (collision.gameObject.CompareTag("Mole"))
        {
            Debug.Log("Colisiona con el topo!");
        }
    }

}
