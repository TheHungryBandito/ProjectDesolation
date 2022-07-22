using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private const int ACTION_POINTS_MAX = 4;

    public static event EventHandler OnAnyActionPointsChanged;
    public static event EventHandler OnAnyUnitSpawned;
    public static event EventHandler OnAnyUnitDead;

    [SerializeField] private bool isEnemy;

    private GridPosition gridPosition;
    private HealthSystem healthSystem;
    private BaseAction[] baseActionArray;

    private int actionPoints = ACTION_POINTS_MAX;



    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        baseActionArray = GetComponents<BaseAction>();
    }

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);

        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        healthSystem.OnDead += HealthSystem_OnDead;

        OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);
    }

    private void Update()
    {
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if (newGridPosition != gridPosition)
        {
            GridPosition oldGridPosition = gridPosition;
            gridPosition = newGridPosition;

            LevelGrid.Instance.UnitMovedGridPosition(this, oldGridPosition, newGridPosition);
        }
    }

    public TAction GetAction<TAction>() where TAction : BaseAction
    {
        foreach (BaseAction baseAction in baseActionArray)
        {
            if(baseAction is TAction)
            {
                return baseAction as TAction;
            }
        }
        return null;
    }

    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }

    public Vector3 GetWorldPosition()
    {
        return transform.position;
    }

    public BaseAction[] GetBaseActionArray()
    {
        return baseActionArray;
    }

    public bool TrySpendActionPointsToTakeAction(BaseAction baseAction)
    {
        if(CanSpendActionPointsToTakeAction(baseAction))
        {
            SpendActionPoints(baseAction.GetActionPointsCost());
            return true;
        }
        return false;
    }
    public bool CanSpendActionPointsToTakeAction(BaseAction baseAction)
    {
        return actionPoints >= baseAction.GetActionPointsCost();
    }

    private void SpendActionPoints(int amount)
    {
        actionPoints -= amount;
        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public int GetActionPoints()
    {
        return actionPoints;
    }
    private void TurnSystem_OnTurnChanged(object sender, System.EventArgs e)
    {
        if ((IsEnemy() && !TurnSystem.Instance.IsPlayerTurn()) ||
                (!IsEnemy() && TurnSystem.Instance.IsPlayerTurn()))
        {

            actionPoints = ACTION_POINTS_MAX;

            OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
        }
    }


    public bool IsEnemy()
    {
        return isEnemy;
    }

    public void Damage(float damageAmount, Transform attackersTransform)
    {
        healthSystem.Damage(damageAmount, attackersTransform);
    }

    private void HealthSystem_OnDead(object sender, Transform attackersTransform)
    {
        LevelGrid.Instance.RemoveUnitAtGridPosition(this.gridPosition, this);
        Destroy(gameObject);

        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
    }

    public float GetHealthNormalized()
    {
        return healthSystem.GetHealthNormalized();
    }
}
