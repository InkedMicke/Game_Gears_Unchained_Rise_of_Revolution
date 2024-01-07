using UnityEngine;

public class G_DestroyOnEnter : MonoBehaviour
{
    // Este m�todo se llama cuando otro Collider entra en el trigger.
    private void OnTriggerEnter(Collider other)
    {
        // Verificar si el objeto que entr� tiene la capa "enemyHitbox".
        if (other.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            // Destruir el objeto actual.
            Destroy(other.gameObject);
        }
    }
}
