using UnityEngine;


namespace _WeAreAthomic.SCRIPTS.Props_Scripts
{
    public class Checkpoint : MonoBehaviour
    {
        public void SetCheckpoint(Transform tr)
        {
            var pos = new Vector3(tr.position.x, tr.position.y, tr.position.z);
            GameManagerSingleton.Instance.currentCheckpoint = pos;
        }
    }
}
