using UnityEngine;
using UnityEngine.UI;
using _WeAreAthomic.SCRIPTS.Interfaces_Scripts;
using System.Collections;

namespace _WeAreAthomic.SCRIPTS.Player_Scripts
{
    public class MainCHealthManager : MonoBehaviour, IDamageable
    {
        private CharacterController _cc;
        private MainCRagdoll _mainCRagdoll;
        private MainCSounds _mainSounds;
        private MainCAttack _mainCAttack;
        private MainCPlayerInterface _mainCInterface;
        private MainCAnimatorController _mainCAnim;
        private MainCLayers _mainCLayers;

        [SerializeField] private Slider healthSlider;

        [SerializeField] private GameObject gameOverCanvas;
        [SerializeField] private GameObject cameraBase;

        [SerializeField] private float timeToGameover = 1.5f;

        public bool IsDeath;
        public bool CanReceiveDamage = true;

        public float currentHealth = 50f;
        public float maxHealth = 100f;

        private void Awake()
        {
            _cc = GetComponentInParent<CharacterController>();
            _mainCRagdoll = GetComponentInParent<MainCRagdoll>();
            _mainSounds = GetComponentInParent<MainCSounds>();
            _mainCInterface = GetComponentInParent<MainCPlayerInterface>();
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

        public void Damage(float damage)
        {
            if (!IsDeath && CanReceiveDamage)
            {
                currentHealth -= damage;
                GameManagerSingleton.Instance.currentHealth = currentHealth;
                _mainSounds.RemoveAllSounds();
                _mainSounds.PlayHurtSound();
                _mainCAnim.TriggerHit();
                _mainCLayers.EnableHitLayer();
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

        private IEnumerator WaitForGameOver()
        {
            yield return new WaitForSeconds(timeToGameover);

            gameOverCanvas.SetActive(true);
            GameManagerSingleton.Instance.SetIsGameOverMenuEnabled(true);
            GameManagerSingleton.Instance.OpenTotallyWindow();
        }

        private IEnumerator InvencibilityTime()
        {
            yield return new WaitForSeconds(2f);

            CanReceiveDamage = true;
        }

        private IEnumerator waitTest()
        {
            yield return new WaitForSeconds(1f);
            Debug.Log("hola2");

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
