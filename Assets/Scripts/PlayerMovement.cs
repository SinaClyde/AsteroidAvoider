using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float forceMagnitude;
    [SerializeField] private float maxVelocity;
    [SerializeField] private float rotationSpeed;

    private Rigidbody _rb;
    private Camera _mainCamera;

    private Vector3 _movementDirection;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();

        _mainCamera = Camera.main;
    }

    void Update()
    {
        ProcessInput();

        KeepPlayerOnScreen();

        RotateToFaceVelocity();
    }

    void FixedUpdate()
    {
        if (_movementDirection == Vector3.zero) { return; }

        _rb.AddForce(_movementDirection * (forceMagnitude * Time.deltaTime), ForceMode.Force);

        _rb.linearVelocity = Vector3.ClampMagnitude(_rb.linearVelocity, maxVelocity);
    }

    private void ProcessInput()
    {
        if (Touchscreen.current.primaryTouch.press.isPressed)
        {
            Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();

            Vector3 worldPosition = _mainCamera.ScreenToWorldPoint(touchPosition);

            _movementDirection = transform.position - worldPosition;
            _movementDirection.z = 0f;
            _movementDirection.Normalize();
        }
        else
        {
            _movementDirection = Vector3.zero;
        }
    }

    private void KeepPlayerOnScreen()
    {
        Vector3 newPosition = transform.position;
        Vector3 viewportPosition = _mainCamera.WorldToViewportPoint(transform.position);

        if (viewportPosition.x > 1)
        {
            newPosition.x = -newPosition.x + 0.1f;
        }
        else if (viewportPosition.x < 0)
        {
            newPosition.x = -newPosition.x - 0.1f;
        }

        if (viewportPosition.y > 1)
        {
            newPosition.y = -newPosition.y + 0.1f;
        }
        else if (viewportPosition.y < 0)
        {
            newPosition.y = -newPosition.y - 0.1f;
        }

        transform.position = newPosition;
    }

    private void RotateToFaceVelocity()
    {
        if (_rb.linearVelocity == Vector3.zero) { return; }

        Quaternion targetRotation = Quaternion.LookRotation(_rb.linearVelocity, Vector3.back);

        transform.rotation = Quaternion.Lerp(
            transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
