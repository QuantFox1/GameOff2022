using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterController2D : MonoBehaviour
{
    [Header("Movement settings")]
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _gravityScale;

    [Header("Camera settings")]
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private float _cameraMovementSpeed;
    [SerializeField] private Vector2 _cameraOffset;

    private bool _facingRight = true;
    private bool _isGrounded = false;
    private float _moveDirection = 0;

    private Rigidbody2D _rigidbody;
    private CircleCollider2D _collider;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<CircleCollider2D>();

        _rigidbody.freezeRotation = true;
        _rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        _rigidbody.gravityScale = _gravityScale;

        _facingRight = transform.localScale.x > 0;
    }

    void Update()
    {
        UpdateMoveDirection();
        UpdateFacingDirection();

        HandleJump();

        MoveCamera();
    }

    void FixedUpdate()
    {
        UpdateIsGrounded();
        UpdateVelocity();
    }

    private void UpdateMoveDirection()
    {
        if ((IsLeftKeyPressed || IsRightKeyPressed) && (_isGrounded || Mathf.Abs(_rigidbody.velocity.x) > 0.01f))
        {
            _moveDirection = IsLeftKeyPressed ? -1 : 1;
            return;
        }

        if (_isGrounded || _rigidbody.velocity.magnitude < 0.01f)
        {
            _moveDirection = 0;
        }
    }

    private void UpdateFacingDirection()
    {
        if (_moveDirection == 0)
        {
            return;
        }

        if (_moveDirection > 0 && !_facingRight)
        {
            _facingRight = true;
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        if (_moveDirection < 0 && _facingRight)
        {
            _facingRight = false;
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    private void HandleJump()
    {
        if (!_isGrounded)
        {
            return;
        }

        if (!IsJumpKeyPressed)
        {
            return;
        }

        _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _jumpForce);
    }

    private void MoveCamera()
    {
        if (_mainCamera == null)
        {
            return;
        }

        var targetPosition = new Vector3(transform.position.x, transform.position.y, _mainCamera.transform.position.z) + (Vector3)_cameraOffset;

        _mainCamera.transform.position = Vector3.Lerp(_mainCamera.transform.position, targetPosition, Time.deltaTime * _cameraMovementSpeed);
    }

    private void UpdateIsGrounded()
    {
        var colliderBounds = _collider.bounds;
        var colliderRadius = _collider.radius * 0.4f * Mathf.Abs(transform.localScale.x);
        var groundCheckPosition = colliderBounds.min + new Vector3(colliderBounds.size.x * 0.5f, colliderRadius * 0.9f, 0);

        var colliders = Physics2D.OverlapCircleAll(groundCheckPosition, colliderRadius);

        _isGrounded = colliders.Where(collider => collider != _collider).Any();
    }

    private void UpdateVelocity() 
        => _rigidbody.velocity = new Vector2(_moveDirection * _maxSpeed, _rigidbody.velocity.y);

    private bool IsLeftKeyPressed
        => Input.GetKey(KeyCode.A);

    private bool IsRightKeyPressed
        => Input.GetKey(KeyCode.D);

    private bool IsJumpKeyPressed
        => Input.GetKey(KeyCode.W);
}
