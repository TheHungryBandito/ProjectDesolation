using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverwatchAction : BaseAction
{
    public static event EventHandler<OnOverwatchTriggeredArgs> OnAnyOverwatchTriggered;
    public static event EventHandler OnAnyOverwatchEnded;

    public event EventHandler<OnOverwatchTriggeredArgs> OnOverwatchShoot;

    /* TODO:
     * When target unit is in range and triggers overwatch but moves out of range
     * the action will not finish and unit will not shoot
     */

    public class OnOverwatchTriggeredArgs : EventArgs
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
    private HealthSystem healthSystem;
    private Unit targetUnit;
    private ShootAction shootAction;
    private List<GridObject> overwatchedGridObjects;
    private float damage;
    private int maxShootDistance;
    private int minShootDistance;
    private float actionTimer;
    private float stateTimer;
    private bool canShootBullet;
    private bool isOverwatching;
    private bool isOverwatchTriggered;
    private State state;


    [SerializeField] private LayerMask obstaclesLayerMask;

    private void Start()
    {
        shootAction = unit.GetAction<ShootAction>();
        healthSystem = GetComponent<HealthSystem>();
        maxShootDistance = shootAction.GetMaxShootDistance();
        minShootDistance = shootAction.GetMinShootDistance();
        damage = shootAction.GetShootDamage();
        state = State.Aiming;

        healthSystem.OnHealthChanged += HealthSystem_OnHealthChanged;
    }

    private void Update()
    {
        if (isOverwatchTriggered && targetUnit != null)
        {
            stateTimer -= Time.deltaTime;
            switch (state)
            {
                case State.Aiming:
                    Aim();
                    break;
                case State.Shooting:
                    if (canShootBullet)
                    {
                        Shoot();
                        canShootBullet = false;
                    }
                    break;
                case State.Cooloff:
                    isOverwatchTriggered = false;
                    break;
            }
            if (stateTimer <= 0f)
            {
                NextState();
            }
        }

        if (!isActive)
        {
            return;
        }

        actionTimer -= Time.deltaTime;

        if(actionTimer <= 0)
        {
            ActionComplete();
        }
    }
    private void NextState()
    {
        switch (state)
        {
            case State.Aiming:
                state = State.Shooting;
                float shootingStateTime = 0.1f * Time.timeScale;
                stateTimer = shootingStateTime;
                break;
            case State.Shooting:
                state = State.Cooloff;
                float cooloffStateTime = 0.5f * Time.timeScale;
                stateTimer = cooloffStateTime;
                break;
            case State.Cooloff:
                break;
        }
    }

    private void Aim()
    {
        Vector3 aimDirection = (targetUnit.GetWorldPosition() - transform.position).normalized;
        Quaternion quatTargetRotation = Quaternion.FromToRotation(Vector3.up, aimDirection);

        float rotateSpeed = 20f;
        transform.rotation = Quaternion.Slerp(transform.rotation, quatTargetRotation, Time.deltaTime * rotateSpeed);
    }

    private void StartOverwatch()
    {
        overwatchedGridObjects = new List<GridObject>();
        GridPosition unitGridPosition = unit.GetGridPosition();
        List<GridPosition> validGridPositions = GetValidActionGridPositionList(unitGridPosition);
        foreach(GridPosition gridPosition in validGridPositions)
        {
            GridObject gridObject = LevelGrid.Instance.GetGridObjectAtGridPosition(gridPosition);
            gridObject.OnUnitChanged += GridObject_OnUnitRemoved;
            overwatchedGridObjects.Add(gridObject);
        }
    }
    private void EndOverwatch()
    {
        targetUnit = null;
        foreach (GridObject gridObject in overwatchedGridObjects)
        {
            gridObject.OnUnitChanged -= GridObject_OnUnitRemoved;
        }
        overwatchedGridObjects.Clear();

        isOverwatching = false;
        OnAnyOverwatchEnded?.Invoke(this, EventArgs.Empty);
    }

    private void GridObject_OnUnitRemoved(object sender, GridObject.OnUnitAddedEventArgs e)
    {
        if(e.unit.IsEnemy() == unit.IsEnemy())
        {
            return;
        }
        targetUnit = e.unit;

        ActionQueueHandler.Instance.AddActionToQueue(TriggerOverwatch);
    }

    private void HealthSystem_OnHealthChanged(object sender, Transform e)
    {
        if (!isOverwatching)
        {
            return;
        }
        Unit targetUnit = e.GetComponent<Unit>();
        this.targetUnit = targetUnit;

        if(healthSystem.GetHealthNormalized() == 0)
        {
            return;
        }
        ActionQueueHandler.Instance.AddActionToQueue(TriggerOverwatch);
    }

    private void TriggerOverwatch()
    {
        Debug.Log("Overwatch Triggered");

        if (targetUnit == null)
        {
            OnAnyOverwatchEnded?.Invoke(this, EventArgs.Empty);
            Debug.Log("Overwatch target null");
            return;
        }
        OnAnyOverwatchTriggered?.Invoke(this, new OnOverwatchTriggeredArgs
        {
            targetUnit = targetUnit,
            attackingUnit = unit
        });

        float aimingStateTime = 1f * Time.timeScale;
        stateTimer = aimingStateTime;
        state = State.Aiming;
        canShootBullet = true;
        isOverwatchTriggered = true;
    }

    private void Shoot()
    {
        damage = shootAction.GetShootDamage();

        OnOverwatchShoot?.Invoke(this, new OnOverwatchTriggeredArgs
        {
            targetUnit = targetUnit,
            attackingUnit = unit
        });

        targetUnit.Damage(damage, unit.transform);
        EndOverwatch();
    }

    public override string GetActionName()
    {
        return "Overwatch";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        int targetCountAtGridPosition = unit.GetAction<ShootAction>()
            .GetTargetCountAtPosition(gridPosition);
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 100 / (1 + targetCountAtGridPosition),
        };
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        GridPosition unitGridPosition = unit.GetGridPosition();
        return new List<GridPosition>
        {
            unitGridPosition
        };
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
                if (testDistance > (maxShootDistance * maxShootDistance))
                {
                    continue;
                }
                if (testDistance < (minShootDistance * minShootDistance))
                {
                    continue;
                }

                Vector3 testWorldPosition = LevelGrid.Instance.GetWorldPosition(testGridPosition);

                Vector3 unitWorldPosition = LevelGrid.Instance.GetWorldPosition(unitGridPosition);
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
        isOverwatching = true;
        isOverwatchTriggered = false;
        ActionStart(onActionComplete);

        StartOverwatch();

    }
    public override int GetActionPointsCost()
    {
        if(unit.GetActionPoints() > 0)
        {
            return unit.GetActionPoints();
        }
        else
        {
            return 1;
        }
    }
    public int GetMaxShootDistance()
    {
        return maxShootDistance;
    }
    public int GetMinShootDistance()
    {
        return minShootDistance;
    }
    public GridVisualTypeSO GetSecondaryGridVisualTypeSO()
    {
        return secondaryGridVisualTypeSO;
    }
    public Unit GetTargetUnit()
    {
        return targetUnit;
    }
}

