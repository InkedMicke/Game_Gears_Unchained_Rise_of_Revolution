using UnityEngine;

namespace Seth
{
    public class SethAnimator : MonoBehaviour
    {
        private Animator _anim;

        [SerializeField] private string pushBack = "pushBack";
        [SerializeField] private string hit = "hit";
        [SerializeField] private string pointOut = "pointOut";
        [SerializeField] private string laserCount = "laserCount";

        private void Awake()
        {
            _anim = GetComponent<Animator>();
        }

        public void TriggerPush()
        {
            _anim.SetTrigger(pushBack);
        }

        public void TriggerHit()
        {
            _anim.SetTrigger(pushBack);
        }

        public void TriggerPointOut()
        {
            _anim.SetTrigger(pushBack);
        }

        public void SetLaserCount(int value)
        {
            _anim.SetInteger(laserCount, value);
        }
    }
}