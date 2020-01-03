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
    public Vector2 Velocity { get { return _velocity; } }
   // public bool CantJump { get { return false; } }
    public bool HandleCollisions { get; set; }
    public ControllersParameters2D Parameters { get { return _overrideParameters ?? DefaultParameters; } }
    public GameObject StandingOn { get; private set; }
    public Vector3 PlatformVelocity { get; private set; }

    public bool CanJump
    {
        get
        {
            if (Parameters.JumpRestrictions == ControllersParameters2D.JumpBehavior.CanJumpAnywhere)
                return _jumpIn <= 0;

            if (Parameters.JumpRestrictions == ControllersParameters2D.JumpBehavior.CanJumpOnGround)
                return State.IsGrounded;

            return false;
        }
    }

    private Vector2 _velocity;
    private Transform _transform;
    private Vector3 _localScale;
    private BoxCollider2D _boxCollider;
    private ControllersParameters2D _overrideParameters;
    private float _jumpIn;
    private GameObject _lastStendingOn;

    private Vector3
        _activGlobalPlatformPoint,
        _activLocalPlatformPoint;

    private Vector3
        _raycastTopLeft,
        _raycastBottomRight,
        _reycastBottomLeft;

    private float 
        _verticalDistanceBetweenRays,
        _horizontalDistanceBetweenRays;

    public void Awake()
    {
        HandleCollisions = true;
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
        //TODO: moving platforms.
        AddForce(new Vector2 (0, Parameters.JumpMagnitude));
        _jumpIn = Parameters.JumpFrequency;
    }
  
    public void LateUpdate()
    {
        _jumpIn -= Time.deltaTime;
        _velocity.y += Parameters.Gravity * Time.deltaTime;
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

            CorrectHorizontalPlacment(ref deltaMovement, true);
            CorrectHorizontalPlacment(ref deltaMovement, false);
        }

        _transform.Translate(deltaMovement, Space.World);

        if (Time.deltaTime > 0)
            _velocity = deltaMovement / Time.deltaTime;

        _velocity.x = Mathf.Min(_velocity.x, Parameters.MaxVelocity.x);
        _velocity.y = Mathf.Min(_velocity.y, Parameters.MaxVelocity.y);

        if (State.IsMovingUpSlope)
            _velocity.y = 0;

        if (StandingOn != null)
        {
            _activGlobalPlatformPoint = transform.position;
            _activLocalPlatformPoint = StandingOn.transform.InverseTransformPoint(transform.position);

            if (_lastStendingOn != StandingOn)
            {
                if (_lastStendingOn != null)
                    _lastStendingOn.SendMessage("ControllerExit2D", this, SendMessageOptions.DontRequireReceiver);

                StandingOn.SendMessage("ControllerEnter2D", this, SendMessageOptions.DontRequireReceiver);
                _lastStendingOn = StandingOn;
            }
            else if (StandingOn != null)
                StandingOn.SendMessage("ControllerStay2D", this, SendMessageOptions.DontRequireReceiver);
        }
        else if (_lastStendingOn !=null)
        {
            _lastStendingOn.SendMessage("ControllerExxit2D", this, SendMessageOptions.DontRequireReceiver);
            _lastStendingOn = null;
        }
        
    }

    private void HandlePlatforms()
    {
        if (StandingOn != null)
        {
            var newGlobalPlatformPoint = StandingOn.transform.TransformPoint(_activLocalPlatformPoint);
            var moveDistance = newGlobalPlatformPoint - _activGlobalPlatformPoint;

            if (moveDistance != Vector3.zero)
                transform.Translate(moveDistance, Space.World);

            PlatformVelocity = (newGlobalPlatformPoint - _activGlobalPlatformPoint) / Time.deltaTime;
        }
        else
            PlatformVelocity = Vector3.zero;

        StandingOn = null;
    }

    private void CorrectHorizontalPlacment(ref Vector2 deltaMovement, bool isRight)
    {
        var halfWidth = (_boxCollider.size.x * _localScale.x) / 2;
        var rayOrigin = isRight ? _raycastBottomRight : _reycastBottomLeft;

        if (isRight)
            rayOrigin.x -= (halfWidth - SkinWidth);
        else
            rayOrigin.x += (halfWidth - SkinWidth);

        var rayDirection = isRight ? Vector2.right : -Vector2.right;
        var offset = 0f;

        for (var i = 1; i < TotalHorizontalRays - 1; i++)
        {
            var rayVector = new Vector2(deltaMovement.x + rayOrigin.x, deltaMovement.y + rayOrigin.y + (i * _verticalDistanceBetweenRays));
            //            Debug.DrawRay(rayVector, rayDirection * halfWidth, isRight ? Color.cyan : Color.magenta);

            var raycastHit = Physics2D.Raycast(rayVector, rayDirection, halfWidth, PlatforMask);
            if (!raycastHit)
                continue;

            offset = isRight ? ((raycastHit.point.x - _transform.position.x) - halfWidth) : (halfWidth - (_transform.position.x - raycastHit.point.x));
        }

        deltaMovement.x += offset;
    }
  
    private void CalculateRayOrigins()
    {
        var size = new Vector2(_boxCollider.size.x * Mathf.Abs(_localScale.x), _boxCollider.size.y * Mathf.Abs(_localScale.y))/2;
        var center = new Vector2(_boxCollider.offset.x * _localScale.x, _boxCollider.offset.y * _localScale.y);
        //replays center to offset

        _raycastTopLeft = _transform.position + new Vector3(center.x - size.x + SkinWidth, center.y + size.y - SkinWidth);
        _raycastBottomRight = _transform.position + new Vector3(center.x + size.x - SkinWidth, center.y - size.y + SkinWidth);
        _reycastBottomLeft = _transform.position + new Vector3(center.x - size.x + SkinWidth, center.y - size.y + SkinWidth);
    }
  
    private void MoveHorizontally(ref Vector2 deltaMovement)
    {
        var isGoingRight = deltaMovement.x > 0;
        var rayDistance = Mathf.Abs(deltaMovement.x) + SkinWidth;
        var rayDirection = isGoingRight ? Vector2.right : -Vector2.right;
        var rayOrigin = isGoingRight ? _raycastBottomRight : _reycastBottomLeft;

        for (var i=0; i < TotalHorizontalRays; i++)
        {
            var rayVector = new Vector2(rayOrigin.x, rayOrigin.y + (i * _verticalDistanceBetweenRays));
            Debug.DrawRay(rayVector, rayDirection * rayDistance, Color.red);

            var rayCastHit = Physics2D.Raycast(rayVector, rayDirection, rayDistance, PlatforMask);
            if (!rayCastHit)
                continue;

            if (i == 0 && HandeleHorizontalSlope(ref deltaMovement, Vector2.Angle(rayCastHit.normal, Vector2.up), isGoingRight))
                break;

            deltaMovement.x = rayCastHit.point.x - rayVector.x;
            rayDistance = Mathf.Abs(deltaMovement.x);

            if(isGoingRight)
            {
                deltaMovement.x -= SkinWidth;
                State.IsCollidingRight = true;
            }
            else
            {
                deltaMovement.x += SkinWidth;
                State.IsCollidingLeft = true;
            }

            if (rayDistance < SkinWidth + .0001f)
                break;
        }
    }

    private void MoveVertically (ref Vector2 deltaMovement)
    {
       var isGoingUp = deltaMovement.y > 0;
        var rayDistance = Mathf.Abs(deltaMovement.y) + SkinWidth;
        var rayDirection = isGoingUp ? Vector2.up : -Vector2.up;
        var rayOrigin = isGoingUp ? _raycastTopLeft : _reycastBottomLeft;

        rayOrigin.x += deltaMovement.x;

        var standingOnDistance = float.MaxValue;
        for (var i = 0; i < TotalVerticalRays; i++)
        {
            var rayVector=new Vector2(rayOrigin.x + (i * _horizontalDistanceBetweenRays), rayOrigin.y);
            Debug.DrawRay(rayVector, rayDirection * rayDistance, Color.red);

            var raycastHit = Physics2D.Raycast(rayVector, rayDirection, rayDistance, PlatforMask);
            if (!raycastHit)
                continue;

            if (!isGoingUp)
            {
                var verticalDistanceToHit = _transform.position.y - raycastHit.point.y;
                if (verticalDistanceToHit < standingOnDistance)
                {
                    standingOnDistance = verticalDistanceToHit;
                    StandingOn = raycastHit.collider.gameObject;
                }
            }

            deltaMovement.y = raycastHit.point.y - rayVector.y;
            rayDistance = Mathf.Abs(deltaMovement.y);

            if (isGoingUp)
            {
                deltaMovement.y -= SkinWidth;
                State.IsCollidingAbove = true;
            }
            else
            {
                deltaMovement.y += SkinWidth;
                State.IsCollidingBelow = true;
            }
            if (!isGoingUp && deltaMovement.y > .0001f)
                State.IsMovingUpSlope = true;

            if (rayDistance < SkinWidth + .0001f)
                break;
        }
    }
    
    private void HandeleVerticalSlope(ref Vector2 deltaMovement)
    {
        var center = (_reycastBottomLeft.x + _raycastBottomRight.x) / 2;
        var direction = -Vector2.up;

        var slopeDistance = SlopeLimitTangant * (_raycastBottomRight.x - center);
        var SlopeRayVector = new Vector2(center, _reycastBottomLeft.y);

        Debug.DrawRay(SlopeRayVector, direction * slopeDistance, Color.yellow);
        var raycasteHit = Physics2D.Raycast(SlopeRayVector, direction, slopeDistance, PlatforMask);
        if (!raycasteHit)
            return;

        var isMovingDownSlope = Mathf.Sign(raycasteHit.normal.x) == Mathf.Sign(deltaMovement.x);
        if (!isMovingDownSlope)
            return;

        var angel = Vector2.Angle(raycasteHit.normal, Vector2.up);
        if (Mathf.Abs(angel) < .0001f)
            return;

        State.IsMovingDownSlope = true;
        State.SlopeAngle = angel;
        deltaMovement.y = raycasteHit.point.y - SlopeRayVector.y;
    
    }

    private bool HandeleHorizontalSlope(ref Vector2 deltaMovement, float angel, bool isGoingRight)
    {
        if (Mathf.RoundToInt(angel) == 90)
            return false;

        if (angel > Parameters.SlopeLimit)
        {
            deltaMovement.x = 0;
            return true;
        }

        if (deltaMovement.y > .07f)
            return true;

        deltaMovement.x += isGoingRight ? -SkinWidth : SkinWidth;
        deltaMovement.y = Mathf.Abs(Mathf.Tan(angel * Mathf.Deg2Rad) * deltaMovement.x);
        State.IsMovingUpSlope = true;
        State.IsCollidingBelow = true;
        return true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var parameters = other.gameObject.GetComponent<ControllerPhsysicsVolume2D>();
        if (parameters == null)
            return;

        _overrideParameters = parameters.Parameters;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var parametars = other.gameObject.GetComponent<ControllerPhsysicsVolume2D>();
        if (parametars = null)
            return;

        _overrideParameters = null;
    }


}
