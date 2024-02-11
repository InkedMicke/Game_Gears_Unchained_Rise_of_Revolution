using UnityEngine;
using UnityEngine.UI;
using _WeAreAthomic.SCRIPTS.Interfaces_Scripts;



namespace _WeAreAthomic.SCRIPTS.Enemi_Scripts.Generic
{
    public class SoldierHurtBox : MonoBehaviour, IDamageable
    {
        private C_EnemiSounds _cEnemiSounds;
        private SoldierHurtBox _soldierHurtbox;
        private GDestroyObject _destroyObject;
        private Enemy _enemy;

        

        [SerializeField] private C_DisolveEnemi _disolveEnemi;

        [SerializeField] private Slider healthSlider;

        [SerializeField] private GameObject mesh;
        [SerializeField] private GameObject soldierWithoutBones;
        [SerializeField] private GameObject healthSliderObj;
        [SerializeField] private GameObject botonSoldier;

        [SerializeField] private GameObject decalAtackDir;
        [SerializeField] private GameObject decalPatrol;

        [SerializeField] private ParticleSystem _particlesHit;

        public bool IsDeath;

        public int HurtedTimes;

        public float maxHealth = 100f;
        public float currentHealth = 0f;

        private void Awake()
        {
            _cEnemiSounds = GetComponent<C_EnemiSounds>();
            _soldierHurtbox = GetComponent<SoldierHurtBox>();
            _destroyObject = GetComponentInParent<GDestroyObject>();
            _enemy = GetComponentInParent<Enemy>();

            currentHealth = maxHealth;
            SetMaxhealthSlider(maxHealth);
            SetHealthSlider(currentHealth);
        }

        public void Damage(float damage)
        {
            _cEnemiSounds.PlayHitEnemiSound();
            _particlesHit.Play();
            currentHealth -= damage;
            HurtedTimes++;
            SetHealthSlider(currentHealth);
            CheckForDeath();
            if (!_soldierHurtbox.IsDeath)
            {
                _enemy.StartChasingPlayer();
                
            }
        }
            
        private void CheckForDeath()
        {
            if (currentHealth <= 0)
            {
                if (!_soldierHurtbox.IsDeath)
                {
                    _soldierHurtbox.SetDeath(true);
                    Death();
                }
            }
        }

        private void Death()
        {
            
            decalAtackDir.SetActive(false);
            decalPatrol.SetActive(false);
            mesh.SetActive(false);
            _enemy.DisableMovement();
            healthSliderObj.SetActive(false);
            soldierWithoutBones.SetActive(true);
            botonSoldier.SetActive(false);
            _disolveEnemi.StartDisolving();
            _destroyObject.DestroyThisObject(3f);
        }

        public void EndHurtAnim()
        {

        }

        public void SetDeath(bool isDeath)
        {
            IsDeath = isDeath;
        }
        public bool CheckIfPlayerIsNear(float radius)
        {
            return Physics.CheckSphere(transform.position, radius);
        }

        public void SetMaxhealthSlider(float maxHealth)
        {
            healthSlider.maxValue = maxHealth;
        }

        public void SetHealthSlider(float health)
        {
            healthSlider.value = health;
        }
    }
}
