using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class LineExt
{
    public static void DrawTrajectory(this LineRenderer LineRend, Rigidbody2D body, Vector2 initialVelocity, int StepNumbers, float secondsToShow = 1)
    {
        float timeDelta = secondsToShow / initialVelocity.magnitude; // for example

        LineRend.positionCount = StepNumbers;

        Vector2 position = body.position;
        Vector2 velocity = initialVelocity;
        Vector2 gravity = Physics2D.gravity * body.gravityScale;
        for (int i = 0; i < StepNumbers; ++i)
        {
            LineRend.SetPosition(i, position);

            position += velocity * timeDelta + 0.5f * gravity * timeDelta * timeDelta;
            velocity += gravity * timeDelta;
            velocity *= Mathf.Clamp01(1f - body.drag * timeDelta);
        }
    }
}
