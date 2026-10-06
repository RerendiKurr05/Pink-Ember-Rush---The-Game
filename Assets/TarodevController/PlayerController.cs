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
        private SpriteRenderer _sprite;
        public bool IsMoving => _frameInput.Move.x != 0;
        public bool IsGrounded => _grounded;
        public bool IsWallSliding => _isWallSliding;
        public bool IsWallJumping => _wallJumpTimer > 0;
        public float VerticalVelocity => _frameVelocity.y;
        public bool IsDoubleJumping => _airJumpsUsed > 0 && !_grounded;

        [Header("Dash Settings")]
        public float dashSpeed = 20f;
        public float dashDuration = 0.2f;
        public float dashCooldown = 1f;
        private float _dashTimeLeft;
        private float _dashCooldownTimer;
        private bool _isDashing;
        public bool IsDashing => _isDashing;

        [Header("Wall Jump Settings")]
        public float wallSlideSpeed = 2f;
        public Vector2 wallJumpForce = new Vector2(10f, 15f);
        public float wallJumpDuration = 0.25f;
        public float wallJumpControlLockTime = 0.15f;
        public LayerMask wallLayer;
        public int maxWallJumps = 2;
        private int _wallJumpsUsed; 
        private float _wallJumpTimer;
        private float _wallJumpControlLockTimer;
        private bool _isWallSliding;
        private bool _canWallJump;
        private int _wallDirX;

        [Header("DOUBLE JUMP")]
        public int maxAirJumps = 1;
        private int _airJumpsUsed;

        private bool _colLeft;
        private bool _colRight;
        private bool _colDown;

        #region Interface

        public Vector2 FrameInput => _frameInput.Move;
        public event Action<bool, float> GroundedChanged;
        public event Action Jumped;
        public event Action DoubleJumped;

        #endregion

        private float _time;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _col = GetComponent<CapsuleCollider2D>();
            _sprite = GetComponentInChildren<SpriteRenderer>();
            _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;

            _distanceJoint = GetComponent<DistanceJoint2D>();
            if (_distanceJoint != null)
            {
                _distanceJoint.enabled = false;
            }
        }

        private void Update()
        {
            _time += Time.deltaTime;

            GatherInput();
            HandleSpriteFlip();
            CalculateGrapple();

            if (!_isGrappling)
            {
                CalculateDash();
                CalculateWallMechanics();
            }
        }

        private void HandleSpriteFlip()
        {
            if (_frameInput.Move.x > 0) _sprite.flipX = false;
            else if (_frameInput.Move.x < 0) _sprite.flipX = true;
            // kalau Move.x == 0 (gak nekan tombol), biarin tetap ke arah terakhir
        }

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

            if (!_isDashing && !_isGrappling)
            {
                HandleJump();
                HandleDirection();
                HandleGravity();
                ApplyMovement();
            }

        }

        #region Collisions

        private float _frameLeftGrounded = float.MinValue;
        private bool _grounded;

        private void CheckCollisions()
        {
            Physics2D.queriesStartInColliders = false;

            bool groundHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.down, _stats.GrounderDistance, ~_stats.PlayerLayer);
            bool ceilingHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.up, _stats.GrounderDistance, ~_stats.PlayerLayer);

            _colLeft = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.left, _stats.GrounderDistance, wallLayer);
            _colRight = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.right, _stats.GrounderDistance, wallLayer);
            _colDown = groundHit;

            if (ceilingHit) _frameVelocity.y = Mathf.Min(0, _frameVelocity.y);

            if (!_grounded && groundHit)
            {
                _grounded = true;
                _coyoteUsable = true;
                _bufferedJumpUsable = true;
                _endedJumpEarly = false;
                _airJumpsUsed = 0;
                _wallJumpsUsed = 0;
                GroundedChanged?.Invoke(true, Mathf.Abs(_frameVelocity.y));
            }
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

            // Prioritas: wall jump dulu, baru grounded/coyote, baru air jump.
            // Cuma SATU jenis jump yang boleh jalan per pencetan tombol.
            if (_canWallJump)
            {
                ExecuteWallJump();
            }
            else if (_grounded || CanUseCoyote)
            {
                ExecuteJump();
            }
            else if (_airJumpsUsed < maxAirJumps)
            {
                ExecuteJump();
                _airJumpsUsed++;
                DoubleJumped?.Invoke();
            }

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

        private void ExecuteWallJump()
        {
            _endedJumpEarly = false;
            _timeJumpWasPressed = 0;
            _bufferedJumpUsable = false;
            _coyoteUsable = false;

            _wallJumpTimer = wallJumpDuration;
            _wallJumpControlLockTimer = wallJumpControlLockTime;
            _frameVelocity.x = -_wallDirX * wallJumpForce.x;
            _frameVelocity.y = wallJumpForce.y;
            _airJumpsUsed = 0;
            _wallJumpsUsed++;

            Jumped?.Invoke();
        }

        private void CalculateWallMechanics()
        {
            bool touchingWall = _colLeft || _colRight;
            if (touchingWall) _wallDirX = _colRight ? 1 : -1;

            if (_wallJumpTimer > 0) _wallJumpTimer -= Time.deltaTime;

            // Sekarang wall jump cuma boleh kalau jatah (_wallJumpsUsed) belum habis
            _canWallJump = touchingWall && !_colDown && _wallJumpTimer <= 0 && _wallJumpsUsed < maxWallJumps;

            if (_canWallJump && _frameVelocity.y < 0)
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
        }


        [Header("GRAPPLING HOOK")]
        public float grappleRange = 8f;
        public LayerMask grappleLayer;

        [Header("Swing Physics")]
        public float swingBoost = 2.5f;       
        public float swingCooldown = 0.25f;   
        public float grappleDrag = 0.15f;
        public float maxSwingSpeed = 10f;
        private bool _boostUsedThisSwing = false;
        private float _previousY;

        public LineRenderer grappleLine;

        private DistanceJoint2D _distanceJoint;
        private bool _isGrappling;
        private Transform _currentGrapplePoint;
        private float _originalDrag;
        private float _lastSwingTime;

        private void CalculateGrapple()
        {
            if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(1))
            {
                Transform nearestPoint = FindNearestGrapplePoint();

                if (nearestPoint != null)
                {
                    _isGrappling = true;
                    _currentGrapplePoint = nearestPoint;

                    _distanceJoint.connectedAnchor = nearestPoint.position;
                    _distanceJoint.enabled = true;

                    _originalDrag = _rb.drag;
                    _rb.drag = grappleDrag;

                    Vector2 radialDir =
                        ((Vector2)transform.position -
                        (Vector2)nearestPoint.position).normalized;

                    Vector2 radialVel =
                        Vector2.Dot(_rb.velocity, radialDir) * radialDir;

                    Vector2 tangentialVel =
                        _rb.velocity - radialVel;

                    _rb.velocity = tangentialVel;

                    _lastSwingTime = Time.time;

                    if (grappleLine != null)
                        grappleLine.enabled = true;
                }
            }


            else if (Input.GetKeyUp(KeyCode.E) ||
                    Input.GetMouseButtonUp(1))
            {
                if (_isGrappling)
                {
                    _isGrappling = false;

                    _distanceJoint.enabled = false;
                    _currentGrapplePoint = null;

                    _rb.drag = _originalDrag;

                    if (grappleLine != null)
                        grappleLine.enabled = false;

                    _frameVelocity = _rb.velocity;
                }
            }


            if (_isGrappling && _currentGrapplePoint != null)
            {
                Vector2 radialDir =
                    ((Vector2)transform.position -
                    (Vector2)_currentGrapplePoint.position).normalized;

                // Arah gerakan yang tegak lurus terhadap tali
                Vector2 tangentDir =
                    new Vector2(-radialDir.y, radialDir.x);

                float inputX = 0f;

                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                {
                    inputX = -1f;
                }
                else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                {
                    inputX = 1f;
                }


                bool isFalling = _rb.velocity.y < 0f;
                // Player berada di bawah titik grapple
                bool isBelowHook = transform.position.y < _currentGrapplePoint.position.y;

                if (isBelowHook && isFalling)
                {
                    _boostUsedThisSwing = false;
                }

                if (inputX != 0f &&
                    isFalling &&
                    isBelowHook &&
                    !_boostUsedThisSwing)
                {
                    // Tentukan arah tangent
                    float tangentDirection =
                        Mathf.Sign(tangentDir.x);

                    // Dorongan mengikuti arah swing yang dipilih
                    if (Mathf.Sign(inputX) == tangentDirection)
                    {
                        _rb.AddForce(
                            tangentDir * swingBoost,
                            ForceMode2D.Impulse
                        );

                        _boostUsedThisSwing = true;
                    }
                }

                if (_rb.velocity.magnitude > maxSwingSpeed)
                {
                    _rb.velocity =
                        _rb.velocity.normalized * maxSwingSpeed;
                }

                if (grappleLine != null)
                {
                    grappleLine.SetPosition(0, transform.position);
                    grappleLine.SetPosition(
                        1,
                        _currentGrapplePoint.position
                    );
                }
            }
        }

        Transform FindNearestGrapplePoint()
        {
            Collider2D[] candidates = Physics2D.OverlapCircleAll(transform.position, grappleRange, grappleLayer);

            if (candidates.Length == 0) return null;

            Transform nearest = null;
            float shortestDistance = float.MaxValue;

            foreach (Collider2D candidate in candidates)
            {
                float dist = Vector2.Distance(transform.position, candidate.transform.position);
                if (dist < shortestDistance)
                {
                    shortestDistance = dist;
                    nearest = candidate.transform;
                }
            }

            return nearest;
        }

        #endregion

        #region Horizontal

        private void HandleDirection()
        {
            if (_wallJumpControlLockTimer > 0)
            {
                _wallJumpControlLockTimer -= Time.fixedDeltaTime;
                return; // selama lock singkat ini, input diabaikan biar dorongan awal kerasa dulu
            }

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

                float facingDir = _frameVelocity.x != 0
                    ? Mathf.Sign(_frameVelocity.x)
                    : transform.localScale.x;

                if (Input.GetAxisRaw("Horizontal") != 0)
                    facingDir = Mathf.Sign(Input.GetAxisRaw("Horizontal"));

                _frameVelocity.x = dashSpeed * facingDir;
                _frameVelocity.y = 0;
                _rb.velocity = _frameVelocity;
                Debug.Log("DASH START!");

                Collider2D nearEnemy = Physics2D.OverlapCircle(transform.position, 2f, LayerMask.GetMask("Enemy"));
                if (nearEnemy != null)
                {
                    GameJuiceManager.instance.TriggerSlowMotion(0.2f, 0.5f);
                    GameJuiceManager.instance.ShakeCamera(2f, 0.2f);
                }
            }

            if (_isDashing)
            {
                _dashTimeLeft -= Time.deltaTime;

                float facingDir = _frameVelocity.x != 0 ? Mathf.Sign(_frameVelocity.x) : transform.localScale.x;
                if (Input.GetAxisRaw("Horizontal") != 0) facingDir = Mathf.Sign(Input.GetAxisRaw("Horizontal"));

                _frameVelocity.y = 0;
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