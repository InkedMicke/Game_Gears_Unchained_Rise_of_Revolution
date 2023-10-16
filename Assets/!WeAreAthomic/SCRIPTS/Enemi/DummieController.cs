using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.Enemi
{
    public class DummieController : MonoBehaviour
    {
        private Animator _anim;
        private GHealthManager _gHealthManager;
        private CharacterController _cc;

        [SerializeField] private GameObject hurtBoxObj;
        
        public bool playDeathAnim;
        public bool removeCollisionDeath;
        public bool isDeath;

        private void Awake()
        {
            _gHealthManager = GetComponent<GHealthManager>();
            _anim = GetComponent<Animator>();
            _cc = GetComponent<CharacterController>();
        }

        private void Update()
        {
            if (_gHealthManager.currentHealth <= 0 && playDeathAnim && !isDeath)
            {
                isDeath = true;
                _anim.SetBool(string.Format("isDeath"), true);
                if (removeCollisionDeath)
                {
                    _cc.enabled = false;
                }
            }
            else if (_gHealthManager.currentHealth <= 0 && playDeathAnim)
            {
                Destroy(this.gameObject);
            }
        }
        public void HurtAnim()
        {
            _anim.SetTrigger(string.Format("isHurt"));
        }


    }
}
