using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseWorld : MonoBehaviour
{
    private static MouseWorld Instance;

    [SerializeField] private LayerMask _mousePlaneLayerMask;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one MouseWorld! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    public static Vector3 GetMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
        Physics.Raycast(ray, out RaycastHit hitInfo, float.MaxValue, Instance._mousePlaneLayerMask);
        return hitInfo.point;
    }
}
