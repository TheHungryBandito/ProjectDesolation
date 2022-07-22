using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRagdollSpawner : MonoBehaviour
{
    [SerializeField] private Transform ragdollPrefab;
    [SerializeField] private HealthSystem healthSystem;
    [SerializeField] private Transform originalRagdollRootBone;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();

        healthSystem.OnDead += HealthSystem_OnDead;
    }

    private void HealthSystem_OnDead(object sender, Transform attackersTransform)
    {
        Transform ragdollTransform = Instantiate(ragdollPrefab, transform.position, transform.rotation);
        UnitRagdoll unitRagdoll = ragdollTransform.GetComponent<UnitRagdoll>();
        Vector3 hitPosition = (transform.position - attackersTransform.position).normalized;
        float offsetY = 0.5f;
        hitPosition.y = offsetY;
        unitRagdoll.Setup(originalRagdollRootBone, hitPosition);

    }
}
