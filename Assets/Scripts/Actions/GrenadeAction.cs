using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeAction : BaseAction
{

    [SerializeField] private GridVisualTypeSO secondaryGridVisualTypeSO;
    [SerializeField] private Transform grenadeProjectilePrefab;
    [SerializeField] private LayerMask obstaclesLayerMask;
    private int maxThrowDistance = 7;
    private int minThrowDistance = 2;

    public override string GetActionName()
    {
        return "Bomb";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        if (targetUnit != null && targetUnit.IsEnemy() != unit.IsEnemy())
        {
            return new EnemyAIAction
            {
                gridPosition = gridPosition,
                actionValue = 100 + Mathf.RoundToInt((1 - targetUnit.GetHealthNormalized()) * 100f),
            };
        }
        else
        {
            return new EnemyAIAction
            {
                gridPosition = gridPosition,
                actionValue = 0,
            };
        }
    }
    public override List<GridPosition> GetValidActionGridPositionList()
    {
        GridPosition unitGridPosition = unit.GetGridPosition();
        return GetValidActionGridPositionList(unitGridPosition);
    }

    public List<GridPosition> GetValidActionGridPositionList(GridPosition unitGridPosition)
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        for (int x = -maxThrowDistance; x <= maxThrowDistance; x++)
        {
            for (int y = -maxThrowDistance; y <= maxThrowDistance; y++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, y);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                int testDistance = (Mathf.Abs(x) * Mathf.Abs(x)) + (Mathf.Abs(y) * Mathf.Abs(y));
                if (testDistance > (maxThrowDistance * maxThrowDistance))
                {
                    continue;
                }
                if (testDistance < (minThrowDistance * minThrowDistance))
                {
                    continue;
                }
                if(!Pathfinding.Instance.IsWalkableGridPosition(testGridPosition))
                {
                    continue;
                }
                Vector3 unitWorldPosition = LevelGrid.Instance.GetWorldPosition(unitGridPosition);
                Vector3 testWorldPosition = LevelGrid.Instance.GetWorldPosition(testGridPosition);
                Vector3 shootDirection = (testWorldPosition - unitWorldPosition).normalized;
                float unitShoulderHeight = 1.7f;
                if (Physics.Raycast(unitWorldPosition + -Vector3.forward * unitShoulderHeight,
                    shootDirection,
                    Vector3.Distance(unitWorldPosition, testWorldPosition),
                    obstaclesLayerMask))
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
        Debug.Log("Taking Grenade Action");

        Transform grenadeProjectileTransform = Instantiate(grenadeProjectilePrefab, unit.GetWorldPosition(), Quaternion.identity);
        GrenadeProjectile grenadeProjectile = grenadeProjectileTransform.GetComponent<GrenadeProjectile>();
        grenadeProjectile.Setup(gridPosition, OnGrenadeBehaviourComplete);

        ActionStart(onActionComplete);
    }

    private void OnGrenadeBehaviourComplete()
    {
        ActionComplete();
    }

    public int GetMaxThrowDistance()
    {
        return maxThrowDistance;
    }
    public int GetMinThrowDistance()
    {
        return minThrowDistance;
    }
    public override int GetActionPointsCost()
    {
        return 2;
    }

    public GridVisualTypeSO GetSecondaryGridVisualTypeSO()
    {
        return secondaryGridVisualTypeSO;
    }
}
