using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public event EventHandler<Transform> OnHealthChanged;
    public event EventHandler<Transform> OnDead;

    [SerializeField] private float health = 100;
    private float healthMax = 100;
    [SerializeField] private float armour = 100;
    private float armourMax = 100;
    [SerializeField] private int armourClass = 1;


    private void Awake()
    {
        healthMax = health;
        armourMax = armour;
    }
    public void Damage(float damageAmount, Transform attackersTransform)
    {
        float damageModifier = (1 - (armourClass / 10)) + (1 - GetArmourNormalized());

        health -= damageAmount * damageModifier;
        armour -= damageAmount / armourClass;

        if (health < 0)
        {
            health = 0;
        }
        if (armour < 0)
        {
            armour = 0;
        }
        OnHealthChanged?.Invoke(this, attackersTransform);

        if (health == 0)
        {
            Die(attackersTransform);
        }

        Debug.Log($"{attackersTransform.name} hit for {damageAmount * damageModifier}.\n" +
            $"Armour hit for {damageAmount / armourClass}");
    }
    private void Die(Transform attackersTransform)
    {
        OnDead?.Invoke(this, attackersTransform);
    }

    public float GetHealthNormalized()
    {
        return health / healthMax;
    }

    public float GetArmourNormalized()
    {
        return armour / armourMax;
    }
}
