using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private bool isOpen;
    private Animator _animator;
    private Action onInteractionComplete;
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private GridPosition gridPosition;
    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(this.transform.position);
        LevelGrid.Instance.SetInteractableAtGridPosition(gridPosition, this);

        if(isOpen)
        {
            OpenDoor();
        }
        else
        {
            CloseDoor();
        }
    }

    public void Interact(Action onInteractionComplete)
    {
        this.onInteractionComplete = onInteractionComplete;
        if (!isOpen)
        {
            OpenDoor();
        }
        else
        {
            CloseDoor();
        }
    }

    private void OpenDoor()
    {
        isOpen = true;
        _animator.SetBool("IsOpen", true);
        UpdateGridPosition();
    }

    private void CloseDoor()
    {
        isOpen = false;
        _animator.SetBool("IsOpen", false);
        UpdateGridPosition();
    }

    public void OnInteractionCompleted()
    {
        if(onInteractionComplete != null)
        {
            onInteractionComplete();
        }
    }

    private void UpdateGridPosition()
    {
        Pathfinding.Instance.SetIsWalkableGridPosition(gridPosition, isOpen);
    }
}
