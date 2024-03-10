using Generics.Collision;
using System.Collections;
using UnityEngine;

namespace Seth
{
    public class SethEye : MonoBehaviour
    {
        [SerializeField] SethEyeWeights weights;

        [SerializeField] LineRenderer laserBeam;

        [SerializeField] LayerMask laserLayer;

        [SerializeField] Transform startPos;
        Transform m_playerTr;

        [SerializeField] EnemyDamageData damageData;

        Transform m_startParent;

        public bool IsEyeAttacking;

        [SerializeField] float speed = 5f;
        [SerializeField] float rotSpeed = 10f;
        [SerializeField] float laserSpeed = 2f;

        private void Awake()
        {
            m_playerTr = GameObject.FindGameObjectWithTag("Player").transform;
            m_startParent = transform.parent;
        }

        private void OnValidate()
        {
            laserBeam = GetComponentInChildren<LineRenderer>();
        }

        private void Start()
        {
            StartAttack();
        }

        public void StartAttack()
        {
            IsEyeAttacking = true;
            transform.SetParent(weights.transform);
            StartCoroutine(MoveToStartPos());
        }

        public void StartGoToStartPos()
        {
            StartCoroutine(GoToStartPos());
        }

        IEnumerator MoveToStartPos()
        {
            while (Mathf.Abs(Vector3.SqrMagnitude(startPos.position - transform.position)) > 1)
            {
                transform.position = Vector3.MoveTowards(transform.position, startPos.position, speed * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }

            weights.starteye();
            StartCoroutine(Laser());
        }

        IEnumerator GoToStartPos()
        {;

            while (Vector3.SqrMagnitude(weights.transform.position - transform.position) > 10)
            {
                yield return new WaitForEndOfFrame();
                var dir = weights.transform.position - transform.position;
                var rotLook = Quaternion.LookRotation(dir);

                Quaternion nuevaRotacion = Quaternion.Lerp(transform.rotation, rotLook, 10f * Time.deltaTime);

                transform.rotation = nuevaRotacion;

                transform.position += Time.deltaTime * 3f * transform.forward;
            }

            weights.starteye();
        }

        IEnumerator Laser()
        {
            var hurtboxLayer = LayerMask.NameToLayer("HurtBox");
            while (true)
            {
                yield return new WaitForEndOfFrame();

                var ray = new Ray(transform.position, transform.forward);
                if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, laserLayer)) continue;

                laserBeam.SetPosition(1, laserBeam.transform.worldToLocalMatrix.MultiplyPoint(hit.point));
                Debug.DrawRay(transform.position, transform.forward, Color.blue, 0.05f);

                if (hit.collider.gameObject == m_playerTr.gameObject && hit.collider.TryGetComponent(out HurtBox hurtbox))
                {
                    hurtbox.GetDamage(GameManagerSingleton.Instance.GetEnemyDamage(damageData));
                }

            }
        }
    }
}