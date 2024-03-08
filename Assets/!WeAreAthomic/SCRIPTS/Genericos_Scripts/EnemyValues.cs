using UnityEngine;

[CreateAssetMenu (fileName = "new EnemyValues", menuName = "Enemy/Enemy Values")]
public class EnemyValues : ScriptableObject
{
    [Header("Motion")]
    public float MoveSpeed;
    public float PatrolSpeed;
    public float ChaseSpeed;
    public float StoppingDistance;
    [Header("KnockBack")]
    public float KnockbackDisplacement;
    public float KnockbackForce;
    [Header("FOV")]
    public float fovRadius;
    public float fovAngle;
    public Color fovRadiusColor;
    [Header("FOVHearNear")]
    public float fovHearNearRadius;
    public float fovHearNearAngle;
    public Color fovHearNearRadiusColor;
    [Header("FOVHearFar")]
    public float fovHearFarRadius;
    public float fovHearFarAngle;
    public Color fovHearFarRadiusColor;
    [Header("Detection")]
    public float TimeToGetCached;
}
