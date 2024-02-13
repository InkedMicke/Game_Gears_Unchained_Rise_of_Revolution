using UnityEngine;
using UnityEngine.InputSystem;

namespace _WeAreAthomic.SCRIPTS.Player_Scripts
{
    public class MainCNewAttack : MonoBehaviour
    {
        private PlayerInputActions m_inputActions;
        private MainCMovement m_mainCMove;
        private MainCAnimatorController m_mainCAnim;
        private MainCLayers m_mainCLayers;

        private bool m_canNextAttack;

        private int m_currentAttack;

        private void Awake()
        {
            m_mainCMove = GetComponent<MainCMovement>();
            m_mainCLayers = GetComponent<MainCLayers>();
            m_mainCAnim = GetComponent<MainCAnimatorController>();

            m_inputActions = new PlayerInputActions();
            m_inputActions.Enable();
            m_inputActions.PlayerPC.Attack.performed += StartAttack;
            m_inputActions.PlayerPC.Attack.performed += NextAttack;
        }

        public void StartAttack(InputAction.CallbackContext x)
        {
            if(m_currentAttack == 0)
            {
                Debug.Log("hola1");
                m_mainCLayers.EnableNewAttackLayer();
                m_currentAttack++;
                m_mainCAnim.SetAttackCountAnim(m_currentAttack);
                m_mainCAnim.SetRootMotion(true);
            }
        }

        public void NextAttack(InputAction.CallbackContext x)
        {
            if (m_currentAttack > 0 && m_canNextAttack)
            {
                m_currentAttack++;
                m_mainCAnim.SetAttackCountAnim(m_currentAttack);
                m_canNextAttack = false;
            }
        }

        public void EndNewAttack()
        {
            m_mainCLayers.DisableNewAttackLayer();
            m_currentAttack = 0;
            m_mainCAnim.SetAttackCountAnim(m_currentAttack);
            m_canNextAttack = false;
            m_mainCAnim.SetRootMotion(false);
        }

        public void EnableCanNextAttack() => m_canNextAttack = true;

    }
}
