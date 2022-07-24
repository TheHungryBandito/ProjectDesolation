using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{
    public event EventHandler OnStartMoving;
    public event EventHandler OnStopMoving;
    [SerializeField] private int maxMoveDistance = 2;

    private List<Vector3> positionList;
    private int currentPositionIndex;

    private void Update()
    {
        if(!isActive) 
        { 
            return;
        }

        Vector3 targetPosition = positionList[currentPositionIndex];
        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        Quaternion quatTargetRotation = Quaternion.FromToRotation(Vector3.up, moveDirection);

        float rotateSpeed = 10f;
        transform.rotation = Quaternion.Slerp(new Quaternion(0, 0, transform.rotation.z, transform.rotation.w), quatTargetRotation, Time.deltaTime * rotateSpeed);
        float stoppingDistance = 0.1f;
        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {
            float moveSpeed = 4f;
            transform.position += moveDirection * (moveSpeed * Time.deltaTime);
        }
        else
        {
            currentPositionIndex++;
            if(currentPositionIndex >= positionList.Count)
            {
                OnStopMoving?.Invoke(this, EventArgs.Empty);
                ActionComplete();
            }
        }

    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        List<GridPosition> pathGridPositionList = Pathfinding.Instance.FindPath(unit.GetGridPosition(), gridPosition, out int pathLength);

        currentPositionIndex = 0;
        positionList = new List<Vector3>();

        foreach (GridPosition pathGridPosition in pathGridPositionList)
        {
            positionList.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition));
        }
        OnStartMoving?.Invoke(this, EventArgs.Empty);

        ActionStart(onActionComplete);

    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();
        for (int x = -maxMoveDistance; x <= maxMoveDistance; x++)
        {
            for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                int testDistance = (Mathf.Abs(x) * Mathf.Abs(x)) + (Mathf.Abs(z) * Mathf.Abs(z));
                if (testDistance > (maxMoveDistance * maxMoveDistance))
                {
                    continue;
                }

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }
                if(unitGridPosition == testGridPosition)
                {
                    continue;
                }
                if(LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    continue;
                }
                if(!Pathfinding.Instance.IsWalkableGridPosition(testGridPosition))
                {
                    continue;
                }
                if (!Pathfinding.Instance.HasPath(unitGridPosition, testGridPosition))
                {
                    continue;
                }

                int pathFindingDistanceMultiplier = 10;
                if (Pathfinding.Instance.GetPathLength(unitGridPosition, testGridPosition) > maxMoveDistance * pathFindingDistanceMultiplier)
                {
                    //Destination Too Far
                    continue;
                }
                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override string GetActionName()
    {
        return "Move";
    }
    public override int GetActionPointsCost()
    {
        return 1;
    }
    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        int targetCountAtGridPosition = unit.GetAction<ShootAction>()
            .GetTargetCountAtPosition(gridPosition);

        if (Mathf.RoundToInt((1 - unit.GetHealthNormalized()) * 100f) > 50f)
        {
            return new EnemyAIAction
            {
                gridPosition = gridPosition,
                actionValue = 200 / (targetCountAtGridPosition + 1)
            };
        }
        else
        {
            return new EnemyAIAction
            {
                gridPosition = gridPosition,
                actionValue = targetCountAtGridPosition * 10
            };
        }
    }
}
