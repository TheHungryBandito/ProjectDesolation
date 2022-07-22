using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _bulletProjectilePrefab;
    [SerializeField] private Transform _shootPointTransform;

    private void Awake()
    {
        //Action Animator Events
        if(TryGetComponent<MoveAction>(out MoveAction moveAction))
        {
            moveAction.OnStartMoving += MoveAction_OnStartMoving;
            moveAction.OnStopMoving += MoveAction_OnStopMoving;
        }
        if(TryGetComponent<ShootAction>(out ShootAction shootAction))
        {
            shootAction.OnShoot += ShootAction_OnShoot;
        }
        if (TryGetComponent<MeleeAction>(out MeleeAction meleeAction))
        {
            meleeAction.OnMelee += MeleeAction_OnMelee;
        }

    }

    private void MeleeAction_OnMelee(object sender, MeleeAction.OnMeleeEventArgs e)
    {
        _animator.SetTrigger("Melee");
    }

    private void ShootAction_OnShoot(object sender, ShootAction.OnShootEventArgs e)
    {
        _animator.SetTrigger("Shoot");

        Transform bulletProjectileTransform = 
            Instantiate(_bulletProjectilePrefab, _shootPointTransform.position, Quaternion.identity);

        BulletProjectile bulletProjectile = bulletProjectileTransform.GetComponent<BulletProjectile>();

        Vector3 targetUnitShootAtPosition = e.targetUnit.GetWorldPosition();

        targetUnitShootAtPosition.y = _shootPointTransform.position.y;

        bulletProjectile.Setup(targetUnitShootAtPosition);
    }

    private void MoveAction_OnStopMoving(object sender, EventArgs e)
    {
        _animator.SetBool("IsWalking", false);
    }

    private void MoveAction_OnStartMoving(object sender, EventArgs e)
    {
        _animator.SetBool("IsWalking", true);
    }

}
