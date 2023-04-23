﻿using UnityEngine;

public class HideFlagsSetter : MonoBehaviour
{
    public HideFlags customHideFlags;

    public enum Mode
    {
        GameObject,
        Component
    }

    public Mode setOn = Mode.GameObject;

    private void OnEnable()
    {
        if (setOn == Mode.GameObject)
        {
            gameObject.hideFlags = customHideFlags;
        }
        else if (setOn == Mode.Component)
        {
            hideFlags = customHideFlags;
        }
    }
}