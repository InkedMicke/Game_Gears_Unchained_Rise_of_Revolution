using UnityEngine;

public class HurtBox : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        GotEnterCollision(other);
    }

    protected virtual void GotEnterCollision(Collider col)
    {

    }
}
