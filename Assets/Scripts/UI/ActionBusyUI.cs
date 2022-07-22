using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBusyUI : MonoBehaviour
{

    private void Start()
    {
        UnitActionSystem.Instance.OnBusyChanged += UnitActionSystem_OnBusyChanged;

        Hide();
    }
    private void UnitActionSystem_OnBusyChanged(object sender, bool isBusy)
    {
        HandleBusyUI(isBusy);
    }
    private void HandleBusyUI(bool isBusy)
    {
        if (isBusy)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
