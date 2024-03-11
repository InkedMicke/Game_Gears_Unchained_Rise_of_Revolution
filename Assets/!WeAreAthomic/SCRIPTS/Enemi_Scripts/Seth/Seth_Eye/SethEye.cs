using DG.Tweening;
using Generics.Collision;
using System.Collections;
using UnityEngine;

namespace Seth
{
    public class SethEye : MonoBehaviour
    {
        [SerializeField] Seth seth;
        [SerializeField] SethEyeWeights weights;

        [SerializeField] LineRenderer laserBeam;

        [SerializeField] LayerMask laserLayer;

        Transform m_playerTr;

        [SerializeField] EnemyDamageData damageData;

        Transform m_startParent;

        Vector3 m_startPos;

        public bool IsEyeAttacking;

        [SerializeField] float speed = 5f;
        [SerializeField] float rotSpeed = 10f;
        [SerializeField] float laserSpeed = 2f;

        private void Awake()
        {
            m_playerTr = GameObject.FindGameObjectWithTag("Player").transform;
            m_startParent = transform.parent;
            m_startPos = transform.position;
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
            transform.SetParent(null);
            transform.DOMove(weights.transform.position, 3f).SetEase(Ease.Linear).OnComplete(() =>
            {
                weights.starteye();
                StartCoroutine(Laser());
                StartCoroutine(RotateTowardsPlayer());
            });
        }



        public void GoToStartPos()
        {
            laserBeam.SetPosition(1, Vector3.zero);
            transform.DOMove(weights.transform.position, 3f).SetEase(Ease.Linear).OnComplete(() =>
            {
                transform.SetParent(m_startParent);
                gameObject.SetActive(false);
            });
        } 

        IEnumerator RotateTowardsPlayer()
        {
            while(IsEyeAttacking)
            {
                yield return new WaitForEndOfFrame();
                var diff = m_playerTr.position - transform.position;
                var lookRot = Quaternion.LookRotation(diff);
                transform.rotation = Quaternion.Lerp(transform.rotation, lookRot, rotSpeed * Time.deltaTime);
            }
        }

        IEnumerator Laser()
        {
            while (IsEyeAttacking)
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

        public void SetIsEyeAttacking(bool isEyeAttacking)
        {
            IsEyeAttacking = isEyeAttacking;
        }
    }
}