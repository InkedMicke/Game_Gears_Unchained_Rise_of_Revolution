using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.Genericos_Scripts {
    public class GWaypoints : MonoBehaviour {
        [Range(0f, 2f)][SerializeField] private float waypointsSize = 1f;
        [SerializeField] private bool closed;
        [SerializeField] private bool linesBetween;
        [SerializeField] private bool seeSpheres = true;

        private void OnDrawGizmos() {
            if (seeSpheres)
            {
                foreach (Transform t in transform)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawSphere(t.position, waypointsSize);
                }
            }

            if (linesBetween)
            {
                Gizmos.color = Color.yellow;
                for (int i = 0; i < transform.childCount - 1; i++)
                {
                    Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(i + 1).position);
                }
            }

            if (closed)
            {
                Gizmos.DrawLine(transform.GetChild(transform.childCount - 1).position, transform.GetChild(0).position);
            }
        }

        public Transform GetNextWaypoint(Transform currentWaypoint) {
            if (currentWaypoint == null) {
                return transform.GetChild(0);
            }

            if (currentWaypoint.GetSiblingIndex() < transform.childCount - 1) {
                return transform.GetChild(currentWaypoint.GetSiblingIndex() + 1);
            }
            else {
                return transform.GetChild(0);
            }
        }
    }
}
