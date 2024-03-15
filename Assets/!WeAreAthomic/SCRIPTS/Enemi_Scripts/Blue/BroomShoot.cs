using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Broom
{
    public class BroomShoot : MonoBehaviour
    {
        [SerializeField] GameObject bulletPrefab;

        Transform m_playerTr;

        bool m_isShooting;

        [SerializeField] float shootCooldown = 2f;

        private void Awake()
        {
            m_playerTr = GameObject.FindGameObjectWithTag("Player").transform;
        }

        public void StartShooting()
        {
            StartCoroutine(Shoot());
            StartCoroutine(RotateTowardsPlayer());
        }

        IEnumerator Shoot()
        {
            m_isShooting = true;
            while (Vector3.SqrMagnitude(m_playerTr.position - transform.position) < 5f)
            {
                yield return new WaitForSeconds(shootCooldown);
                var bullet = Instantiate(bulletPrefab);
            }
        }

        IEnumerator RotateTowardsPlayer()
        {
            while(m_isShooting)
            {
                yield return new WaitForEndOfFrame();
                var diff = m_playerTr.position - transform.position;
                var desiredRot = Quaternion.LookRotation(diff);
                transform.rotation = Quaternion.Lerp(transform.rotation, desiredRot, Time.deltaTime * 15f);
            }
        }
    }
}