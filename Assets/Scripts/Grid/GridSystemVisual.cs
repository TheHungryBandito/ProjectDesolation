using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{
    public static GridSystemVisual Instance { get; private set; }


    [SerializeField] private Transform gridSystemVisualSinglePrefab;
    [SerializeField] private List<GridVisualTypeSO> gridVisualTypeSOList;

    private GridSystemVisualSingle[,] gridSystemVisualSingleArray;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one GridSystemVisual! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        int width = LevelGrid.Instance.GetWidth();
        int height = LevelGrid.Instance.GetHeight();
        gridSystemVisualSingleArray = new GridSystemVisualSingle[width, height];

        for (int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                GridPosition gridPosition = new GridPosition(x, y);
                Transform gridSystemVisualSingleTransform = Instantiate(gridSystemVisualSinglePrefab, 
                    LevelGrid.Instance.GetWorldPosition(gridPosition), 
                    Quaternion.identity);

                gridSystemVisualSingleArray[x, y] = gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();

            }
        }

        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
        LevelGrid.Instance.OnAnyUnitMovedGridPosition += LevelGrid_OnAnyUnitMovedGridPosition;
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;
        PathfindingUpdater.Instance.OnPathFindingUpdated += PathfindingUpdater_OnPathFindingUpdated;
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;

        UpdateGridVisual();
    }

    public void HideAllGridPositions()
    {
        int width = LevelGrid.Instance.GetWidth();
        int height = LevelGrid.Instance.GetHeight();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                gridSystemVisualSingleArray[x, y].Hide();
            }
        }
    }

    private void ShowGridPositionRangeCircle(GridPosition gridPosition, int maxRange, int minRange, GridVisualTypeSO gridVisualTypeSO)
    {
        List<GridPosition> gridPositionList = new List<GridPosition>();

        for(int x = -maxRange; x <= maxRange; x++)
        {
            for(int y = -maxRange; y <= maxRange; y++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, y);
                GridPosition testGridPosition = gridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                int testDistance = (Mathf.Abs(x) * Mathf.Abs(x)) + (Mathf.Abs(y) * Mathf.Abs(y));
                if (testDistance > (maxRange * maxRange))
                {
                    continue;
                }
                if (testDistance < (minRange * minRange))
                {
                    continue;
                }

                gridPositionList.Add(testGridPosition);

                ShowGridPositionList(gridPositionList, gridVisualTypeSO);
            }
        }
    }

    private void ShowGridPositionRangeSquare(GridPosition gridPosition, int maxRange, int minRange, GridVisualTypeSO gridVisualTypeSO)
    {
        List<GridPosition> gridPositionList = new List<GridPosition>();

        for (int x = -maxRange; x <= maxRange; x++)
        {
            for (int y = -maxRange; y <= maxRange; y++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, y);
                GridPosition testGridPosition = gridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                int testDistance = Mathf.Abs(x) + Mathf.Abs(y);

                if (testDistance < minRange)
                {
                    continue;
                }
                gridPositionList.Add(testGridPosition);

                ShowGridPositionList(gridPositionList, gridVisualTypeSO);
            }
        }
    }

    public void ShowGridPositionList(List<GridPosition> gridPositions, GridVisualTypeSO gridVisualTypeSO)
    {
        for (int i = 0; i < gridPositions.Count; i++)
        {
            gridSystemVisualSingleArray[gridPositions[i].x, gridPositions[i].y].Show(gridVisualTypeSO.GetMaterial());
        }
    }

    private void UpdateGridVisual()
    {
        HideAllGridPositions();

        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();

        if (selectedAction != null)
        {
            GridVisualTypeSO gridVisualTypeSO = selectedAction.GetGridVisualTypeSO();

            switch(selectedAction)
            {
                case ShootAction shootAction:
                    ShowGridPositionRangeCircle(selectedUnit.GetGridPosition(), 
                        shootAction.GetMaxShootDistance(), 
                        shootAction.GetMinShootDistance(), 
                        shootAction.GetSecondaryGridVisualTypeSO());
                    break;
                case GrenadeAction grenadeAction:
                    ShowGridPositionRangeCircle(selectedUnit.GetGridPosition(),
                        grenadeAction.GetMaxThrowDistance(),
                        grenadeAction.GetMinThrowDistance(),
                        grenadeAction.GetSecondaryGridVisualTypeSO());
                    break;
                case MeleeAction meleeAction:
                    ShowGridPositionRangeSquare(selectedUnit.GetGridPosition(),
                        meleeAction.GetMaxMeleeDistance(),
                        meleeAction.GetMinMeleeDistance(),
                        meleeAction.GetSecondaryGridVisualTypeSO());
                    break;
                case OverwatchAction overwatchAction:
                    ShowGridPositionRangeCircle(selectedUnit.GetGridPosition(),
                        overwatchAction.GetMaxShootDistance(),
                        overwatchAction.GetMinShootDistance(),
                        overwatchAction.GetSecondaryGridVisualTypeSO());
                    break;
                default:
                    break;
            }

            ShowGridPositionList(
                selectedAction.GetValidActionGridPositionList(), 
                gridVisualTypeSO);
        }
    }
    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }
    private void LevelGrid_OnAnyUnitMovedGridPosition(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }
    private void Unit_OnAnyUnitDead(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }
    private void PathfindingUpdater_OnPathFindingUpdated(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }
    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }
}
