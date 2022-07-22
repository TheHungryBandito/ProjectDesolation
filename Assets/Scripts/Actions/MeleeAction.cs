using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAction : BaseAction
{
    public static event EventHandler<OnMeleeEventArgs> OnAnyMelee;

    public event EventHandler<OnMeleeEventArgs> OnMelee;

    public class OnMeleeEventArgs : EventArgs
    {
        public Unit targetUnit;
        public Unit attackingUnit;
    }

    private enum State
    {
        MeleeBeforeHit,
        MeleeAfterHit,
    }

    [SerializeField] private GridVisualTypeSO secondaryGridVisualTypeSO;
    [SerializeField] private float damage = 40;
    private State state;
    private int maxMeleeDistance = 1;
    private int minMeleeDistance = 1;
    private Unit targetUnit;
    private float stateTimer;



    private void Update()
    {
        if(!isActive)
        {
            return;
        }

        stateTimer -= Time.deltaTime;
        switch (state)
        {
            case State.MeleeBeforeHit:
                Aim();
                break;
            case State.MeleeAfterHit:
                break;
        }
        if (stateTimer <= 0f)
        {
            NextState();
        }
    }

    public void Melee()
    {
        OnAnyMelee?.Invoke(this, new OnMeleeEventArgs
        {
            targetUnit = targetUnit,
            attackingUnit = unit,
        });

        targetUnit.Damage(damage, unit.transform);
    }

    private void NextState()
    {
        switch (state)
        {
            case State.MeleeBeforeHit:
                state = State.MeleeAfterHit;
                float afterHitStateTime = 2f;
                stateTimer = afterHitStateTime;
                break;
            case State.MeleeAfterHit:
                ActionComplete();
                break;
        }
    }
    private void Aim()
    {
        Vector3 aimDir = (targetUnit.GetWorldPosition() - transform.position).normalized;
        float rotateSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, aimDir, Time.deltaTime * rotateSpeed);
    }

    public override string GetActionName()
    {
        return "Melee";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            actionValue = 200,
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

        for (int x = -maxMeleeDistance; x <= maxMeleeDistance; x++)
        {
            for (int z = -maxMeleeDistance; z <= maxMeleeDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    //No Unit at position
                    continue;
                }
                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                if (targetUnit.IsEnemy() == unit.IsEnemy())
                {
                    //Both units are on the same "Team"
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        Debug.Log("Taking Melee Action");

        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        state = State.MeleeBeforeHit;
        float beforeHitStateTime = 0.7f;
        stateTimer = beforeHitStateTime;

        OnMelee?.Invoke(this, new OnMeleeEventArgs
        {
            targetUnit = targetUnit,
            attackingUnit = unit,
        });

        ActionStart(onActionComplete);
    }

    public GridVisualTypeSO GetSecondaryGridVisualTypeSO()
    {
        return secondaryGridVisualTypeSO;
    }

    public int GetMaxMeleeDistance()
    {
        return maxMeleeDistance;
    }

    public int GetMinMeleeDistance()
    {
        return minMeleeDistance;
    }
}
