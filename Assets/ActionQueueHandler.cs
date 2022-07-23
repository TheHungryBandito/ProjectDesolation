using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionQueueHandler : MonoBehaviour
{
    public static ActionQueueHandler Instance { get; private set; }

    private Queue<Action> actionsQueue;
    private bool isActionRunning = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one ActionQueueHandler! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Start()
    {
        actionsQueue = new Queue<Action>();

        OverwatchAction.OnAnyOverwatchEnded += OverwatchAction_OnAnyOverwatchEnded;
    }

    private void Update()
    {
        if(actionsQueue.Count > 0 && isActionRunning == false)
        {
            isActionRunning = true;
            CompleteNextAction();
        }
    }

    public void AddActionToQueue(Action actionToComplete)
    {
        actionsQueue.Enqueue(actionToComplete);
    }

    private void ClearActionQueue()
    {
        actionsQueue.Clear();
    }

    private void CompleteNextAction()
    { 
        Action action = actionsQueue.Dequeue();
        action();
    }
    public void ActionCompleted()
    {
        isActionRunning = false;
    }

    private void OverwatchAction_OnAnyOverwatchEnded(object sender, EventArgs e)
    {
        ActionCompleted();
    }
}
