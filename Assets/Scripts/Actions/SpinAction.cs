using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAction : BaseAction
{
    private float totalSpinAmount;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Update()
    {
        if (!isActive) { return; }

        float spinAmount = 360f * Time.deltaTime;
        transform.eulerAngles += new Vector3(0, spinAmount, 0);

        totalSpinAmount += spinAmount;

        if (totalSpinAmount >= 360f)
        {
            ActionComplete();
        }
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        totalSpinAmount = 0f;

        ActionStart(onActionComplete);
    }

    public override string GetActionName()
    {
        return "Spin";
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        GridPosition unitGridPosition = unit.GetGridPosition();
        return new List<GridPosition>
        {
            unitGridPosition
        };
    }
    public override int GetActionPointsCost()
    {
        return 1;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 0
        };
    }

}
