using UnityEngine;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance {  get; private set; }

    private Vector3 _moveInput;
    private PlayerInputAction _plAction;
    private void Awake()
    {
        Instance = this;

        _plAction = new PlayerInputAction();
        _plAction.Enable();
    }

    private void Update()
    {
        GetMoveDirection();
    }

    public Vector2 GetMoveDirection()
    {
        return _plAction.Player.Move.ReadValue<Vector2>();
    }

    public Vector2 GetLookDelta()
    {
        return _plAction.Player.Look.ReadValue<Vector2>();
    }
}
