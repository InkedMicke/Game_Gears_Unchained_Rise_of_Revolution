using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.Genericos_Scripts
{
    public class GRotateXYBasedOnTr : MonoBehaviour
    {
        [SerializeField] private Transform target;
        void Update()
        {
            Vector3 newRotation = new Vector3(0f, target.eulerAngles.y, target.eulerAngles.z);
            transform.rotation = Quaternion.Euler(newRotation);
        }
    }
}
