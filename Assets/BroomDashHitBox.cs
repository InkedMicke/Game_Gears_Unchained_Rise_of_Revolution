using Generics.Collision;
using UnityEngine;

public class BroomDashHitBox : HitBox
{
    [System.NonSerialized] public EnemyDamageData EnemyDamageDataDash;

    BoxCollider m_cc;

    private void Awake()
    {
        m_cc = GetComponent<BoxCollider>();
        m_cc.enabled = false;
    }

    protected override void GotEnterCollision(Collider col)
    {
        DoDamage(GameManagerSingleton.Instance.GetEnemyDamage(EnemyDamageDataDash), col);
        m_cc.enabled = false;
    }
}
