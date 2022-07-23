using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverwatchAction : BaseAction
{
    public static event EventHandler<OnOverwatchTriggeredArgs> OnAnyOverwatchTriggered;
    public static event EventHandler OnAnyOverwatchEnded;

    public event EventHandler<OnOverwatchTriggeredArgs> OnOverwatchShoot;

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
    private Unit targetUnit;
    private ShootAction shootAction;
    private List<GridObject> overwatchedGridObjects;
    private int maxShootDistance;
    private int minShootDistance;
    private float actionTimer;
    private float stateTimer;
    private bool canShootBullet;
    private State state;


    [SerializeField] private LayerMask obstaclesLayerMask;

    private void Start()
    {
        shootAction = unit.GetAction<ShootAction>();
        maxShootDistance = shootAction.GetMaxShootDistance();
        minShootDistance = shootAction.GetMinShootDistance();
        state = State.Aiming;
    }

    private void Update()
    {
        if (targetUnit != null)
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
            gridObject.OnUnitAdded += GridObject_OnUnitAdded;
            overwatchedGridObjects.Add(gridObject);
        }
    }
    private void EndOverwatch()
    {
        targetUnit = null;
        foreach (GridObject gridObject in overwatchedGridObjects)
        {
            gridObject.OnUnitAdded -= GridObject_OnUnitAdded;
        }
        overwatchedGridObjects.Clear();

        OnAnyOverwatchEnded?.Invoke(this, EventArgs.Empty);
    }

    private void GridObject_OnUnitAdded(object sender, GridObject.OnUnitAddedEventArgs e)
    {
        Debug.Log($"Unit at this {sender as GridObject}");
        if(e.unit.IsEnemy() == unit.IsEnemy())
        {
            return;
        }
        targetUnit = e.unit;

        ActionQueueHandler.Instance.AddActionToQueue(TriggerOverwatch);
    }

    private void TriggerOverwatch()
    {
        if(targetUnit == null)
        {
            OnAnyOverwatchEnded?.Invoke(this, EventArgs.Empty);
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
    }

    private void Shoot()
    {
        OnOverwatchShoot?.Invoke(this, new OnOverwatchTriggeredArgs
        {
            targetUnit = targetUnit,
            attackingUnit = unit
        });

        targetUnit.Damage(40, unit.transform);
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
        return new EnemyAIAction()
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
            return base.GetActionPointsCost();
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
}

