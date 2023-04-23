using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static partial class ArikanExtensions
{

    public static Bounds OrthographicBounds(this Camera camera)
    {
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float cameraHeight = camera.orthographicSize * 2;
        Bounds bounds = new Bounds(
            camera.transform.position,
            new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
        return bounds;
    }

    /// <summary>
    /// X = Right
    /// Y = Top
    /// Z = Left
    /// W = Bottom
    /// </summary>
    /// <param name="camera"></param>
    /// <returns></returns>
    public static Vector4 RightTopLeftBottom(this Camera camera)
    {
        Bounds b = camera.OrthographicBounds();
        Vector4 v = new Vector4(
            (b.center + b.extents).x,
            (b.center + b.extents).y,
            (b.center - b.extents).x,
            (b.center - b.extents).y);
        return v;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cam"></param>
    /// <returns></returns>
    public static Vector3 ScreenToWorld(this Camera cam)
    {
        Vector3 worldPos = cam.ScreenToWorldPoint(
            Input.touches.Length > 0 ? (Vector3)Input.GetTouch(0).position : Input.mousePosition);
        return worldPos;
    }
    

    public static RaycastHit2D ScreenRay2D(this Camera cam)
    {
        Vector2 worldPos = cam.ScreenToWorldPoint(Input.touches.Length > 0 ? (Vector3)Input.GetTouch(0).position : Input.mousePosition);
        return Physics2D.Raycast(worldPos, Vector2.zero);//0: Default Layer
    }
    public static RaycastHit2D ScreenRay2D(this Camera cam, LayerMask mask)
    {
        Vector2 worldPos = cam.ScreenToWorldPoint(Input.touches.Length > 0 ? (Vector3)Input.GetTouch(0).position : Input.mousePosition);
        return Physics2D.Raycast(worldPos, Vector2.zero, Mathf.Infinity, mask);//0: Default Layer
    }
    public static RaycastHit2D ScreenRay2D(this Camera cam, Vector3 screenCoord)
    {
        Vector2 worldPos = cam.ScreenToWorldPoint(screenCoord);
        return Physics2D.Raycast(worldPos, Vector2.zero);//0: Default Layer
    }
    public static RaycastHit2D ScreenRay2D(this Camera cam, Vector3 screenCoord, LayerMask mask)
    {
        Vector2 worldPos = cam.ScreenToWorldPoint(screenCoord);
        return Physics2D.Raycast(worldPos, Vector2.zero, Mathf.Infinity, mask);//0: Default Layer
    }



    public static RaycastHit2D[] ScreenRayAll2D(this Camera cam)
    {
        Vector2 worldPos = cam.ScreenToWorldPoint(Input.touches.Length > 0 ? (Vector3)Input.GetTouch(0).position : Input.mousePosition);
        return Physics2D.RaycastAll(worldPos, Vector2.zero);//0: Default Layer
    }
    public static RaycastHit2D[] ScreenRayAll2D(this Camera cam, LayerMask mask)
    {
        Vector2 worldPos = cam.ScreenToWorldPoint(Input.touches.Length > 0 ? (Vector3)Input.GetTouch(0).position : Input.mousePosition);
        return Physics2D.RaycastAll(worldPos, Vector2.zero, Mathf.Infinity, mask);//0: Default Layer
    }
    public static RaycastHit2D[] ScreenRayAll2D(this Camera cam, Vector3 screenCoord)
    {
        Vector2 worldPos = cam.ScreenToWorldPoint(screenCoord);
        return Physics2D.RaycastAll(worldPos, Vector2.zero);//0: Default Layer
    }
    public static RaycastHit2D[] ScreenRayAll2D(this Camera cam, Vector3 screenCoord, LayerMask mask)
    {
        Vector2 worldPos = cam.ScreenToWorldPoint(screenCoord);
        return Physics2D.RaycastAll(worldPos, Vector2.zero, Mathf.Infinity, mask);//0: Default Layer
    }
}
