using UnityEngine;
using UnityEngine.UI;
using _WeAreAthomic.SCRIPTS.Interfaces_Scripts;

namespace _WeAreAthomic.SCRIPTS.Player_Scripts
{
    public class MainCHealthManager : MonoBehaviour, IDamageable
    {
        private Animator _anim;
        private CharacterController _cc;
        private MainCRagdoll _mainCRagdoll;
        private MainCSounds _mainSounds;
        private MainCAttack _mainCAttack;
        private MainCPlayerInterface _mainCInterface;

        [SerializeField] private Slider healthSlider;
       

        public bool IsDeath;

        public float currentHealth = 50f;
        public float maxHealth = 100f;

        private void Awake()
        {
            _anim = GetComponentInParent<Animator>();
            _cc = GetComponentInParent<CharacterController>();
            _mainCRagdoll = GetComponentInParent<MainCRagdoll>();
            _mainSounds = GetComponentInParent<MainCSounds>();
            _mainCInterface = GetComponentInParent<MainCPlayerInterface>();
            _mainCAttack = GetComponentInParent<MainCAttack>();
        }

        private void Start()
        {
            SetMaxHealthSlider();
            SetHealthSlider();
        }

        private void Update()
        {
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
        }

        public void Damage(float damage)
        {
            if (!IsDeath)
            {
                currentHealth -= damage;
                GameManagerSingleton.Instance.currentHealth = currentHealth;
                _mainSounds.RemoveAllSounds();
                _mainSounds.PlayHurtSound();
                SetHealthSlider();
                CheckDeath();
            }
            
        }

        public void GetHealth(float health)
        {
            currentHealth += health;
            GameManagerSingleton.Instance.currentHealth = currentHealth;
            SetHealthSlider();
        }

        private void CheckDeath()
        {
            if(currentHealth <= 0)
            {
                if (!IsDeath)
                {
                    IsDeath = true;
                    Death();
                }
                currentHealth = 0;
                GameManagerSingleton.Instance.currentHealth = currentHealth;
            }
        }

        private void Death()
        {
           
            _anim.enabled = false;
            _cc.enabled = false;
            _mainCRagdoll.SetEnabled(true);
            _mainSounds.PlayDieSound();
        }

        public void Revive()
        {
            _cc.enabled = true;
            _anim.enabled = true;
            currentHealth = 100;
            GameManagerSingleton.Instance.currentHealth = currentHealth;
            SetHealthSlider();
            IsDeath = false;
            _mainCAttack.SetIsSheathed(false);
            _mainCRagdoll.ResetBody();
        }

        public void Die()
        {
            currentHealth = 0;
            GameManagerSingleton.Instance.currentHealth = currentHealth;
            SetHealthSlider();
            Death();
        }

        public void SetHealthSlider()
        {
            healthSlider.value = currentHealth;
        }

        private void SetMaxHealthSlider()
        {
            healthSlider.maxValue = maxHealth;
        }
    }
}
