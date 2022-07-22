using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimationEvents : MonoBehaviour
{
    [SerializeField] private Unit unit;

    public void Unit_AnimationMeleeEvent()
    {
        MeleeAction meleeAction = unit.GetAction<MeleeAction>();

        meleeAction.Melee();
    }

}
