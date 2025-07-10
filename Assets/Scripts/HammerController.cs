using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class HammerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveStep = 1.5f; // Distance the hammer moves per input step
    [SerializeField] private float moveDuration = 0.1f; // Duration of each movement
    [SerializeField] private float repeatDelay = 0.2f; // Delay between repeated movements

    [SerializeField] private Vector2 movementLimitsX = new Vector2(-1.5f, 1.5f); // Horizontal boundaries
    [SerializeField] private Vector2 movementLimitsZ = new Vector2(-4.5f, -1.5f); // Depth boundaries

    [Header("Hammer Settings")]
    [SerializeField] private Transform hammerHead;
    [SerializeField] private GameObject hammerVisual;
    [SerializeField] private float initialHammerAngle = -90f; // Starting rotation angle
    [SerializeField] private float hitAngle = -180f; // Rotation angle when hitting
    [SerializeField] private float hitDownDuration = 0.08f; // Time to swing down
    [SerializeField] private float hitPause = 0.05f; // Pause after hitting
    [SerializeField] private float hitCooldown = 0.1f; // Delay before fade out
    [SerializeField] private float fadeDuration = 0.2f; // Duration of fade in/out

    [Header("Impact Preview")]
    [SerializeField] private GameObject hammerPreviewOutline;
    [SerializeField] private Vector3 impactOffset = new Vector3(0, 0, 3f); // Offset for where the hammer will land
    [SerializeField] private Vector3 previewYOffset = new Vector3(0, 0.01f, 0); // Small lift to avoid z-fighting
    [SerializeField] private bool showPreview = true; // Toggle preview visibility

    [Header("Materials - Hammer Head")]
    [SerializeField] private Renderer hammerHeadRenderer;
    [SerializeField] private Material hammerHeadOpaque;
    [SerializeField] private Material hammerHeadTransparent;

    [Header("Materials - Handle")]
    [SerializeField] private Renderer hammerHandleRenderer;
    [SerializeField] private Material hammerHandleOpaque;
    [SerializeField] private Material hammerHandleTransparent;

    private Vector2 heldInput = Vector2.zero;
    private Coroutine holdCoroutine;

    private bool isMoving = false;
    private bool isHitting = false;

    private void Start ()
    {
        if (hammerHead != null)
            hammerHead.localRotation = Quaternion.Euler(initialHammerAngle, 0f, 0f);

        SetHammerVisualTransparency(true);
        hammerVisual.SetActive(false); // Hide hammer initially

        // Ensure transparent materials start fully invisible
        hammerHeadTransparent.SetColor("_BaseColor", new Color(1f, 1f, 1f, 0f));
        hammerHandleTransparent.SetColor("_BaseColor", new Color(1f, 1f, 1f, 0f));
    }

    private void Update ()
    {
        UpdateHammerPreviewOutline(); // Update position of impact preview
    }

    // Handle player input to move the hammer
    public void MoveHammer ( InputAction.CallbackContext context )
    {
        Vector2 input = context.ReadValue<Vector2>();

        if (context.performed && !isHitting)
        {
            heldInput = input;
            if (holdCoroutine == null)
                holdCoroutine = StartCoroutine(HoldMove());
        }

        if (context.canceled)
        {
            heldInput = Vector2.zero;
            if (holdCoroutine != null)
            {
                StopCoroutine(holdCoroutine);
                holdCoroutine = null;
            }
        }
    }

    // Continuously move hammer while input is held
    private IEnumerator HoldMove ()
    {
        while (heldInput != Vector2.zero)
        {
            Vector2Int direction = Vector2Int.zero;

            // Determine movement direction based on input threshold
            if (heldInput.y < -0.5f) direction = Vector2Int.up;
            else if (heldInput.y > 0.5f) direction = Vector2Int.down;
            else if (heldInput.x < -0.5f) direction = Vector2Int.left;
            else if (heldInput.x > 0.5f) direction = Vector2Int.right;

            if (direction != Vector2Int.zero)
            {
                Vector3 moveDirection = new Vector3(direction.x * moveStep, 0, -direction.y * moveStep);
                Vector3 targetPosition = transform.position + moveDirection;

                if (IsWithinLimits(targetPosition) && !isMoving)
                    yield return StartCoroutine(SmoothMove(targetPosition));
            }

            yield return new WaitForSeconds(repeatDelay);
        }
    }

    // Check if target position is inside allowed area
    private bool IsWithinLimits ( Vector3 position )
    {
        return position.x >= movementLimitsX.x && position.x <= movementLimitsX.y &&
               position.z >= movementLimitsZ.x && position.z <= movementLimitsZ.y;
    }

    // Smoothly move hammer from current to target position
    private IEnumerator SmoothMove ( Vector3 targetPos )
    {
        isMoving = true;
        Vector3 startPos = transform.position;
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        isMoving = false;
    }

    // Position the outline preview at the future impact point
    private void UpdateHammerPreviewOutline ()
    {
        if (!showPreview || hammerPreviewOutline == null) return;

        Vector3 impactPosition = transform.position + impactOffset + previewYOffset;
        impactPosition.x = Mathf.Round(impactPosition.x / moveStep) * moveStep;
        impactPosition.z = Mathf.Round(impactPosition.z / moveStep) * moveStep;

        hammerPreviewOutline.transform.position = impactPosition;
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

        hammerVisual.SetActive(true);
        SetHammerVisualTransparency(true); // Set transparent materials

        // Fade in both parts
        Coroutine fadeInHead = StartCoroutine(FadeInMaterial(hammerHeadRenderer.material, fadeDuration));
        Coroutine fadeInHandle = StartCoroutine(FadeInMaterial(hammerHandleRenderer.material, fadeDuration));
        yield return fadeInHead;
        yield return fadeInHandle;

        SetHammerVisualTransparency(false); // Switch to opaque

        // Swing down
        Quaternion startRotation = hammerHead.localRotation;
        Quaternion downRotation = startRotation * Quaternion.Euler(hitAngle, 0f, 0f);

        float elapsed = 0f;
        while (elapsed < hitDownDuration)
        {
            hammerHead.localRotation = Quaternion.Slerp(startRotation, downRotation, elapsed / hitDownDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        hammerHead.localRotation = downRotation;
        yield return new WaitForSeconds(hitPause);

        // Return to starting angle
        elapsed = 0f;
        while (elapsed < hitDownDuration)
        {
            hammerHead.localRotation = Quaternion.Slerp(downRotation, startRotation, elapsed / hitDownDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        hammerHead.localRotation = startRotation;
        yield return new WaitForSeconds(hitCooldown);

        SetHammerVisualTransparency(true);

        // Fade out
        Coroutine fadeOutHead = StartCoroutine(FadeOutMaterial(hammerHeadRenderer.material, fadeDuration));
        Coroutine fadeOutHandle = StartCoroutine(FadeOutMaterial(hammerHandleRenderer.material, fadeDuration));
        yield return fadeOutHead;
        yield return fadeOutHandle;

        hammerVisual.SetActive(false);
        isHitting = false;
    }

    // Switch between opaque and transparent materials
    private void SetHammerVisualTransparency ( bool transparent )
    {
        if (transparent)
        {
            hammerHeadRenderer.material = hammerHeadTransparent;
            hammerHandleRenderer.material = hammerHandleTransparent;
        }
        else
        {
            hammerHeadRenderer.material = hammerHeadOpaque;
            hammerHandleRenderer.material = hammerHandleOpaque;
        }
    }

    // Gradually fade in a material (opacity from 0 to 1)
    private IEnumerator FadeInMaterial ( Material mat, float duration )
    {
        if (mat.HasProperty("_BaseColor"))
        {
            Color endColor = mat.GetColor("_BaseColor");
            Color startColor = new Color(endColor.r, endColor.g, endColor.b, 0f);
            mat.SetColor("_BaseColor", startColor);

            float elapsed = 0f;

            while (elapsed < duration)
            {
                Color current = Color.Lerp(startColor, endColor, elapsed / duration);
                mat.SetColor("_BaseColor", current);
                elapsed += Time.deltaTime;
                yield return null;
            }

            mat.SetColor("_BaseColor", endColor);
        }
    }

    // Gradually fade out a material (opacity from 1 to 0)
    private IEnumerator FadeOutMaterial ( Material mat, float duration )
    {
        if (mat.HasProperty("_BaseColor"))
        {
            Color startColor = mat.GetColor("_BaseColor");
            Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

            float elapsed = 0f;

            while (elapsed < duration)
            {
                Color current = Color.Lerp(startColor, endColor, elapsed / duration);
                mat.SetColor("_BaseColor", current);
                elapsed += Time.deltaTime;
                yield return null;
            }

            mat.SetColor("_BaseColor", endColor);
        }
    }

    // Draw visual gizmos in editor for grid positioning
    private void OnDrawGizmos ()
    {
        Gizmos.color = Color.yellow;
        for (int x = 0; x < 3; x++)
        {
            for (int z = 0; z < 3; z++)
            {
                Vector3 pos = new Vector3(
                    (x * moveStep) + movementLimitsX.x,
                    transform.position.y,
                    (z * moveStep) + movementLimitsZ.x
                );
                Gizmos.DrawWireCube(pos, new Vector3(1f, 0.1f, 1f));
            }
        }
    }
}