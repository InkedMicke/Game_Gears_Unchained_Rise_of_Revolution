using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.LightKiller
{
    public class GLookAt : MonoBehaviour
    {
        private void LateUpdate()
        {
            transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
        }
    }
}
