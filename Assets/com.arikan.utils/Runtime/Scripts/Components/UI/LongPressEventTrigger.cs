using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class LongPressEventTrigger : UIBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [Tooltip("How long must pointer be down on this object to trigger a long press")]
    public float durationThreshold = 1.0f;

    public UnityEvent onLongPress = new UnityEvent();

    private bool isPointerDown = false;
    private bool longPressTriggered = false;
    private float timePressStarted;
    private int lastPtrDownIndex;

    public void OnPointerDown(PointerEventData eventData)
    {
        timePressStarted = Time.unscaledTime;
        isPointerDown = true;
        longPressTriggered = false;
        lastPtrDownIndex = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        StartCoroutine(WaitUntilTimesUp(lastPtrDownIndex));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPointerDown = false;
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerDown = false;
    }

    IEnumerator WaitUntilTimesUp(int clickIndex)
    {
        yield return new WaitForSecondsRealtime(durationThreshold);

        if (clickIndex == lastPtrDownIndex && isPointerDown && !longPressTriggered)
        {
            if (Time.unscaledTime - timePressStarted > durationThreshold)
            {
                longPressTriggered = true;
                onLongPress.Invoke();
            }
        }
    }
}