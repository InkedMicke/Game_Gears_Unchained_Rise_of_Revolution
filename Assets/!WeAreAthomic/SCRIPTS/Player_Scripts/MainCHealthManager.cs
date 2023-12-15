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

        [SerializeField] private Slider healthSlider;

        public bool IsDeath;

        public float currentHealth = 50f;
        public float maxHealth = 100f;

        private void Awake()
        {
            _anim = GetComponentInParent<Animator>();
            _cc = GetComponentInParent<CharacterController>();
            _mainCRagdoll = GetComponentInParent<MainCRagdoll>();
            _mainSounds = GetComponent<MainCSounds>();
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
            currentHealth -= damage;
            GameManagerSingleton.Instance.currentHealth = currentHealth;
            _mainSounds.PlayHurtSound();
            CheckDeath();
            SetHealthSlider();
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
            _mainSounds.PlayDieSound();
            _anim.enabled = false;
            _cc.enabled = false;
            _mainCRagdoll.SetEnabled(true);
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
