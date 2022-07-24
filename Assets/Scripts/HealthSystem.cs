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


    private void Awake()
    {
        healthMax = health;
    }
    public void Damage(float damageAmount, Transform attackersTransform)
    {
        health -= damageAmount;

        if (health < 0)
        {
            health = 0;
        }
        OnHealthChanged?.Invoke(this, attackersTransform);

        if (health == 0)
        {
            Die(attackersTransform);
        }

        Debug.Log($"{attackersTransform.name} hit for {damageAmount}.");
    }
    private void Die(Transform attackersTransform)
    {
        OnDead?.Invoke(this, attackersTransform);
    }

    public float GetHealthNormalized()
    {
        return health / healthMax;
    }
}
