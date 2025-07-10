using System.Collections.Generic;
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
    private Vector2 movementInput;

    // === Pop out ===
    [SerializeField] private float popOutCooldown;

    void Awake()
    {
        holeNavigationScript = GetComponent<HoleNavigation>();

        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["Move"];
        popOutAction = playerInput.actions["PopOut"];
    }

    void Update()
    {
        movementTimer += Time.deltaTime;
        movementInput = moveAction.ReadValue<Vector2>();

        // Only processed if sufficient time has passed since the last movement
        if (movementTimer >= movementCooldown && movementInput != Vector2.zero)
        {
            holeNavigationScript.SelectHole(movementInput, Vector3.left, Vector3.back);
            movementTimer = 0;
        }
    }
}