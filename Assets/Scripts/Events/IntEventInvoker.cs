using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Integer event invoker class (Child of MonoBehaviour)
/// </summary>
public class IntEventInvoker : MonoBehaviour
{
    #region Fields
    /// <summary>
    /// Unity events dictionary
    /// </summary>
    protected Dictionary<EventEnum, UnityEvent<int>> unityEvents =
        new Dictionary<EventEnum, UnityEvent<int>>();
    #endregion

    /// <summary>
    /// ads the Listener in respect to the event name
    /// </summary>
    /// <param name="eventName">event enum name</param>
    /// <param name="listener">listner</param>
    public void AddListener(EventEnum eventName, UnityAction<int> listener)
    {
        // only add listeners for supported events (eventName)
        if (unityEvents.ContainsKey(eventName))
        {
            unityEvents[eventName].AddListener(listener);
        }
    }
}
