using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CharacterController2D : MonoBehaviour
{
    private const float SkinWidth = .02f;
    private const int TotalHorizontalRays = 8;
    private const int TotalVerticalRays = 4;

    private static readonly float SlopeLimitTangant = Mathf.Tan(75f * Mathf.Deg2Rad);

    public LayerMask PlatforMask;
    public ControllersParameters2D DefaultParameters;

    public ControllerState2D State { get; private set; }
    public Vector2 Velocity { get { return _velocity} }
    public bool CantJump { get { return false; } }
    public bool HandleCollisions { get; set; }
    public ControllersParameters2D Parameters { get { return _overrideParameters ?? DefaultParameters; } }

    private Vector2 _velocity;
    private Transform _transform;
    private Vector3 _localScale;
    private BoxCollider2D _boxCollider;
    private ControllersParameters2D _overrideParameters;

    private float 
        _verticalDistanceBetweenRays,
        _horizontalDistanceBetweenRays;

    public void Awake()
    {
      // State = new ControllersParameters2D();
        State = new ControllerState2D();
        _transform = transform;
        _localScale = transform.localScale;
        _boxCollider = GetComponent<BoxCollider2D>();

        var colliderWidth = _boxCollider.size.x * Mathf.Abs(transform.localScale.x) - (2 * SkinWidth); 
        _horizontalDistanceBetweenRays = colliderWidth / (TotalVerticalRays - 1);

        var colliderHeight = _boxCollider.size.y * Mathf.Abs(transform.localScale.y) - (2 * SkinWidth);
        _verticalDistanceBetweenRays = colliderHeight / (TotalHorizontalRays - 1);
    }

   
    public void AddForce(Vector2 force)
    {
        _velocity = force;
    }
  
    public void SetForce(Vector2 force)
    {
        _velocity += force;
    }
   
    public void SetHorizontalForce(float x)
    {
        _velocity.x = x;
    }
  
    public void SetVerticalForce(float y)
    {
        _velocity.y = y;
    }
   
    public void Jump()
    {

    }
  
    public void LateUpdate()
    {
        Move(Velocity * Time.deltaTime);
    }
    private void Move(Vector2 deltaMovement)
    {
        var wasGrounded = State.IsCollidingBelow;
        State.Reset();

        if (HandleCollisions)
        {
            HandlePlatforms();
            CalculateRayOrigins();

            if (deltaMovement.y < 0 && wasGrounded)
                HandeleVerticalSlope(ref deltaMovement);

            if (Mathf.Abs(deltaMovement.x) > .001f)
                MoveHorizontally(ref deltaMovement);

            MoveVertically(ref deltaMovement);
        }

        _transform.Translate(deltaMovement, Space.World);

        //ToDo: moving platform code.

        if (Time.deltaTime > 0)
            _velocity = deltaMovement / Time.deltaTime;

        _velocity.x = Mathf.Min(_velocity.x, Parameters.MaxVelocity.x);
        _velocity.y = Mathf.Min(_velocity.y, Parameters.MaxVelocity.y);

        if (State.IsMovingUpSlope)
            _velocity = 0;
        
    }

    private void HandlePlatforms()
    {

    }
  
    private void CalculateRayOrigins()
    {

    }
  
    private void MoveHorizontally(ref Vector2 deltaMovement)
    {

    }

    private void MoveVertically (ref Vector2 deltaMovement)
    {

    }

    private void HandeleVerticalSlope(ref Vector2 deltaMovement)
    {

    }

    private void HandeleHorizontalSlope(ref Vector2 deltaMovement, float angel, bool isGoingRight)
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        
    }


}
