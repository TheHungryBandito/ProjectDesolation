using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ScreenShake : MonoBehaviour
{
    public static ScreenShake Instance { get; private set; }

    private CinemachineImpulseSource cinemachineImpulseSource;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one ScreenShake! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
        cinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void Shake(float intensity)
    {
        cinemachineImpulseSource.GenerateImpulse(intensity);
    }

    public void Rumble(float intensity = 1f)
    {
        cinemachineImpulseSource.m_ImpulseDefinition.m_ImpulseShape = CinemachineImpulseDefinition.ImpulseShapes.Rumble;
        cinemachineImpulseSource.m_DefaultVelocity.Set(0, 0.3f, 0);
        cinemachineImpulseSource.m_ImpulseDefinition.m_ImpulseDuration = 0.3f;

        Shake(intensity);
    }
    public void Recoil(float intensity = 1f)
    {
        cinemachineImpulseSource.m_ImpulseDefinition.m_ImpulseShape = CinemachineImpulseDefinition.ImpulseShapes.Recoil;
        cinemachineImpulseSource.m_DefaultVelocity.Set(0, 0.3f, 0.5f);
        cinemachineImpulseSource.m_ImpulseDefinition.m_ImpulseDuration = 0.2f;
        
        Shake(intensity);
    }
    public void Explosion(float intensity = 1f)
    {
        cinemachineImpulseSource.m_ImpulseDefinition.m_ImpulseShape = CinemachineImpulseDefinition.ImpulseShapes.Explosion;
        cinemachineImpulseSource.m_DefaultVelocity.Set(0, 0.3f, 0);
        cinemachineImpulseSource.m_ImpulseDefinition.m_ImpulseDuration = 0.3f;

        Shake(intensity);
    }
    public void CustomShake(float intensity, CinemachineImpulseDefinition.ImpulseShapes impulseShape, Vector3 defaultVelocity, float impulseDuration)
    {
        cinemachineImpulseSource.m_ImpulseDefinition.m_ImpulseShape = impulseShape;
        cinemachineImpulseSource.m_DefaultVelocity.Set(defaultVelocity.x, defaultVelocity.y, defaultVelocity.z);
        cinemachineImpulseSource.m_ImpulseDefinition.m_ImpulseDuration = impulseDuration;

        Shake(intensity);
    }
}
