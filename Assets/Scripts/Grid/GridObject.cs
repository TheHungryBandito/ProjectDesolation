using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject
{
    public event EventHandler<OnUnitAddedEventArgs> OnUnitChanged;

    public class OnUnitAddedEventArgs : EventArgs
    {
        public Unit unit;
    }

    private GridSystem<GridObject> gridSystem;
    private GridPosition gridPosition;
    private List<Unit> unitList;
    private IInteractable interactable;


    public GridObject(GridSystem<GridObject> gridSystem, GridPosition gridPosition)
    {
        this.gridSystem = gridSystem;
        this.gridPosition = gridPosition;
        unitList = new List<Unit>();
    }

    public override string ToString()
    {
        string unitString = "";
        foreach (Unit unit in unitList)
        {
            unitString += unit + "\n";
        }
        return $"{gridPosition} \n {unitString}";
    }
    public void AddUnit(Unit unit)
    {
        unitList.Add(unit);
        OnUnitChanged?.Invoke(this, new OnUnitAddedEventArgs
        {
            unit = unit,
        });
    }
    public void RemoveUnit(Unit unit)
    {
        unitList.Remove(unit);
        OnUnitChanged?.Invoke(this, new OnUnitAddedEventArgs
        {
            unit = unit,
        });
    }

    public List<Unit> GetUnitList()
    {
        return unitList;
    }

    public Unit GetUnit()
    {
        if(HasAnyUnit())
        {
            return unitList[0];
        }
        return null;
    }
    public bool HasAnyUnit()
    {
        return unitList.Count > 0;
    }

    public IInteractable GetInteractable()
    {
        return interactable;
    }

    public void SetInteractable(IInteractable interactable)
    {
        this.interactable = interactable;
    }


}
