using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Events;

public static class UnityEventExt
{
    ///
    /// Gets all the actions in the event as an IEnumerable(UnityAction).
    ///
    public static IEnumerable GetAllActionsAsEnumerable(this UnityEvent e)
    {
        // Loop through all of the actions in the event.
        for (int i = 0; i < e.GetPersistentEventCount(); i++)
        {
            // Get the information about the action.
            MethodInfo actionInfo = UnityEventBase.GetValidMethodInfo(e.GetPersistentTarget(i), e.GetPersistentMethodName(i), new Type[0]);
            // Cast actionInfo into a UnityAction to get the listener.
            UnityAction l = () => { actionInfo.Invoke(e.GetPersistentTarget(i), null); };
            yield return l;
        }
    }
    ///
    /// Gets all the actions in the event as an array of UnityAction.
    ///
    public static UnityAction[] GetAllActions(this UnityEvent e)
    {
        // Make a list of the actions that will be returned at the end.
        var actions = new List<UnityAction>();
        // Get all the actions as an enumerable.
        IEnumerable eventActions = e.GetAllActionsAsEnumerable();
        // Loop through the actions and add them to the actions list.
        foreach (UnityAction currentAction in eventActions)
            actions.Add(currentAction);
        // Return the list as an array.
        return actions.ToArray();
    }
    ///
    /// Gets a specific action at the index of index.
    ///
    public static UnityAction GetAction(this UnityEvent e, int index)
    {
        return e.GetAllActions()[index];
    }
}

