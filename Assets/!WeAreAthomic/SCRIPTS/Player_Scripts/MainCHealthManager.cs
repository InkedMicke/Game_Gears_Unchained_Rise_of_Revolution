using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Generics.Collision;
using Cinemachine;

namespace Player
{
    public class MainCHealthManager : HurtBox
    {
        private CharacterController _cc;
        private MainCRagdoll _mainCRagdoll;
        private MainCSounds _mainSounds;
        private MainCAttack _mainCAttack;
        private MainCMovement _mainCMove;
        private MainCAnimatorController _mainCAnim;
        private MainCLayers _mainCLayers;
        private MainCRail _mainCRail;
        private MainCVFX _mainCVFX;
        private MainCHurtedMaterial _mainCHurtMaterial;

        private Coroutine _hitCoroutine;

        [SerializeField] HealthManagerSO healthManagerSO;
        [SerializeField] private CinemachineVirtualCamera deathCamera; 


        [SerializeField] private Slider healthSlider;

        [SerializeField] private GameObject gameOverCanvas;
        [SerializeField] private GameObject cameraBase;
        [SerializeField] private GameObject hitFrame;
        [SerializeField] private Image sliderHealthImage;

        [SerializeField] private float timeToGameover = 1.5f;
        private float CurrentHealth;

        private bool gotHit;
        bool _isDeath;
        bool m_canReceiveDamage = true;

        protected void Awake()
        {
            _cc = GetComponentInParent<CharacterController>();
            _mainCRagdoll = GetComponentInParent<MainCRagdoll>();
            _mainSounds = GetComponentInParent<MainCSounds>();
            _mainCAttack = GetComponentInParent<MainCAttack>();
            _mainCAnim = GetComponentInParent<MainCAnimatorController>();
            _mainCLayers = GetComponentInParent<MainCLayers>();
            _mainCRail = GetComponentInParent<MainCRail>();
            _mainCMove = GetComponentInParent<MainCMovement>();
            _mainCVFX = GetComponentInParent<MainCVFX>();
            _mainCHurtMaterial = GetComponentInParent<MainCHurtedMaterial>();

            healthManagerSO.OnDeath += Death;
        }

        private void Start()
        {
            healthManagerSO.CurrentHealth = 50;
        }

        public override void GetDamage(float damage)
        {
            if (m_canReceiveDamage)
            {
                hitFrame.SetActive(true);
                sliderHealthImage.color = Color.red;
                _mainCHurtMaterial.HurtEffects();
                _mainSounds.RemoveAllSounds();
                _mainSounds.PlayHurtSound();
                _mainCAnim.TriggerHit();
                healthManagerSO.DecreaseHealth(damage);
                StartCoroutine(HitDesactivate());
                if (gotHit)
                {
                    StopCoroutine(_hitCoroutine);
                }
                _hitCoroutine = StartCoroutine(WaitForDisableHit());

            }
        }

        private void Death()
        {
            deathCamera.Priority = 100;
            _mainCAnim.AnimEnabled(false);
            _cc.enabled = false;
            _mainCRagdoll.SetEnabled(true);
            _mainSounds.PlayDieSound();
            StartCoroutine(WaitForGameOver());
        }

        public void Revive()
        {
            _cc.enabled = false;
            if (_mainCRail.IsSliding)
            {
                _mainCVFX.SetRailEffects(false);
                _mainCRail.SetIsSliding(false);
                _mainCLayers.DisableSlideLayer();
            }
            _mainCRagdoll.SetEnabled(false);
            m_canReceiveDamage = false;
            CurrentHealth = 100;
            GameManagerSingleton.Instance.currentHealth = CurrentHealth;
            _isDeath = false;
            _mainCAttack.SetIsSheathed(false);
            _mainCRagdoll.ResetBody();
            StartCoroutine(InvencibilityTime());
            PosToCheckpoint();
            deathCamera.Priority = 0;
        }

        public void Die()
        {
            CurrentHealth = 0;
            GameManagerSingleton.Instance.currentHealth = CurrentHealth;
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
            yield return new WaitForSeconds(.5f);
            hitFrame.SetActive(false);
            sliderHealthImage.color = Color.white;

        }

        private IEnumerator WaitForGameOver()
        {
            yield return new WaitForSeconds(timeToGameover);

            gameOverCanvas.SetActive(true);
            GameManagerSingleton.Instance.SetIsGameOverMenuEnabled(true);
            GameManagerSingleton.Instance.OpenTotallyWindow();
        }

        private IEnumerator WaitForDisableHit()
        {
            gotHit = true;
            _mainCMove.DisableMovement();
            _mainCAttack.DisableCanAttack();
            yield return new WaitForSeconds(.2f);
            _mainCMove.EnableMovement();
            _mainCAttack.EnableCanAttack();
            gotHit = false;

        }

        private IEnumerator InvencibilityTime()
        {
            yield return new WaitForSeconds(1f);

            m_canReceiveDamage = true;
        }

        public void SetCanReceiveDamage(bool canReceiveDamage)
        {
            m_canReceiveDamage = canReceiveDamage;
        }
    }
}
