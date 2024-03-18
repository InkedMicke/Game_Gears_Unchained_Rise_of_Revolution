using System.Collections.Generic;
using UnityEngine;
using Interfaces;
using Generics.Camera;

namespace Player
{
    public class MainCWrenchHitBox : MonoBehaviour
    {
        [SerializeField] GCameraShake cameraShake;

        [SerializeField] private HealthManagerSO furyManager;
        [SerializeField] private float furyPerHit = 0.2f;
        [SerializeField] private float furyPerHitDummie = 15f;

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

                        if(col.name == "DummieHurtBox")
                            furyManager.GetHealth(furyPerHitDummie);
                        else
                            furyManager.GetHealth(furyPerHit);


                        cameraShake.ShakeCamera(1f, 1f,.2f);
                    }
                    else
                    {
                        cameraShake.ShakeCamera(.5f, 10f, .1f);
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