using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractAction : BaseAction
{

    private int maxInteractDistance = 1;

    private void Update()
    {
        if(!isActive)
        {
            return;
        }
    }
    public override string GetActionName()
    {
        return "Interact";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            actionValue = 0,
            gridPosition = gridPosition,
        };
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        GridPosition unitGridPosition = unit.GetGridPosition();
        return GetValidActionGridPositionList(unitGridPosition);
    }

    public List<GridPosition> GetValidActionGridPositionList(GridPosition unitGridPosition)
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        for (int x = -maxInteractDistance; x <= maxInteractDistance; x++)
        {
            for (int z = -maxInteractDistance; z <= maxInteractDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }
                if(LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    continue;
                }
                if(LevelGrid.Instance.GetInteractableAtGridPosition(testGridPosition) == null)
                {
                    continue;
                }
                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        Debug.Log("Taking Interact Action");
        IInteractable interactable = LevelGrid.Instance.GetInteractableAtGridPosition(gridPosition);
        interactable.Interact(OnInteractionComplete);
        ActionStart(onActionComplete);
    }

    private void OnInteractionComplete()
    {
        ActionComplete();
    }
}
