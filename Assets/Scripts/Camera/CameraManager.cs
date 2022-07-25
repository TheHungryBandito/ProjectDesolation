using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private GameObject actionCameraGameObject;

    private void Start()
    {
        BaseAction.OnAnyActionStarted += BaseAction_OnAnyActionStarted;
        BaseAction.OnAnyActionCompleted += BaseAction_OnAnyActionCompleted;
        OverwatchAction.OnAnyOverwatchTriggered += OverwatchAction_OnAnyOverwatchTriggered;
        OverwatchAction.OnAnyOverwatchEnded += OverwatchAction_OnAnyOverwatchEnded;

        HideActionCamera();
    }
    private void OverwatchAction_OnAnyOverwatchTriggered(object sender, OverwatchAction.OnOverwatchTriggeredArgs e)
    {
        StartActionCamera(sender);
    }
    private void OverwatchAction_OnAnyOverwatchEnded(object sender, EventArgs e)
    {
        EndActionCamera(sender);
    }

    private void ShowActionCamera()
    {
        actionCameraGameObject.SetActive(true);
    }

    private void HideActionCamera()
    {
        actionCameraGameObject.SetActive(false);
    }
    private void BaseAction_OnAnyActionStarted(object sender, EventArgs e)
    {
        StartActionCamera(sender);
    }
    private void BaseAction_OnAnyActionCompleted(object sender, EventArgs e)
    {
        EndActionCamera(sender);
    }

    private void StartActionCamera(object sender)
    {
        Unit targetUnit = null;
        Vector3 targetUnitWorldPosition = Vector3.zero;
        Vector3 actionCameraPosition = Vector3.zero;

        switch (sender)
        {
            case ShootAction shootAction:
                targetUnit = shootAction.GetTargetUnit();

                targetUnitWorldPosition = targetUnit.GetWorldPosition();

                actionCameraPosition = new Vector3(targetUnitWorldPosition.x, targetUnitWorldPosition.y, actionCameraGameObject.transform.position.z);

                actionCameraGameObject.transform.position = actionCameraPosition;
                ShowActionCamera();
                break;
            case MoveAction moveAction:
                break;
            case SpinAction spinAction:
                break;
            case OverwatchAction overwatchAction:
                targetUnit = overwatchAction.GetTargetUnit();
                if(targetUnit == null)
                {
                    return;
                }

                targetUnitWorldPosition = targetUnit.GetWorldPosition();
                actionCameraPosition = new Vector3(targetUnitWorldPosition.x, targetUnitWorldPosition.y, actionCameraGameObject.transform.position.z);
                actionCameraGameObject.transform.position = actionCameraPosition;

                ShowActionCamera();
                break;
            default:
                break;
        }
    }

    private void EndActionCamera(object sender)
    {
        switch (sender)
        {
            case ShootAction shootAction:

                HideActionCamera();
                break;
            case MoveAction moveAction:
                break;
            case SpinAction spinAction:
                break;
            case OverwatchAction overwatchAction:

                HideActionCamera();
                break;
            default:
                break;
        }
    }

}
