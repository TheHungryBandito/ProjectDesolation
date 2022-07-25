using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionQueueHandler : MonoBehaviour
{
    public static ActionQueueHandler Instance { get; private set; }

    private Queue<Action> actionsQueue;
    private bool isActionRunning = false;
    private float timer;

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
        timer = 0.1f;
        actionsQueue = new Queue<Action>();

        OverwatchAction.OnAnyOverwatchEnded += OverwatchAction_OnAnyOverwatchEnded;
        ShootAction.OnAnyShootEnded += ShootAction_OnAnyShootEnded;
    }
    private void Update()
    {
        if(actionsQueue.Count == 0)
        {
            return;
        }

        timer -= Time.deltaTime;
        if(timer > 0)
        {
            return;
        }
        if(isActionRunning == false)
        {
            isActionRunning = true;
            CompleteNextAction();
        }
    }
    public void AddActionToQueue(Action actionToComplete)
    {
        Debug.Log($"Queuing {actionToComplete.Method}");
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
        Debug.Log($"Doing {action.Method}");
    }
    private void ActionCompleted()
    {
        timer = 0.1f;
        isActionRunning = false;
    }
    private void OverwatchAction_OnAnyOverwatchEnded(object sender, EventArgs e)
    {
        Debug.Log($"Action ended {sender}");
        ActionCompleted();
    }
    private void ShootAction_OnAnyShootEnded(object sender, EventArgs e)
    {
        Debug.Log($"Action ended {sender}");
        ActionCompleted();
    }
}
