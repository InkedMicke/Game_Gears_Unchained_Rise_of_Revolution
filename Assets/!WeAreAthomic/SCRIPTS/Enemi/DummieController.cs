using _WeAreAthomic.SCRIPTS.Genericos;
using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.Enemi
{
    public class DummieController : MonoBehaviour
    {
        private Animator _anim;
        private GHealthManager _gHealthManager;
        private CharacterController _cc;

        [SerializeField] private GameObject hurtBoxObj;
        
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
            if (_gHealthManager.currentHealth <= 0 && !isDeath)
            {
                isDeath = true;
                Debug.Log("hola");
                _anim.SetTrigger(string.Format("isDeath"));
                if (removeCollisionDeath)
                {
                    _cc.enabled = false;
                }
            }
        }
        public void HurtAnim()
        {
            _anim.SetTrigger(string.Format("isHurt"));
        }


    }
}
