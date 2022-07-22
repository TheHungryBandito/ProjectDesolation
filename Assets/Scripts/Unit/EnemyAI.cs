using System;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{

    private enum State
    {
        WaitingForEnemyTurn,
        TakingTurn,
        Busy
    }

    private State state;

    private float turnTimer;

    private void Awake()
    {
        state = State.WaitingForEnemyTurn;
    }
    private void Start()
    {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }
    private void Update()
    {
        if(TurnSystem.Instance.IsPlayerTurn())
        {
            return;
        }

        switch(state)
        {
            case State.WaitingForEnemyTurn:

                break;
            case State.TakingTurn:
                turnTimer -= Time.deltaTime;
                if (turnTimer <= 0f)
                {
                    if(TryTakeEnemyAIAction(SetStateTakingTurn))
                    {
                        state = State.Busy;
                    }
                    else
                    {
                        TurnSystem.Instance.NextTurn();
                    }
                }
                break;
            case State.Busy:
                break;
        }

        
    }

    private void SetStateTakingTurn()
    {
        state = State.TakingTurn;
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if(TurnSystem.Instance.IsPlayerTurn())
        {
            return;
        }
        state = State.TakingTurn;
        turnTimer = 2f;
    }

    private bool TryTakeEnemyAIAction(Action onEnemyAIActionComplete)
    {
        Debug.Log("Taking Enemy AI Action");
        foreach (Unit enemyUnit in UnitManager.Instance.GetEnemyUnitList())
        {
            if(TryTakeEnemyAIAction(enemyUnit, onEnemyAIActionComplete))
            {
                return true;
            }
        }

        return false;
    }
    private bool TryTakeEnemyAIAction(Unit enemyUnit, Action onEnemyAIActionComplete)
    {
        EnemyAIAction bestEnemyAIAction = null;
        BaseAction bestBaseAction = null;

        foreach (BaseAction baseAction in enemyUnit.GetBaseActionArray())
        {
            if(!enemyUnit.CanSpendActionPointsToTakeAction(baseAction))
            {
                continue;
            }
            if(bestEnemyAIAction == null)
            {
                bestEnemyAIAction = baseAction.GetBestEnemyAIAction();
                bestBaseAction = baseAction;
            } else
            {
                EnemyAIAction testEnemyAction = baseAction.GetBestEnemyAIAction();
                if (testEnemyAction != null && testEnemyAction.actionValue > bestEnemyAIAction.actionValue)
                {
                    bestEnemyAIAction = testEnemyAction;
                    bestBaseAction = baseAction;
                }
            }
        }

        if(bestEnemyAIAction != null && enemyUnit.TrySpendActionPointsToTakeAction(bestBaseAction))
        {
            Debug.Log($"{this.gameObject.name} do {bestBaseAction.GetActionName()}");
            bestBaseAction.TakeAction(bestEnemyAIAction.gridPosition, onEnemyAIActionComplete);
            return true;
        } else
        {
            return false;
        }
    }
}
