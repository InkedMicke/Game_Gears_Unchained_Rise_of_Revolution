using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.Player_Scripts
{
    public class SphereDetector : MonoBehaviour
    {

        public string hurtBoxString = "HurtBox";
        [SerializeField] private GameObject arrow;

        public void OnTriggerEnter(Collider col)
        {
            var transform1 = col.transform;
            var position = transform1.position;
            var locUp = new Vector3(position.x, position.y + 2f, position.z);
            Instantiate(arrow, locUp, Quaternion.identity);
            if (col.gameObject.name == string.Format(hurtBoxString))
            {
                GameManagerSingleton.Instance.AddGameObject(col.gameObject);
            }

        }


    }
}
