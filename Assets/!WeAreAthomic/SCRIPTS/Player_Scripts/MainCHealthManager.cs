using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using _WeAreAthomic.SCRIPTS.Genericos_Scripts;

namespace _WeAreAthomic.SCRIPTS.Player_Scripts
{
    public class MainCHealthManager : HurtBox
    {
        private CharacterController _cc;
        private MainCRagdoll _mainCRagdoll;
        private MainCSounds _mainSounds;
        private MainCAttack _mainCAttack;
        private MainCAnimatorController _mainCAnim;
        private MainCLayers _mainCLayers;

        [SerializeField] private Slider healthSlider;

        [SerializeField] private GameObject gameOverCanvas;
        [SerializeField] private GameObject cameraBase;
        [SerializeField] private GameObject hitFrame;
        [SerializeField] private Image sliderHealthImage;

        [SerializeField] private float timeToGameover = 1.5f;
        public float maxHealth = 100;
        public float currentHealth;

        public bool IsDeath;
        public bool CanReceiveDamage = true;

        private void Awake()
        {
            _cc = GetComponentInParent<CharacterController>();
            _mainCRagdoll = GetComponentInParent<MainCRagdoll>();
            _mainSounds = GetComponentInParent<MainCSounds>();
            _mainCAttack = GetComponentInParent<MainCAttack>();
            _mainCAnim = GetComponentInParent<MainCAnimatorController>();
            _mainCLayers = GetComponentInParent<MainCLayers>();
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

        public override void Damage(float damage)
        {
            if (!IsDeath && CanReceiveDamage)
            {
                hitFrame.SetActive(true);
                sliderHealthImage.color = Color.red;
                currentHealth -= damage;
                GameManagerSingleton.Instance.currentHealth = currentHealth;
                _mainSounds.RemoveAllSounds();
                _mainSounds.PlayHurtSound();
                _mainCAnim.TriggerHit();
                _mainCLayers.EnableHitLayer();
                SetHealthSlider();
                CheckDeath();
                StartCoroutine(HitDesactivate());
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
            if (currentHealth <= 0)
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
            _mainCAnim.AnimEnabled(false);
            _cc.enabled = false;
            _mainCRagdoll.SetEnabled(true);
            _mainSounds.PlayDieSound();
            StartCoroutine(WaitForGameOver());
        }

        public void Revive()
        {
            _mainCRagdoll.SetEnabled(false);
            CanReceiveDamage = false;
            currentHealth = 100;
            GameManagerSingleton.Instance.currentHealth = currentHealth;
            SetHealthSlider();
            IsDeath = false;
            _mainCAttack.SetIsSheathed(false);
            _mainCRagdoll.ResetBody();
            StartCoroutine(InvencibilityTime());
        }

        public void Die()
        {
            currentHealth = 0;
            GameManagerSingleton.Instance.currentHealth = currentHealth;
            SetHealthSlider();
            Death();
        }

        public void PosToCheckpoint()
        {
            transform.parent.position = GameManagerSingleton.Instance.currentCheckpoint;
            cameraBase.transform.position = GameManagerSingleton.Instance.currentCheckpoint;
            _mainCAnim.AnimEnabled(true);
            _cc.enabled = true;
        }

        IEnumerator HitDesactivate()
        {
            hitFrame.SetActive(false);
            sliderHealthImage.color = Color.white;
            yield return new WaitForSeconds(.5f); 
        }

        private IEnumerator WaitForGameOver()
        {
            yield return new WaitForSeconds(timeToGameover);

            gameOverCanvas.SetActive(true);
            GameManagerSingleton.Instance.SetIsGameOverMenuEnabled(true);
            GameManagerSingleton.Instance.OpenTotallyWindow();
        }

        private IEnumerator InvencibilityTime()
        {
            yield return new WaitForSeconds(1f);

            CanReceiveDamage = true;
        }

        public void SetCanReceiveDamage(bool canReceiveDamage)
        {
            CanReceiveDamage = canReceiveDamage;
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
