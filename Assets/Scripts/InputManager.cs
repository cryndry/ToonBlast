using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private InputManager() { }
    public static InputManager Instance { get; private set; }

    private InputSystemActions inputSystemActions;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            // If another instance already exists,
            // destroy this new one and stop.
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        inputSystemActions = new InputSystemActions();
    }

    private void OnEnable()
    {
        inputSystemActions.UI.Click.Enable();
        inputSystemActions.UI.Click.performed += OnLeftClickCallback;
    }

    private void OnDisable()
    {
        inputSystemActions.UI.Click.performed -= OnLeftClickCallback;
        inputSystemActions.UI.Click.Disable();
    }

    private void OnLeftClickCallback(InputAction.CallbackContext context)
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPosition, Vector2.zero);
        if (hit.collider != null)
        {
            EventManager.InvokeTap(hit.collider);
        }
    }
}
