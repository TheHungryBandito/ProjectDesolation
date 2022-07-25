using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeProjectile : MonoBehaviour
{
    public static event EventHandler OnAnyGrenadeExploded;

    [SerializeField] private Transform grenadeExplodeVFXPrefab;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private AnimationCurve arcYAnimationCurve;

    private Action OnGrenadeBehaviourComplete;
    private Vector3 _targetPosition;
    private float totalDistance;
    private Vector3 positionXY;

    private void Update()
    {
        Vector3 moveDirection = (_targetPosition - positionXY).normalized;

        float moveSpeed = 15f;
        positionXY += moveDirection * moveSpeed * Time.deltaTime;

        float distance = Vector3.Distance(positionXY, _targetPosition);
        float distanceNormalized = 1 - distance / totalDistance;

        float maxHeight = totalDistance / 4f;
        float positionZ = arcYAnimationCurve.Evaluate(distanceNormalized) * maxHeight;
        transform.position = new Vector3(positionXY.x, positionXY.y, -positionZ);

        float reachedTargetDistance = 0.2f;
        if (Vector3.Distance(positionXY, _targetPosition) < reachedTargetDistance)
        {
            //Take Grid Cellsize into account when setting radius
            float damageRadius = 4f;
            Collider[] colliderArray = Physics.OverlapSphere(_targetPosition, damageRadius);

            foreach (Collider collider in colliderArray)
            {
                if(collider.TryGetComponent<Unit>(out Unit targetUnit))
                {
                    //Deals damage that scales with distance from bomb
                    float damageDistanceNormalized =
                        Vector3.Distance(collider.transform.position, _targetPosition) / damageRadius;
                    int rangedDamaged = Mathf.RoundToInt(Mathf.Lerp(50f, 10f, damageDistanceNormalized));
                    targetUnit.Damage(rangedDamaged, this.transform);
                }
                if(collider.TryGetComponent<DestructableCrate>(out DestructableCrate destructableCrate))
                {
                    float explosionForce = 400f;
                    destructableCrate.Damage(explosionForce, this.transform);
                }
            }
            OnAnyGrenadeExploded?.Invoke(this, EventArgs.Empty);

            trailRenderer.transform.parent = null;
            Instantiate(grenadeExplodeVFXPrefab, _targetPosition + Vector3.up * 1f, Quaternion.identity);
            
            Destroy(gameObject);
            OnGrenadeBehaviourComplete();
        }
    }
    public void Setup(GridPosition targetGridPosition, Action onGrenadeBehaviourComplete)
    {
        this.OnGrenadeBehaviourComplete = onGrenadeBehaviourComplete;
        this._targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);

        positionXY = transform.position;
        positionXY.z = 0;
        totalDistance = Vector3.Distance(positionXY, _targetPosition);

    }
}
