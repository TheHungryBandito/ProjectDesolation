using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyUpAction : BaseAction
{
    [SerializeField] private float damageModifier = 20;
    private float timer = 0f;
    private ShootAction shootAction;

    private void Start()
    {
        shootAction = unit.GetAction<ShootAction>();
    }
    private void Update()
    {
        if (!isActive)
        {
            return;
        }
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            ActionComplete();
        }
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

        ReadyUp();
    }
    private void ReadyUp()
    {
        float oldShootDamage = shootAction.GetOriginalShootDamage();
        float newShootDamage = oldShootDamage + damageModifier;
        int turnsUntilDamageReset = 1;

        shootAction.ModifyDamage(newShootDamage, turnsUntilDamageReset);
    }

    public override string GetActionName()
    {
        return "ReadyUp!";
    }

    public override int GetActionPointsCost()
    {
        return 2;
    }
}
