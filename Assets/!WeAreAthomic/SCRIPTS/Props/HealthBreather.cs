using System.Collections;
using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.Props
{
    public class HealthBreather : MonoBehaviour
    {
        MainCHealthManager _mainHealth;

        private GameObject _playerObj;

        [SerializeField] private float healthPerHalfSecond = 4f;
        [SerializeField] private float speedOfHealth = .1f;

        private void Start()
        {
            _playerObj = GameObject.FindGameObjectWithTag(string.Format("Player"));
            _mainHealth = _playerObj.GetComponent<MainCHealthManager>();
        }

        public void StartHeal()
        {
            StartCoroutine(nameof(HealCoroutine));
            Debug.Log("hola3");
        }

        public void EndHeal()
        {
            StopCoroutine(nameof(HealCoroutine));
        }

        private IEnumerator HealCoroutine()
        {
            var enable = true;

            while(enable)
            {
                if (_mainHealth.currentHealth < _mainHealth.maxHealth)
                {
                    _mainHealth.GetHealth(healthPerHalfSecond);
                }
                else
                {
                    enable = false;
                }

                yield return new WaitForSeconds(speedOfHealth);
            }
        }
    }
}
