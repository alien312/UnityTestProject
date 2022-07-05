using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float JumpHeight = 1.2f;
    public float Gravity = -15.0f;
    
    public bool Grounded = true;
    public float GroundedOffset = -0.14f;
    public float GroundedRadius = 0.5f;
    public LayerMask GroundLayers;
    
    public float JumpTimeout = 0.5f;
    public float FallTimeout = 0.15f;
    
    public float MoveSpeed = 4.0f;
    public float SprintSpeed = 6.0f;
    public float RotationSpeed = 1.0f;
    public float SpeedChangeRate = 10.0f;

    private GameObject _mainCamera;
    private bool _hasAnimator;
    private Animator _animator;
    private CharacterController _controller;
    private PlayerInputController _inputController;
    
    // таймеры
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;
    
    // player
    private float _speed;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;
    
    private void Awake()
    {
        // получили ссылку на ГО камеры
        if (_mainCamera == null)
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
    }
    
    private void Start()
    {
        _hasAnimator = TryGetComponent(out _animator);
        _controller = GetComponent<CharacterController>();
        _inputController = GetComponent<PlayerInputController>();
        
        // сбросили таймеры прыжка и падения
        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;
    }
    
    private void Update()
    {
        JumpAndGravity();
        GroundedCheck();
        Move();
    }
    
    private void JumpAndGravity()
    {
        if (Grounded)
        {
            // сбросили таймер паления
            _fallTimeoutDelta = FallTimeout;
            
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            // прыжок
            if (_inputController.IsJumping && _jumpTimeoutDelta <= 0.0f)
            {
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
            }

            // таймер прыжка
            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            // сбросили таймер прыжка
            _jumpTimeoutDelta = JumpTimeout;

            // таймер падения
            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            
            _inputController.IsJumping = false;
        }
        
        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += Gravity * Time.deltaTime;
        }
    }
    
    private void GroundedCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
    }
    
    private void Move()
    {
        // set target speed based on move speed, sprint speed and if sprint is pressed
        float targetSpeed =  _inputController.IsSprinting ? SprintSpeed : MoveSpeed;

        // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

        // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is no input, set the target speed to 0
        if (!_inputController.IsMoving) targetSpeed = 0.0f;

        // a reference to the players current horizontal velocity
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = _inputController.Move.magnitude;

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

            // round speed to 3 decimal places
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        // normalise input direction
        Vector3 inputDirection = new Vector3(_inputController.Move.x, 0.0f, _inputController.Move.y).normalized;

        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        if (_inputController.IsMoving)
        {
            // move
            inputDirection = transform.right *  _inputController.Move.x + transform.forward * _inputController.Move.y;
        }

        // move the player
        _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
    }
}