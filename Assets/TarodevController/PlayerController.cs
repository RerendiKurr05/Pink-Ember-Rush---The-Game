using System;
using UnityEngine;

namespace Controller
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class PlayerController : MonoBehaviour, IPlayerController
    {
        [SerializeField] private ScriptableStats _stats;
        private Rigidbody2D _rb;
        private CapsuleCollider2D _col;
        private FrameInput _frameInput;
        private Vector2 _frameVelocity;
        private bool _cachedQueryStartInColliders;

        // --- ADDED: Missing Variables for Custom Mechanics ---
        [Header("Dash Settings")]
        public float dashSpeed = 20f;
        public float dashDuration = 0.2f;
        public float dashCooldown = 1f;
        private float _dashTimeLeft;
        private float _dashCooldownTimer;
        private bool _isDashing;

        [Header("Wall Jump Settings")]
        public float wallSlideSpeed = 2f;
        public Vector2 wallJumpForce = new Vector2(10f, 15f);
        public float wallJumpDuration = 0.25f;
        private float _wallJumpTimer;
        private bool _isWallSliding;
        private int _wallDirX;

        // Collision Checks
        private bool _colLeft;
        private bool _colRight;
        private bool _colDown;


        #region Interface

        public Vector2 FrameInput => _frameInput.Move;
        public event Action<bool, float> GroundedChanged;
        public event Action Jumped;

        #endregion

        private float _time;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _col = GetComponent<CapsuleCollider2D>();
            _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
        }

        private void Update() 
        {
            _time += Time.deltaTime;
            
            // Restored original Tarodev input gathering
            GatherInput(); 

            // Your custom mechanics
            CalculateDash();
            CalculateWallMechanics();
        }

        // ADDED: The missing method that translates Unity inputs into Tarodev logic
        private void GatherInput()
        {
            _frameInput = new FrameInput
            {
                JumpDown = Input.GetButtonDown("Jump"),
                JumpHeld = Input.GetButton("Jump"),
                Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"))
            };

            if (_frameInput.JumpDown)
            {
                _jumpToConsume = true;
                _timeJumpWasPressed = _time;
            }
        }

        private void FixedUpdate()
        {
            CheckCollisions();

            // Suspend normal horizontal/jump logic if the player is currently dashing
            if (!_isDashing)
            {
                HandleJump();
                HandleDirection();
                HandleGravity();
            }

            ApplyMovement();
        }

        #region Collisions

        private float _frameLeftGrounded = float.MinValue;
        private bool _grounded;

        private void CheckCollisions()
        {
            Physics2D.queriesStartInColliders = false;

            // Ground and Ceiling
            bool groundHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.down, _stats.GrounderDistance, ~_stats.PlayerLayer);
            bool ceilingHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.up, _stats.GrounderDistance, ~_stats.PlayerLayer);

            // ADDED: Left/Right checks to enable your Wall Slide logic
            _colLeft = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.left, _stats.GrounderDistance, ~_stats.PlayerLayer);
            _colRight = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.right, _stats.GrounderDistance, ~_stats.PlayerLayer);
            _colDown = groundHit;

            // Hit a Ceiling
            if (ceilingHit) _frameVelocity.y = Mathf.Min(0, _frameVelocity.y);

            // Landed on the Ground
            if (!_grounded && groundHit)
            {
                _grounded = true;
                _coyoteUsable = true;
                _bufferedJumpUsable = true;
                _endedJumpEarly = false;
                GroundedChanged?.Invoke(true, Mathf.Abs(_frameVelocity.y));
            }
            // Left the Ground
            else if (_grounded && !groundHit)
            {
                _grounded = false;
                _frameLeftGrounded = _time;
                GroundedChanged?.Invoke(false, 0);
            }

            Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
        }

        #endregion

        #region Jumping

        private bool _jumpToConsume;
        private bool _bufferedJumpUsable;
        private bool _endedJumpEarly;
        private bool _coyoteUsable;
        private float _timeJumpWasPressed;

        private bool HasBufferedJump => _bufferedJumpUsable && _time < _timeJumpWasPressed + _stats.JumpBuffer;
        private bool CanUseCoyote => _coyoteUsable && !_grounded && _time < _frameLeftGrounded + _stats.CoyoteTime;

        private void HandleJump()
        {
            if (!_endedJumpEarly && !_grounded && !_frameInput.JumpHeld && _rb.velocity.y > 0) _endedJumpEarly = true;

            if (!_jumpToConsume && !HasBufferedJump) return;

            if (_grounded || CanUseCoyote) ExecuteJump();

            _jumpToConsume = false;
        }

        private void ExecuteJump()
        {
            _endedJumpEarly = false;
            _timeJumpWasPressed = 0;
            _bufferedJumpUsable = false;
            _coyoteUsable = false;
            _frameVelocity.y = _stats.JumpPower;
            Jumped?.Invoke();
        }
        
        private void CalculateWallMechanics()
        {
            bool touchingWall = _colLeft || _colRight;
            _wallDirX = _colRight ? 1 : -1;

            // WALL SLIDE 
            if (touchingWall && !_colDown && _frameVelocity.y < 0)
            {
                _isWallSliding = true;

                if (_frameVelocity.y < -wallSlideSpeed)
                {
                    _frameVelocity.y = -wallSlideSpeed;
                }
            }
            else
            {
                _isWallSliding = false;
            }

            if (_wallJumpTimer > 0) _wallJumpTimer -= Time.deltaTime;

            // WALL JUMP 
            if (_isWallSliding && _frameInput.JumpDown)
            {
                _wallJumpTimer = wallJumpDuration;
                _frameVelocity.x = -_wallDirX * wallJumpForce.x;
                _frameVelocity.y = wallJumpForce.y;
            }
        }

        #endregion

        #region Horizontal

        private void HandleDirection()
        {
            // Lock out normal horizontal movement while performing a wall jump
            if (_wallJumpTimer > 0) return;

            if (_frameInput.Move.x == 0)
            {
                var deceleration = _grounded ? _stats.GroundDeceleration : _stats.AirDeceleration;
                _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
            }
            else
            {
                _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, _frameInput.Move.x * _stats.MaxSpeed, _stats.Acceleration * Time.fixedDeltaTime);
            }
        }
        
        private void CalculateDash()
        {
            if (_dashCooldownTimer > 0) _dashCooldownTimer -= Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.LeftShift) && _dashCooldownTimer <= 0 && !_isDashing)
            {
                _isDashing = true;
                _dashTimeLeft = dashDuration;
                _dashCooldownTimer = dashCooldown;
            }

            if (_isDashing)
            {
                _dashTimeLeft -= Time.deltaTime;

                float facingDir = _frameVelocity.x != 0 ? Mathf.Sign(_frameVelocity.x) : transform.localScale.x;
                if (Input.GetAxisRaw("Horizontal") != 0) facingDir = Mathf.Sign(Input.GetAxisRaw("Horizontal"));

                _frameVelocity.y = 0; // Hover slightly while dashing
                _frameVelocity.x = dashSpeed * facingDir;

                if (_dashTimeLeft <= 0)
                {
                    _isDashing = false;
                }
            }
        }

        #endregion

        #region Gravity

        private void HandleGravity()
        {
            // Wall slide logic manages its own gravity
            if (_isWallSliding) return;

            if (_grounded && _frameVelocity.y <= 0f)
            {
                _frameVelocity.y = _stats.GroundingForce;
            }
            else
            {
                var inAirGravity = _stats.FallAcceleration;
                if (_endedJumpEarly && _frameVelocity.y > 0) inAirGravity *= _stats.JumpEndEarlyGravityModifier;
                _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, -_stats.MaxFallSpeed, inAirGravity * Time.fixedDeltaTime);
            }
        }

        #endregion

        private void ApplyMovement() => _rb.velocity = _frameVelocity;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_stats == null) Debug.LogWarning("Please assign a ScriptableStats asset to the Player Controller's Stats slot", this);
        }
#endif
    }

    public struct FrameInput
    {
        public bool JumpDown;
        public bool JumpHeld;
        public Vector2 Move;
    }

    public interface IPlayerController
    {
        public event Action<bool, float> GroundedChanged;

        public event Action Jumped;
        public Vector2 FrameInput { get; }
    }
}