using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    #region Variables
    [Header("Components")]
    [SerializeField] private Rigidbody2D _rb; // Player rigid body 2D
    [SerializeField] private Collider2D _collider; // Player physics collider

    [Header("Variables")]
    public float movementSpeed = 1f; // Horizontal force applied when moving left/right
    [SerializeField] private float jumpStrength = 1f; // Vertical force applied when jumping
    [SerializeField] private float wallJumpSrength = 0.5f; // Horizontal force applied before jumping when wall sliding
    [SerializeField] private Vector2 _maxVelocity = new(5f, 5f); // Max velocity values
    [SerializeField] private bool _canMove = true, _canJump = true; // Movement limiter bools
    [SerializeField] private bool _isMoving, _isGrounded, _isWallSliding, _isFacingRight; // Movement state bools

    private Vector2 _movementInput; // Stored WASD/Arrow Keys input
    #endregion

    #region Update
    void Update()
    {
        // Determine grounded and wall sliding states
        DetermineGrounded();
        DetermineWallSliding();

        // Update player orientation
        UpdateOrientation();

        // Update _isMoving bool
        _isMoving = !(_movementInput == Vector2.zero && _rb.linearVelocity == Vector2.zero && _isGrounded);
    }
    void FixedUpdate()
    {
        // Move the player, then clamp their velocity
        MovePlayer();
        LimitVelocity();
    }
    #endregion

    #region Main Methods
    private void MovePlayer()
    {
        if (_movementInput == Vector2.zero || _rb == null || !_canMove) return; // Do nothing if there is no input, the rigidbody is null, or _canMove is false

        if (_movementInput.x > 0 && _rb.linearVelocityX < _maxVelocity.x)
            _rb.AddForceX(_movementInput.x * movementSpeed, ForceMode2D.Force);
        else if (_movementInput.x < 0 && _rb.linearVelocityX > -_maxVelocity.x)
            _rb.AddForceX(_movementInput.x * movementSpeed, ForceMode2D.Force);
    }
    private void Jump()
    {
        if (_rb == null || !_canJump) return; // Do nothing if the rigidbody is null or _canJump is false

        // Disable jump only if jumping from the air
        if (!_isGrounded && !_isWallSliding) _canJump = false;

        // If wall sliding, first apply a small force away from the wall
        if (_isWallSliding)
        {
            if (_isFacingRight) _rb.AddForceX(-wallJumpSrength, ForceMode2D.Impulse); // If facing right, apply force left (negative)
            else _rb.AddForceX(wallJumpSrength, ForceMode2D.Impulse); // If facing left, apply force right (positive)
        }

        // Halt Y velocity then apply force so that jump strength is always the same
        _rb.linearVelocityY = 0;
        _rb.AddForceY(jumpStrength, ForceMode2D.Impulse);
    }
    private void LimitVelocity()
    {
        if (_rb == null) return; // Rigidbody null check

        // Clamp velocity on the X axis
        if (Mathf.Abs(_rb.linearVelocityX) > _maxVelocity.x)
            _rb.linearVelocityX = (_rb.linearVelocityX > 0) ? _maxVelocity.x : -_maxVelocity.x;
        // Clamp velocity on the Y axis
        if (Mathf.Abs(_rb.linearVelocityY) > _maxVelocity.y)
            _rb.linearVelocityY = (_rb.linearVelocityY > 0) ? _maxVelocity.y : -_maxVelocity.y;
    }
    private void UpdateOrientation()
    {
        // If the player is inputting right
        if (_movementInput.x > 0)
        {
            // Flip the scale to face right if necessary
            if (transform.localScale.x < 0) transform.localScale = new(transform.localScale.x * -1f, transform.localScale.y, transform.localScale.z);

            // Flip facingRight bool to face right if necessary
            if (!_isFacingRight) _isFacingRight = true;
        }
        // If the player is inputting left
        else if (_movementInput.x < 0)
        {
            // Flip the scale to face left if necessary
            if (transform.localScale.x > 0) transform.localScale = new(transform.localScale.x * -1f, transform.localScale.y, transform.localScale.z);

            // Flip facingRight bool to face left if necessary
            if (_isFacingRight) _isFacingRight = false;
        }
    }
    #endregion

    #region Helper Methods
    private void DetermineGrounded()
    {
        if (_collider == null) return; // Collider null check

        // Calculate 3 ray origins at the player's feet
        Vector2 middleRayOrigin = new(_collider.bounds.center.x, _collider.bounds.min.y);
        Vector2 leftRayOrigin = middleRayOrigin + Vector2.left * (_collider.bounds.size.x / 2);
        Vector2 rightRayOrigin = middleRayOrigin + Vector2.right * (_collider.bounds.size.x / 2);

        // Set the ray direction, length, and layer mask
        Vector2 rayDirection = Vector2.down;
        LayerMask groundLayer = LayerMask.GetMask("Environment");
        float rayLength = 0.05f;

        // Do the raycasts
        RaycastHit2D middleHit = Physics2D.Raycast(middleRayOrigin, rayDirection, rayLength, groundLayer);
        RaycastHit2D leftHit = Physics2D.Raycast(leftRayOrigin, rayDirection, rayLength, groundLayer);
        RaycastHit2D rightHit = Physics2D.Raycast(rightRayOrigin, rayDirection, rayLength, groundLayer);

        // If any ray hits ground, set grounded to true and enable jump and dash
        if ((middleHit.collider != null && middleHit.collider.CompareTag("Ground")) || (leftHit.collider != null && leftHit.collider.CompareTag("Ground")) || (rightHit.collider != null && rightHit.collider.CompareTag("Ground")))
        {
            if (!_isGrounded) _isGrounded = true;
            if (!_canJump) _canJump = true;

            // Draw debug rays
            Debug.DrawRay(middleRayOrigin, rayDirection * rayLength, (middleHit.collider != null) ? Color.green : Color.red);
            Debug.DrawRay(leftRayOrigin, rayDirection * rayLength, (leftHit.collider != null) ? Color.green : Color.red);
            Debug.DrawRay(rightRayOrigin, rayDirection * rayLength, (rightHit.collider != null) ? Color.green : Color.red);
        }
        // If no ray hits the ground, set grounded to false and draw red debug rays
        else
        {
            if (_isGrounded) _isGrounded = false;
            Debug.DrawRay(middleRayOrigin, rayDirection * rayLength, Color.red);
            Debug.DrawRay(leftRayOrigin, rayDirection * rayLength, Color.red);
            Debug.DrawRay(rightRayOrigin, rayDirection * rayLength, Color.red);
        }
    }
    private void DetermineWallSliding()
    {
        if (_collider == null) return; // Collider null check

        // Calculate the origins of upper and lower raycasts
        Vector2 middleRayOrigin = (_isFacingRight) ? new(_collider.bounds.center.x + _collider.bounds.size.x / 2f, _collider.bounds.center.y) : new(_collider.bounds.center.x - _collider.bounds.size.x / 2f, _collider.bounds.center.y);
        Vector2 upperRayOrigin = middleRayOrigin + Vector2.up * (_collider.bounds.size.y / 2);
        Vector2 lowerRayOrigin = middleRayOrigin + Vector2.down * (_collider.bounds.size.y / 2);

        // Determine the rays' direction and set their length
        Vector2 rayDirection = _isFacingRight ? Vector2.right : Vector2.left;
        float rayLength = 0.05f;
        LayerMask wallLayer = LayerMask.GetMask("Environment");

        // Do the raycasts
        RaycastHit2D upperHit = Physics2D.Raycast(upperRayOrigin, rayDirection, rayLength, wallLayer);
        RaycastHit2D middleHit = Physics2D.Raycast(middleRayOrigin, rayDirection, rayLength, wallLayer);
        RaycastHit2D lowerHit = Physics2D.Raycast(lowerRayOrigin, rayDirection, rayLength, wallLayer);

        // If any ray hits a wall, set wallSliding to true and enable jump and dash
        if ((upperHit.collider != null && upperHit.collider.CompareTag("Wall")) || (middleHit.collider != null && middleHit.collider.CompareTag("Wall")) || (lowerHit.collider != null && lowerHit.collider.CompareTag("Wall")))
        {
            // Set bools
            if (!_isWallSliding) _isWallSliding = true;
            if (!_canJump) _canJump = true;

            // Draw debug rays
            Debug.DrawRay(upperRayOrigin, rayDirection * rayLength, (upperHit.collider != null) ? Color.green : Color.red);
            Debug.DrawRay(middleRayOrigin, rayDirection * rayLength, (middleHit.collider != null) ? Color.green : Color.red);
            Debug.DrawRay(lowerRayOrigin, rayDirection * rayLength, (lowerHit.collider != null) ? Color.green : Color.red);
        }
        // If no ray hits a wall, set wallSliding to false and draw red debug rays
        else
        {
            if (_isWallSliding) _isWallSliding = false;
            Debug.DrawRay(upperRayOrigin, rayDirection * rayLength, Color.red);
            Debug.DrawRay(middleRayOrigin, rayDirection * rayLength, Color.red);
            Debug.DrawRay(lowerRayOrigin, rayDirection * rayLength, Color.red);
        }
    }
    #endregion

    #region Input Methods
    public void OnMove(InputValue inputValue)
    {
        // Set movement input, actual movement handled in FixedUpdate
        _movementInput = inputValue.Get<Vector2>();
    }
    public void OnJump()
    {
        // Call jump method
        Jump();
    }
    #endregion
}