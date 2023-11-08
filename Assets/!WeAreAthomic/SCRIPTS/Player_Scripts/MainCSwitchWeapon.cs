using _WeAreAthomic.SCRIPTS.Player;
using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.Player_Scripts
{
    public class MainCSwitchWeapon : MonoBehaviour
    {
        private MainCMovement _mainCMovement;
        private MainCAttack _mainCAttack;

        [SerializeField] private GameObject wrenchObj;
        [SerializeField] private GameObject pistolObj;
    
        public bool isUsingWrench = true;
        public bool isUsingPistol;

        private void Awake()
        {
            _mainCAttack = GetComponent<MainCAttack>();
            _mainCMovement = GetComponent<MainCMovement>();
        }


        public void SwitchWeapon()
        {
            if (wrenchObj.activeSelf)
            {
                pistolObj.SetActive(true);
                wrenchObj.SetActive(false);
            }
            
            if (pistolObj.activeSelf)
            {
                wrenchObj.SetActive(true);
                pistolObj.SetActive(false);
            }
        }
    
    }
}