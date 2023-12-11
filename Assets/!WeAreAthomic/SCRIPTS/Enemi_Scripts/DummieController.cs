using _WeAreAthomic.SCRIPTS.Genericos;
using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.Enemi_Scripts
{
    public class DummieController : MonoBehaviour
    {
        private Animator _anim;
        private GHealthManager _gHealthManager;
        private CharacterController _cc;

        [SerializeField] private GameObject hurtBoxObj;
        
        public bool removeCollisionOnDeath;

        private void Awake()
        {
            _gHealthManager = GetComponent<GHealthManager>();
            _anim = GetComponent<Animator>();
            _cc = GetComponent<CharacterController>();
        }

        public void DisableCharacterController()
        {
            if (removeCollisionOnDeath)
            {
                _cc.enabled = false;
            }
        }
        
        public void HurtAnim()
        {
            _anim.SetTrigger(string.Format("isHurt"));
        }


    }
}
