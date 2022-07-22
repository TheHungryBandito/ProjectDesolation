using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractSphere : MonoBehaviour, IInteractable
{
    [SerializeField] private Material greenMaterial;
    [SerializeField] private Material redMaterial;

    [SerializeField] private MeshRenderer meshRenderer;

    private Action onInteractionComplete;
    private bool isGreen;
    private bool isActive;
    private float timer;



    private void Start()
    {
        GridPosition gridPosition = LevelGrid.Instance.GetGridPosition(this.transform.position);
        LevelGrid.Instance.SetInteractableAtGridPosition(gridPosition, this);

        SetColorRed();
    }

    private void Update()
    {
        if(!isActive)
        {
            return;
        }

        timer -= Time.deltaTime;

        if(timer <= 0f)
        {
            isActive = false;
            OnInteractionCompleted();
        }
    }
    public void Interact(Action onInteractionComplete)
    {
        this.onInteractionComplete = onInteractionComplete;
        isActive = true;
        timer = 0.5f;

        if(isGreen)
        {
            SetColorRed();
        }
        else
        {
            SetColorGreen();
        }
    }

    private void SetColorGreen()
    {
        isGreen = true;
        meshRenderer.material = greenMaterial;
    }

    private void SetColorRed()
    {
        isGreen = false;
        meshRenderer.material = redMaterial;
    }

    public void OnInteractionCompleted()
    {
        if (onInteractionComplete != null)
        {
            onInteractionComplete();
        }
    }




}
