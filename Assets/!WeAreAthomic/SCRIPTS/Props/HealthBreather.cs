using System.Collections;
using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.Props
{
    public class HealthBreather : MonoBehaviour
    {
        MainCHealthManager _mainHealth;

        [SerializeField] private GameObject hurtbox;
        [SerializeField] private GameObject particleEffects;
        private GameObject _playerObj;

        [SerializeField] private bool enableBreather;

        [SerializeField] private float healthPerHalfSecond = 4f;
        [SerializeField] private float speedOfHealth = .1f;

        private void Start()
        {
            _playerObj = GameObject.FindGameObjectWithTag(string.Format("Player"));
            _mainHealth = _playerObj.GetComponent<MainCHealthManager>();

            if(!enableBreather)
            {
                DisableBreather();
            }
        }

        public void StartHeal()
        {
            if (enableBreather)
            {
                StartCoroutine(nameof(HealCoroutine));
            }
        }

        public void EndHeal()
        {
            StopCoroutine(nameof(HealCoroutine));
        }

        public void EnableBreather()
        {
            enableBreather = true;
            hurtbox.SetActive(true);
            particleEffects.SetActive(true);
            particleEffects.GetComponent<ParticleSystem>().Play();
        }

        public void DisableBreather()
        {
            enableBreather = false;
            hurtbox.SetActive(false);
            particleEffects.SetActive(false);
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
