using UnityEngine;

public class G_DestroyOnEnter : MonoBehaviour
{
    // Este método se llama cuando otro Collider entra en el trigger.
    private void OnTriggerEnter(Collider other)
    {
        // Verificar si el objeto que entró tiene la capa "enemyHitbox".
        if (other.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            // Destruir el objeto actual.
            Destroy(other.gameObject);
        }
    }
}
