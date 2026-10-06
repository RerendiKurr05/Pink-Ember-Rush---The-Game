using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator _animator;
    private Controller.PlayerController _player;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _player = GetComponent<Controller.PlayerController>();
        _player.DoubleJumped += OnDoubleJump;
    }

    private void OnDoubleJump()
    {
        _animator.SetTrigger("DoubleJump");
    }
    
    private void Update()
    {
        _animator.SetBool("IsMoving", _player.IsMoving);
        _animator.SetBool("IsGrounded", _player.IsGrounded);
        _animator.SetBool("IsWallSliding", _player.IsWallSliding);
        _animator.SetFloat("VerticalVelocity", _player.VerticalVelocity);
        _animator.SetBool("IsWallJumping", _player.IsWallJumping);
    }

}