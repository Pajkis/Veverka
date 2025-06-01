using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Event manager class
/// </summary>
public static class EventManager 
{

    #region Fields

    /// <summary>
    /// dictionary of events with list of invokers
    /// </summary>
    static Dictionary<EventEnum, List<IntEventInvoker>> invokers =
        new Dictionary<EventEnum, List<IntEventInvoker>>();

    /// <summary>
    /// dictionary of events with list of listeners
    /// </summary>
    static Dictionary<EventEnum, List<UnityAction<int>>> listeners =
        new Dictionary<EventEnum, List<UnityAction<int>>>();

    #endregion

    #region Methods

    /// <summary>
    /// Initialize dictionaries of listeners and invokers
    /// </summary>
    public static void InitEvents()
    {
        //create empty lists for dictionaries
        foreach (EventEnum eventName in Enum.GetValues(typeof(EventEnum)))
        {
            // creates empty lists if game played for the first time
            if (!invokers.ContainsKey(eventName))
            {
                invokers.Add(eventName, new List<IntEventInvoker>());
                listeners.Add(eventName, new List<UnityAction<int>>());
            }
            // clear lists if played more than once
            else
            {
                invokers[eventName].Clear();
                listeners[eventName].Clear();
            }        
        }    
    }

    /// <summary>
    /// Add the given invoker for the event
    /// </summary>
    /// <param name="enumName"> name of the event enum</param>
    /// <param name="invoker">invoker</param>
    public static void AddInvoker(EventEnum eventName, IntEventInvoker invoker)
    {
        // add listeners to the new invoker
        foreach (UnityAction<int> listener in listeners[eventName])
        {
            invoker.AddListener(eventName, listener);
        }
        // add invoker into dictionary of invokers
        invokers[eventName].Add(invoker);
    }

    /// <summary>
    /// Add the given listener for the event
    /// </summary>
    /// <param name="eventName">name of the event enum</param>
    /// <param name="listener">listener</param>
    public static void AddListener(EventEnum eventName, UnityAction<int> listener)
    {
        foreach (IntEventInvoker invoker in invokers[eventName])
        {
            invoker.AddListener(eventName, listener);

        }
        listeners[eventName].Add(listener);
    
    }

    /// <summary>
    /// Remove invoker from dictionary
    /// </summary>
    /// <param name="eventName">name of the event enum</param>
    /// <param name="invoker">invoker  to be removed</param>
    public static void RemoveInvoker(EventEnum eventName, IntEventInvoker invoker)
    {
        invokers[eventName].Remove(invoker);
    }

    #endregion

}
