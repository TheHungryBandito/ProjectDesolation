using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="GridVisualType",
    menuName="ScriptableObject/Grid/VisualType")]
public class GridVisualTypeSO : ScriptableObject
{
    [SerializeField] private Material material;

    public Material GetMaterial()
    {
        return material;
    }
}
