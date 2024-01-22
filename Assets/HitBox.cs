using UnityEngine;

public class HitBox : MonoBehaviour
{
    [SerializeField] protected int damage;

    private void OnTriggerEnter(Collider collision)
    {
        GotEnterCollision(collision);
    }

    public virtual void GotEnterCollision(Collider collision)
    {
        if (collision.TryGetComponent(out HurtBox hurtbox))
        {
            hurtbox.Damage(damage);
        }
    }
}
