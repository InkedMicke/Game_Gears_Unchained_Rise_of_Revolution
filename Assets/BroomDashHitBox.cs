using _WeAreAthomic.SCRIPTS.Genericos_Scripts;
using UnityEngine;

public class BroomDashHitBox : MonoBehaviour
{
    [System.NonSerialized] public EnemyDamageData EnemyDamageDataDash;

    BoxCollider m_cc;

    private void Awake()
    {
        m_cc = GetComponent<BoxCollider>();
        m_cc.enabled = false;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.TryGetComponent(out HurtBox hurtbox))
        {
            hurtbox.Damage(GameManagerSingleton.Instance.GetEnemyDamage(EnemyDamageDataDash));
        }
        m_cc.enabled = false;
    }
}
