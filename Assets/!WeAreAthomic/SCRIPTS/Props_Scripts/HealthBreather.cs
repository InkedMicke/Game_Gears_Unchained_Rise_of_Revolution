using System.Collections;
using _WeAreAthomic.SCRIPTS.Player_Scripts;
using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.Props
{
    public class HealthBreather : MonoBehaviour
    {
        private enum TypeOfBreather
        {
            Health,
            Energy
        }

        MainCHealthManager _mainHealth;
        MainCPlayerInterface _mainCPlayer;

        [SerializeField] TypeOfBreather _typeOfBreather;

        [SerializeField] private GameObject hurtbox;
        [SerializeField] private GameObject particleEffects;
        [SerializeField] private int energyFill;


        private GameObject _volumeHealing;
        private GameObject _playerObj;

        [SerializeField] private bool enableBreather;

        [SerializeField] private float healthAmount = 4f;
        [SerializeField] private float healthPerTime = .1f;

        [SerializeField] private AudioSource healthSound;

        private void Start()
        {
            _playerObj = GameObject.FindGameObjectWithTag(string.Format("Player"));
            _mainHealth = _playerObj.GetComponentInChildren<MainCHealthManager>();
            _mainCPlayer = _playerObj.GetComponent<MainCPlayerInterface>();
            _volumeHealing = _playerObj.transform.GetChild(_playerObj.transform.childCount - 1).gameObject;

            if (!enableBreather)
            {
                DisableBreather();
            }
        }

        public void StartHeal()
        {
            if (enableBreather)
            {
                switch (_typeOfBreather)
                {
                    case TypeOfBreather.Health:
                        StartCoroutine(nameof(HealCoroutine));
                        _volumeHealing.SetActive(true);
                        break;

                    case TypeOfBreather.Energy:
                        break;
                }

                StartCoroutine(nameof(HealCoroutine));
                _volumeHealing.SetActive(true);
            }

        }

        public void EndHeal()
        {
            StopCoroutine(nameof(HealCoroutine));
            _volumeHealing.SetActive(false);
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
            healthSound.Play();

            while (enable)
            {
                if (_mainHealth.currentHealth < _mainHealth.maxHealth)
                {
                    _mainHealth.GetHealth(healthAmount);
                }
                else
                {
                    enable = false;
                }

                yield return new WaitForSeconds(healthPerTime);
            }
        }
        private IEnumerator EnergiCoroutine()
        {
            var enable = true;
            healthSound.Play();

            while (enable)
            {
                if (_mainCPlayer.localEnergy < _mainCPlayer.maxEnergy)
                {
                    _mainCPlayer.ChargeEnergy(energyFill);
                }
                else
                {
                    enable = false;
                }

                yield return new WaitForSeconds(healthPerTime);
            }
        }
    }
}
