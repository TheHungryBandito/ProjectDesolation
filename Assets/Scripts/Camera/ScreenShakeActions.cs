using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShakeActions : MonoBehaviour
{
    
    private void Start()
    {
        ShootAction.OnAnyShoot += ShootAction_OnAnyShoot;
        GrenadeProjectile.OnAnyGrenadeExploded += GrenadeProjectile_OnAnyGrenadeExploded;
        MeleeAction.OnAnyMelee += MeleeAction_OnAnyMelee;
    }

    private void MeleeAction_OnAnyMelee(object sender, MeleeAction.OnMeleeEventArgs e)
    {
        ScreenShake.Instance.Rumble();
    }

    private void GrenadeProjectile_OnAnyGrenadeExploded(object sender, System.EventArgs e)
    {
        ScreenShake.Instance.Explosion(2f);
    }

    private void ShootAction_OnAnyShoot(object sender, ShootAction.OnShootEventArgs e)
    {
        ScreenShake.Instance.Recoil();
    }
}
