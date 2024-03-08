using Generics.Collision;
using System.Collections;
using UnityEngine;

namespace Seth
{
    public class SethEye : MonoBehaviour
    {
        [SerializeField] SethEyeWeights weights;

        [SerializeField] LineRenderer laserBeam;
        [SerializeField] Transform startPos;
        Transform m_playerTr;

        [SerializeField] EnemyDamageData damageData;

        Transform m_startParent;

        public bool IsEyeAttacking;

        [SerializeField] float speed = 5f;
        [SerializeField] float rotSpeed = 10f;

        private void Awake()
        {
            m_playerTr = GameObject.FindGameObjectWithTag("Player").transform;
            m_startParent = transform.parent;
        }

        private void OnValidate()
        {
            laserBeam = GetComponentInChildren<LineRenderer>();
        }

        public void StartAttack()
        {
            IsEyeAttacking = true;
            transform.SetParent(null);
            StartCoroutine(MoveToStartPos());
        }

        IEnumerator MoveToStartPos()
        {
            StartCoroutine(RotateToPlayer());
            while (Mathf.Abs(Vector3.SqrMagnitude(startPos.position - transform.position)) > 1)
            {
                transform.position = Vector3.MoveTowards(transform.position, startPos.position, speed * Time.deltaTime);
                Debug.Log("hola1");
                yield return new WaitForEndOfFrame();
            }

            weights.starteye();
            StartCoroutine(Laser());
        }

        IEnumerator RotateToPlayer()
        {
            while (true)
            {
                var diff = m_playerTr.position - transform.position;
                var targetRotation = Quaternion.LookRotation(diff);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotSpeed * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
        }

        IEnumerator Laser()
        {
            while (true)
            {
                yield return new WaitForEndOfFrame();
                var ray = new Ray(transform.position, transform.forward);
                if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, LayerMask.GetMask("HurtBox", "Ground", "Obstacle"))) continue;

                laserBeam.SetPosition(1, laserBeam.transform.worldToLocalMatrix.MultiplyPoint( hit.point) );
                Debug.DrawRay(transform.position, transform.forward, Color.blue, 0.05f);

                if (hit.collider.gameObject == m_playerTr.gameObject && hit.collider.TryGetComponent(out HurtBox hurtbox))
                {
                    hurtbox.GetDamage(GameManagerSingleton.Instance.GetEnemyDamage(damageData));
                }
            }
        }
    }
}