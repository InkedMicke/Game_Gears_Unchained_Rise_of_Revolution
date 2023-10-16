using System.Collections;
using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.Enemi
{
    public class DummieHurtBox : MonoBehaviour
    {
        private GHealthManager _gHealthManager;
        private DummieController _dummieController;
        private Animator _anim;
        private CharacterController _cc;

        private Transform _playerTr;

        [SerializeField] private ParticleSystem sparksHit;
        
        [SerializeField] private float pushForce = 5f;
    
        private void Awake()
        {
            _gHealthManager = GetComponentInParent<GHealthManager>();
            _anim = GetComponentInParent<Animator>();
            _cc = GetComponentInParent<CharacterController>();
            _dummieController = GetComponentInParent<DummieController>();
        }

        private void Start()
        {
            _playerTr = GameObject.FindGameObjectWithTag("Player").transform;
        }

        public void TakeDamage(float value)
        {
            _gHealthManager.currentHealth -= value;
            _anim.SetTrigger(string.Format("isHurt"));
            sparksHit.Play();
            if (!_dummieController.isDeath)
            {
                StartCoroutine(nameof(PushBack));
            }
        }

        public IEnumerator PushBack()
        {
            var enable = true;
            var t = .2f;
            while (enable)
            {
                t -= 4f * Time.unscaledDeltaTime;
                var playerDesiredPos = new Vector3(_playerTr.position.x, transform.position.y, _playerTr.position.z);
                var moveDirection = (playerDesiredPos - transform.position).normalized;
                _cc.Move(-(moveDirection) * Time.deltaTime * pushForce);

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
