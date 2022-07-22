using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingUpdater : MonoBehaviour
{
    public static PathfindingUpdater Instance { get; private set; }

    public event EventHandler OnPathFindingUpdated;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one PathFindingUpdater! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Start()
    {
        DestructableCrate.OnAnyDestroyed += DestructableCrate_OnAnyDestroyed;
    }

    private void DestructableCrate_OnAnyDestroyed(object sender, EventArgs e)
    {
        DestructableCrate destructableCrate = sender as DestructableCrate;
        Pathfinding.Instance.SetIsWalkableGridPosition(destructableCrate.GetGridPosition(), true);
        OnPathFindingUpdated?.Invoke(this, EventArgs.Empty);
    }
}
