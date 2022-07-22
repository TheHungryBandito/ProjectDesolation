using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableCrate : MonoBehaviour
{
    public static event EventHandler OnAnyDestroyed;

    [SerializeField] private Transform crateDestroyedPrefab;

    private GridPosition gridPosition;

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(this.transform.position);
    }
    public void Damage(float explosionForce, Transform damagersTransform)
    {

        Transform crateDestroyedTransform = Instantiate(crateDestroyedPrefab, transform.position, transform.rotation);
        ApplyExplosionToChildrenRagdoll(crateDestroyedTransform, explosionForce, damagersTransform.position, 10f);

        Destroy(gameObject);

        OnAnyDestroyed?.Invoke(this, EventArgs.Empty);
    }

    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }
    private void ApplyExplosionToChildrenRagdoll(Transform root, float explosionForce, Vector3 explosionPosition, float explosionRange)
    {
        foreach (Transform child in root)
        {
            if (child.TryGetComponent<Rigidbody>(out Rigidbody childRigidbody))
            {
                childRigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRange);

            }

            ApplyExplosionToChildrenRagdoll(child, explosionForce, explosionPosition, explosionRange);
        }
    }
}
