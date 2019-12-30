using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]

public class ControllersParameters2D
{
    public enum JumpBehavior
    {
        CanJumpOnGround,
        CanJumpAnywhere,
        CantJump
    }

    public Vector2 MaxVelocity = new Vector2(float.MaxValue, float.MaxValue);

    [Range(0, 90)]
    public float SlopeLimit = 30;

    public float Gravity = -25f;

    public JumpBehavior JumpRestrictions;

    public float JumpFrequency = .25f;

    public bool IsGrounded { get; internal set; }

    public float JumpMagnitude = 12;
}
