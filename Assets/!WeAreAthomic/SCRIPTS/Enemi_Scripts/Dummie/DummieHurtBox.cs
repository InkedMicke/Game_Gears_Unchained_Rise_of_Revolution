using System.Collections;
using _WeAreAthomic.SCRIPTS.Genericos;
using _WeAreAthomic.SCRIPTS.Player_Scripts;
using _WeAreAthomic.SCRIPTS.Interfaces_Scripts;
using _WeAreAthomic.SCRIPTS.Enemi_Scripts.Generic;
using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.Enemi_Scripts.Dummie
{
    public class DummieHurtBox : MonoBehaviour, IDamageable
    {
        private GHealthManager _gHealthManager;
        private DummieController _dummieController;
        private Animator _anim;
        private CharacterController _cc;
        private DummieSounds _dummieSounds;
        private CapsuleCollider _cC;
        private MainCAttack _mainCAttack;
        private SoldierHurtBox _soldierHurtbox;

        [SerializeField] private LayerMask obstacles;

        [SerializeField] private GameObject soundComponentObj;
        private GameObject _playerObj;
        
        private Transform _playerTr;

        [SerializeField] private ParticleSystem sparksHit;

        [SerializeField] private bool useKnockback = true;
        public bool isDeath;
        
        [SerializeField] private float pushForce = 5f;
    
        private void Awake()
        {
            _gHealthManager = GetComponentInParent<GHealthManager>();
            _anim = GetComponentInParent<Animator>();
            _cc = GetComponentInParent<CharacterController>();
            _dummieController = GetComponentInParent<DummieController>();
            _dummieSounds = soundComponentObj.GetComponent<DummieSounds>();
            _cC = GetComponent<CapsuleCollider>();
            _soldierHurtbox = GetComponent<SoldierHurtBox>();
        }

        private void Start()
        {
            _playerObj = GameObject.FindGameObjectWithTag("Player");
            _playerTr = _playerObj.transform;
            _mainCAttack = _playerObj.GetComponent<MainCAttack>();
        }

        public void TakeDamage(float value)
        {
            _gHealthManager.currentHealth -= value;
            _gHealthManager.SetSlider(_gHealthManager.currentHealth);
            _anim.SetTrigger(string.Format("isHurt"));
            sparksHit.Play();
            if (!isDeath)
            {
                if (useKnockback && _mainCAttack.attackCount != 2)
                {
                    StartCoroutine(nameof(PushBack));
                }

                _dummieSounds.PlayHurtSound();
            }

            CheckForDeath();
        }

        public void Damage(float value)
        {
            TakeDamage(value);
        }

        private void CheckForDeath()
        {
            if (_gHealthManager.currentHealth <= 0)
            {
                isDeath = true;
                _anim.SetBool(string.Format("isDeath"), true);
                _cC.enabled = false;
                _dummieController.DisableCharacterController();
            }
        }

        private void OnDrawGizmos()
        {
            Debug.DrawRay(transform.position, -transform.forward * 1f);
        }

        public IEnumerator PushBack()
        {
            var enable = true;
            var t = .2f;
            var ray = new Ray(transform.position, -transform.forward);
            if (!Physics.Raycast(ray, .5f, obstacles))
            {
                while (enable)
                {
                    t -= 4f * Time.unscaledDeltaTime;
                    var playerDesiredPos = new Vector3(_playerTr.position.x, transform.position.y, _playerTr.position.z);
                    var moveDirection = (playerDesiredPos - transform.position).normalized;
                    _cc.Move(-moveDirection * Time.deltaTime * pushForce);

                    if (t <= 0f)
                    {
                        t = 0f;
                        enable = false;
                    }

                    yield return new WaitForSeconds(0.01f);
                }
            }
        }
    }
}
