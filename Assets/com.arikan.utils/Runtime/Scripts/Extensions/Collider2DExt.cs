using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static partial class ArikanExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="lookingObj"></param>
    /// <param name="target"></param>
    /// <param name="layerMaskValue"></param>
    /// <returns></returns>
    public static bool CanSee(this Collider2D lookingObj, GameObject target, int layerMaskValue = 1)
    {
        Vector3 toTargetDirection = (target.transform.position - lookingObj.transform.position).normalized;

        float colliderOutsideMagnitude = lookingObj.bounds.size.magnitude / 2 + 0.001f;

        var hit = Physics2D.Raycast(
            lookingObj.transform.position + toTargetDirection * colliderOutsideMagnitude,
            toTargetDirection,
            (target.transform.position - lookingObj.transform.position).magnitude,
            layerMaskValue);

        // bir şeye değdiyse ve kendisi deilse
        return (hit.collider != null && hit.collider.gameObject == target);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="lookingObj"></param>
    /// <param name="target"> Target GameObject </param>
    /// <param name="colliderOutsideMagnitude">this.bounds.size.magnitude / 2 + 0.001f</param>
    /// <param name="layerMaskValue"> must be LayerMask.value </param>
    /// <returns></returns>
    public static bool CanSee(this Collider2D lookingObj, GameObject target,float colliderOutsideMagnitude, int layerMaskValue = 1)
    {
        Vector3 toTargetDirection = (target.transform.position - lookingObj.transform.position).normalized;

        var hit = Physics2D.Raycast(
            lookingObj.transform.position + toTargetDirection * colliderOutsideMagnitude,
            toTargetDirection,
            (target.transform.position - lookingObj.transform.position).magnitude,
            layerMaskValue);

        // bir şeye değdiyse ve kendisi deilse
        return (hit.collider != null && hit.collider.gameObject == target);
    }

    public static T GetComponent<T>(this Collision2D coll) where T : Component
    {
        return coll.gameObject.GetComponent<T>();
    }

    public static Component GetComponent(this Collision2D coll, Type t)
    {
        return coll.gameObject.GetComponent(t);
    }
}
