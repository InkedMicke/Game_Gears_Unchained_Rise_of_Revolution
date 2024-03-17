using System.Collections.Generic;
using UnityEngine;
using Interfaces;
using Generics.Camera;

namespace Player
{
    public class MainCWrenchHitBox : MonoBehaviour
    {
       
        [SerializeField] private HealthManagerSO furyManager;
        [SerializeField] private float furyPerHit = 0.2f;

        [SerializeField] private PlayerDamageData wrenchDamageData;
        
        public List<Collider> colliderList = new();

        public bool GotHit;

   

        private void OnTriggerEnter(Collider other)
        {
            colliderList.Add(other);
            SpeedDownTime();
            Invoke(nameof(SpeedUpTime), .005f);
        }

        public void ClearList()
        {
            colliderList.Clear();
        }

        public void SetGotHit(bool gotHit)
        {
            GotHit = gotHit;
        }

        public void ApplyDamage()
        {
            foreach (var col in colliderList)
            {
                if (col.TryGetComponent(out IDamageable damageable))
                {
                    if (damageable.CanReceiveDamage())
                    {
                        damageable.GetDamage(GameManagerSingleton.Instance.GetPlayerDamage(wrenchDamageData, col.gameObject));

                        furyManager.DecreaseHealth(furyPerHit);
                        GCameraShake.Instance.ShakeCamera(1f, 1f,.2f);
                    }
                    else
                    {
                        GCameraShake.Instance.ShakeCamera(.5f, 10f, .1f);
                        damageable.GetDamage(GameManagerSingleton.Instance.GetPlayerDamage(wrenchDamageData, col.gameObject));
                    }
                }

            }
            ClearList();
        }

        private void SpeedUpTime()
        {
            Time.timeScale = 1f;
        }

        private void SpeedDownTime()
        {
            Time.timeScale = 0.1f;
        }
       
    }
}