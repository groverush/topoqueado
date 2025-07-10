using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class HammerController : MonoBehaviour
{
    [SerializeField] private float moveStep = 1.5f;
    [SerializeField] private Vector2Int gridPosition = new Vector2Int(1, 2); // Posición inicial (centro abajo = hoyo 8)
    [SerializeField] private Transform hammerHead;
    [SerializeField] private float hitSpeed = 10f;

    private bool isHitting = false;
    private Vector2Int previousDirection = Vector2Int.zero;

    public void MoveHammer ( InputAction.CallbackContext context )
    {
        if (!context.performed || isHitting) return;

        Vector2 input = context.ReadValue<Vector2>();
        Vector2Int direction = Vector2Int.zero;

        // Leer con margen de tolerancia y convertir a dirección grilla
        if (input.y > 0.5f)       // W
            direction = new Vector2Int(0, -1);
        else if (input.y < -0.5f) // S
            direction = new Vector2Int(0, 1);
        else if (input.x < -0.5f) // A
            direction = new Vector2Int(-1, 0);
        else if (input.x > 0.5f)  // D
            direction = new Vector2Int(1, 0);

        if (direction == Vector2Int.zero || direction == previousDirection)
            return;

        // Calcular nueva posición tentativa
        Vector2Int nextPosition = gridPosition + direction;

        // Verificar que esté dentro de los límites de la grilla (0 a 2)
        if (nextPosition.x >= 0 && nextPosition.x <= 2 && nextPosition.y >= 0 && nextPosition.y <= 2)
        {
            gridPosition = nextPosition;
            UpdateHammerPosition();
        }

        // Guardar dirección para no repetir movimiento mientras se mantiene presionada
        previousDirection = direction;
    }

    public void ResetDirection ( InputAction.CallbackContext context )
    {
        if (!context.canceled) return;
        previousDirection = Vector2Int.zero;
    }

    public void OnHit ()
    {
        if (!isHitting)
            StartCoroutine(HitAnimation());
    }

    private void UpdateHammerPosition ()
    {
        Vector3 newPosition = new Vector3(
            gridPosition.x * moveStep,         // eje X: columnas
            transform.position.y,              // altura se mantiene
            -gridPosition.y * moveStep         // eje Z invertido para representar filas de arriba a abajo
        );
        transform.position = newPosition;
    }

    private IEnumerator HitAnimation ()
    {
        isHitting = true;

        float rotationAmount = 0;
        while (rotationAmount < 90)
        {
            float step = hitSpeed * Time.deltaTime;
            hammerHead.Rotate(Vector3.right, step);
            rotationAmount += step;
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        while (rotationAmount > 0)
        {
            float step = hitSpeed * Time.deltaTime;
            hammerHead.Rotate(Vector3.left, step);
            rotationAmount -= step;
            yield return null;
        }

        isHitting = false;
        Debug.Log("Hammer hit!");
    }
}