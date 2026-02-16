using UnityEngine;
using UnityEngine.Accessibility;

public class FPSController : MonoBehaviour
{
    [SerializeField] private Animator _vacuumAnimator;
    [SerializeField] private CharacterController _controller;
    [SerializeField] private float _speed = 5f;

    private float _yGravity = -9.81f;
    private float _verticalVelocity;
    private float _lookSpeed = 1f;
    private float _xRotation = 0f;

    private float _smoothedSpeed;
    private float acceleration;
    public float GetRotationX() => _xRotation;
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Update()
    {
        HandleLook();
        HandleMovement();
        ApplyGravity();

        UpdateVacuumAnimation();
    }

    private void UpdateVacuumAnimation()
    {
        Vector2 moveInput = GameInput.Instance.GetMoveDirection().normalized;
        float targetSpeed = moveInput.magnitude * _speed;

        if(targetSpeed > 0)
        {
            acceleration = 5f;
        }
        else
        {
            acceleration = 10f;
        }

        _smoothedSpeed = Mathf.MoveTowards(_smoothedSpeed, targetSpeed, Time.deltaTime * acceleration);

        if (_speed > 0)
        {
            _vacuumAnimator.SetFloat("Speed", (_smoothedSpeed / _speed) * 1.1f);
        }
    }

    private void HandleLook()
    {
        Vector2 lookInput = GameInput.Instance.GetLookDelta() * _lookSpeed;
        transform.Rotate(Vector3.up * lookInput.x);

        _xRotation -= lookInput.y;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);
    }

    private void HandleMovement()
    {
        Vector2 _moveInput = GameInput.Instance.GetMoveDirection();
        Vector3 moveDir = transform.right * _moveInput.x
            + transform.forward * _moveInput.y;
        _controller.Move(moveDir * _speed * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        if (_controller.isGrounded && _verticalVelocity < 0)
        {
            _verticalVelocity = -2f;
        }
        else
        {
            _verticalVelocity += _yGravity * Time.deltaTime;
        }
        _controller.Move(new Vector3(0, _verticalVelocity, 0) * Time.deltaTime);
    }
}