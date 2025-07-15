using UnityEngine;

namespace Player
{
    public class MovementController : MonoBehaviour
    {
        [Range(0, 100)]
        public float _mouseSensitivity = 15f;
        [Range(0f, 200f)]
        private float _snappiness = 100f;
        [Range(0f, 20f)]
        public float _walkSpeed = 10f;
        [Range(0f, 30f)]
        public float _sprintSpeed = 15f;
        [Range(0f, 15f)]
        public float _jumpSpeed = 3f;

        public float _normalFov = 60f;
        public float _sprintFov = 70f;
        public float _fovChangeSpeed = 5f;

        public bool _canJump = true;
        public bool _canSprint = true;

        public Transform _groundCheck;
        public float _groundDistance = 0.3f;
        public LayerMask _groundMask;
        public Transform _playerCamera;
        private Camera _camera;

        private float _rotX, _rotY;
        private float _xVelocity, _yVelocity;
        private CharacterController _characterController;
        private Vector3 _moveDirection = Vector3.zero;
        private bool _isGrounded;
        private Vector2 _moveInput;
        public bool _isSprinting;

        private bool _lookEnabled = true;
        private bool _moveEnabled = true;
        private float _currentCameraHeight;
        private float _currentFov;
        private float _fovVelocity;
        private float _gravity;

        private void Awake()
        {
            _gravity = 9.81f;
            _characterController = GetComponent<CharacterController>();
            _camera = _playerCamera.GetComponent<Camera>();
            _currentFov = _normalFov;
            SetCursorVisibility(true);
        }

        private void Update()
        {
            _isGrounded = Physics.Raycast(new(_groundCheck.position, Vector3.down), _groundDistance, _groundMask);
            if (_isGrounded && _moveDirection.y < 0) {
                _moveDirection.y = 0;
            }

            float deltaTime = Time.deltaTime;

            if (_lookEnabled) {
                float mouseX = Input.GetAxis("Mouse X") * 10 * _mouseSensitivity * deltaTime;
                float mouseY = Input.GetAxis("Mouse Y") * 10 * _mouseSensitivity * deltaTime;
                _rotX += mouseX;
                _rotY -= mouseY;
                _rotY = Mathf.Clamp(_rotY, -90f, 90f);
                _xVelocity = Mathf.Lerp(_xVelocity, _rotX, _snappiness * deltaTime);
                _yVelocity = Mathf.Lerp(_yVelocity, _rotY, _snappiness * deltaTime);
                _playerCamera.transform.localRotation = Quaternion.Euler(_yVelocity, 0f, 0f);
                transform.rotation = Quaternion.Euler(0f, _xVelocity, 0f);
            }

            float targetFov = _isSprinting ? _sprintFov : _normalFov;
            _currentFov = Mathf.SmoothDamp(_currentFov, targetFov, ref _fovVelocity, 1f / _fovChangeSpeed);
            _camera.fieldOfView = _currentFov;

            HandleMovement(deltaTime);
        }

        private void HandleMovement(float deltaTime)
        {
            if (_moveEnabled) {
                _moveInput.x = Input.GetAxis("Horizontal");
                _moveInput.y = Input.GetAxis("Vertical");
                _isSprinting = _canSprint && Input.GetKey(KeyCode.LeftShift) && _moveInput.y > 0.1f && _isGrounded;

                float currentSpeed = _isSprinting ? _sprintSpeed : _walkSpeed;
                Vector3 direction = new(_moveInput.x, 0f, _moveInput.y);
                Vector3 moveVector = transform.TransformDirection(direction) * currentSpeed;
                moveVector = Vector3.ClampMagnitude(moveVector, currentSpeed);

                if (_isGrounded) {
                    if (_canJump && Input.GetKeyDown(KeyCode.Space)) {
                        _moveDirection.y = _jumpSpeed;
                    } else if (_moveDirection.y < 0) {
                        _moveDirection.y = -2f;
                    }
                } else {
                    _moveDirection.y -= _gravity * deltaTime;
                }

                _moveDirection = new(moveVector.x, _moveDirection.y, moveVector.z);
            } else {
                _moveDirection = Vector3.zero;
            }

            _characterController.Move(_moveDirection * deltaTime);
        }

        public void SetCursorVisibility(bool newVisibility)
        {
            Cursor.lockState = newVisibility ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = newVisibility;
        }
    }
}