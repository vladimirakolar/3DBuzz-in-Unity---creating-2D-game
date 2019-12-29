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

    public ControllersParameters2D State { get; private set; }

    public void Awake()
    {
        
    }

   
    public void AddForce(Vector2 force)
    {

    }
  
    public void SetForce(Vector2 force)
    {

    }
   
    public void SetHorizontalForce(float x)
    {

    }
  
    public void SetVerticalForce(float y)
    {

    }
   
    public void Jump()
    {

    }
  
    public void LateUpdate()
    {
        
    }
    private void Move(Vector2 deltaMovement)
 
    {
        
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
