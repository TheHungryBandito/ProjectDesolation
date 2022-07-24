using System;
using System.Collections.Generic;
using UnityEngine;

public class ShootAction : BaseAction
{
    public static event EventHandler<OnShootEventArgs> OnAnyShoot;
    public static event EventHandler OnAnyShootEnded;
    public event EventHandler<OnShootEventArgs> OnShoot;
     
    public class OnShootEventArgs : EventArgs
    {
        public Unit targetUnit;
        public Unit attackingUnit;
    }

    private enum State
    {
        Aiming,
        Shooting,
        Cooloff,
    }

    [SerializeField] private GridVisualTypeSO secondaryGridVisualTypeSO;
    [SerializeField] private float damage = 40;
    private float originalDamage;
    private int maxShootDistance = 5;
    private int minShootDistance = 2;
    private int turnsUntilDamageReset = 0;
    private float stateTimer;
    private Unit targetUnit;
    private bool canShootBullet;
    [SerializeField] private LayerMask obstaclesLayerMask;

    private State state;
    private void Start()
    {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        originalDamage = damage;
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if (turnsUntilDamageReset == 0)
        {
            return;
        }

        turnsUntilDamageReset -= 1;

        if (turnsUntilDamageReset == 0)
        {
            damage = originalDamage;
        }
    }

    private void Update()
    {
        if (!isActive) 
        { 
            return; 
        }

        stateTimer -= Time.deltaTime;
        switch (state)
        {
            case State.Aiming:
                Aim();
                break;
            case State.Shooting:
                if (canShootBullet)
                {
                    ActionQueueHandler.Instance.AddActionToQueue(Shoot);
                    canShootBullet = false;
                }
                break;
            case State.Cooloff:
                break;
        }
        if (stateTimer <= 0f)
        {
            NextState();
        }
    }

    private void NextState()
    {
        switch (state)
        {
            case State.Aiming:
                state = State.Shooting;
                float shootingStateTime = 0.1f;
                stateTimer = shootingStateTime;
                break;
            case State.Shooting:
                state = State.Cooloff;
                float cooloffStateTime = 0.5f;
                stateTimer = cooloffStateTime;
                break;
            case State.Cooloff:
                OnAnyShootEnded?.Invoke(this, EventArgs.Empty);
                ActionComplete();
                break;
        }
    }

    private void Aim()
    {
        Vector3 aimDirection = (targetUnit.GetWorldPosition() - transform.position).normalized;
        Quaternion quatTargetRotation = Quaternion.FromToRotation(Vector3.up, aimDirection);

        float rotateSpeed = 10f;
        transform.rotation = Quaternion.Slerp(transform.rotation, quatTargetRotation, Time.deltaTime * rotateSpeed);
    }
    private void Shoot()
    {
        OnAnyShoot?.Invoke(this, new OnShootEventArgs
        {
            targetUnit = targetUnit,
            attackingUnit = unit
        });

        OnShoot?.Invoke(this, new OnShootEventArgs
        {
            targetUnit = targetUnit,
            attackingUnit = unit
        });


        targetUnit.Damage(damage, unit.transform);
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        GridPosition unitGridPosition = unit.GetGridPosition();
        return GetValidActionGridPositionList(unitGridPosition);
    }

    public List<GridPosition> GetValidActionGridPositionList(GridPosition unitGridPosition)
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        for (int x = -maxShootDistance; x <= maxShootDistance; x++)
        {
            for (int y = -maxShootDistance; y <= maxShootDistance; y++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, y);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                int testDistance = (Mathf.Abs(x) * Mathf.Abs(x)) + (Mathf.Abs(y) * Mathf.Abs(y));
                if(testDistance > (maxShootDistance * maxShootDistance))
                {
                    continue;
                }
                if(testDistance < (minShootDistance * minShootDistance))
                {
                    continue;
                }

                if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    //No Unit at position
                    continue;
                }
                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                if(targetUnit.IsEnemy() == unit.IsEnemy())
                {
                    //Both units are on the same "Team"
                    continue;
                }

                Vector3 unitWorldPosition = LevelGrid.Instance.GetWorldPosition(unitGridPosition);
                Vector3 shootDirection = (targetUnit.GetWorldPosition() - unitWorldPosition).normalized;
                float unitShoulderHeight = 1.7f;
                if (Physics.Raycast(unitWorldPosition + -Vector3.forward * unitShoulderHeight,
                    shootDirection,
                    Vector3.Distance(unitWorldPosition, targetUnit.GetWorldPosition()), 
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

        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

        state = State.Aiming;
        float aimingStateTime = 1f;
        stateTimer = aimingStateTime;

        canShootBullet = true;

        ActionStart(onActionComplete);

    }

    public void ModifyDamage(float damage, int turnsUntilDamageReset)
    {
        this.damage = damage;
        this.turnsUntilDamageReset = turnsUntilDamageReset;
    }

    public override string GetActionName()
    {
        return "Shoot";
    }
    public override int GetActionPointsCost()
    {
        if (unit.GetActionPoints() > 0)
        {
            return unit.GetActionPoints();
        }
        else
        {
            return 1;
        }
    }
    public Unit GetTargetUnit()
    {
        return targetUnit;
    }
    public int GetMaxShootDistance()
    {
        return maxShootDistance;
    }
    public int GetMinShootDistance()
    {
        return minShootDistance;
    }
    public float GetOriginalShootDamage()
    {
        return originalDamage;
    }
    public float GetShootDamage()
    {
        return damage;
    }
    public GridVisualTypeSO GetSecondaryGridVisualTypeSO()
    {
        return secondaryGridVisualTypeSO;
    }
    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 100 + Mathf.RoundToInt((1 - targetUnit.GetHealthNormalized()) * 100f),
        };
    }
    public int GetTargetCountAtPosition(GridPosition gridPosition)
    {
        return GetValidActionGridPositionList(gridPosition).Count;
    }

}
