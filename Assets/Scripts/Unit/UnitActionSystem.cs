using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitActionSystem : MonoBehaviour
{
    public static UnitActionSystem Instance { get; private set; }

    public event EventHandler OnSelectedUnitChanged;
    public event EventHandler OnSelectedActionChanged;
    public event EventHandler<bool> OnBusyChanged;
    public event EventHandler OnActionStarted;

    [SerializeField] private Unit selectedUnit;
    [SerializeField] private LayerMask unitLayerMask;

    private BaseAction selectedAction;
    private bool isBusy;

    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogError("There's more than one UnitActionSystem! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if(selectedUnit == null) 
        { 
            return; 
        }
        SetSelectedUnit(selectedUnit);
    }

    private void Update()
    {
        if(isBusy) 
        { 
            return; 
        }
        if(!TurnSystem.Instance.IsPlayerTurn())
        {
            return;
        }
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if (TryHandleUnitSelection()) 
        { 
            return; 
        }

        HandleSelectedAction();
    }

    private void HandleSelectedAction()
    {
        if(InputManager.Instance.IsLeftMouseButtonDownClick())
        {
            if (selectedUnit == null)
            {
                return;
            }

            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetMousePosition());

            if (!selectedAction.IsValidActionGridPosition(mouseGridPosition))
            {
                return;
            }

            if (!selectedUnit.TrySpendActionPointsToTakeAction(selectedAction))
            {
                return;
            }

            SetBusy();
            selectedAction.TakeAction(mouseGridPosition, ClearBusy);

            OnActionStarted?.Invoke(this, EventArgs.Empty);
        }
    }
    private void SetBusy()
    {
        isBusy = true;

        OnBusyChanged?.Invoke(this, isBusy);
    }

    private void ClearBusy()
    {
        isBusy = false;

        OnBusyChanged?.Invoke(this, isBusy);
    }

    private bool TryHandleUnitSelection()
    {
        if (InputManager.Instance.IsLeftMouseButtonDownClick())
        {
            Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
            if (Physics.Raycast(ray, out RaycastHit hitinfo, float.MaxValue, unitLayerMask))
            {
                if (hitinfo.transform.TryGetComponent<Unit>(out Unit unit))
                {
                    if (unit == selectedUnit)
                    {
                        //Unit already selected
                        return false;
                    }
                    if(unit.IsEnemy())
                    {
                        return false;
                    }
                    SetSelectedUnit(unit);
                    return true;
                }
            }
        }       

        return false;
    }

    private void SetSelectedUnit(Unit unit)
    {
        selectedUnit = unit;
        SetSelectedAction(unit.GetAction<MoveAction>());
        OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetSelectedAction(BaseAction baseAction)
    {
        selectedAction = baseAction;
        OnSelectedActionChanged?.Invoke(this, EventArgs.Empty);
    }
    public BaseAction GetSelectedAction()
    {
        return selectedAction;
    }

    public Unit GetSelectedUnit()
    {
        return selectedUnit;
    }
}
