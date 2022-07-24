using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvanceAction : BaseAction
{
    private float timer = 0f;
    private MoveAction moveAction;

    private void Start()
    {
        moveAction = unit.GetAction<MoveAction>();
    }

    private void Update()
    {
        if (!isActive)
        {
            return;
        }
        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            ActionComplete();
        }
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 0,
        };
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        GridPosition gridPosition = unit.GetGridPosition();
        return new List<GridPosition>
        {
            gridPosition,
        };
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        float actionTimer = 0.5f;
        timer = actionTimer;

        ActionStart(onActionComplete);

        Advance();
    }

    private void Advance()
    {
        int oldMaxMoveDistance = moveAction.GetOriginalMaxMoveDistance();
        int newMaxMoveDistance = oldMaxMoveDistance * 2;
        int turnsUntilMovementReset = 1;

        moveAction.ModifyMaxMovementDistance(newMaxMoveDistance, turnsUntilMovementReset);
    }
    public override string GetActionName()
    {
        return "Advance";
    }

    public override int GetActionPointsCost()
    {
        return 2;
    }
}
