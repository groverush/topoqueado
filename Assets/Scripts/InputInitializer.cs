using UnityEngine;
using UnityEngine.InputSystem;

public class InputInitializer : MonoBehaviour
{
    [SerializeField] private PlayerInput moleInput;
    [SerializeField] private PlayerInput hammerInput;

    void Awake()
    {
        var keyboard = Keyboard.current;

        if (moleInput != null)
        {
            moleInput.SwitchCurrentControlScheme("Keyboard", keyboard);
        }

        if (hammerInput != null)
        {
            hammerInput.SwitchCurrentControlScheme("Keyboard", keyboard);
        }
    }
}