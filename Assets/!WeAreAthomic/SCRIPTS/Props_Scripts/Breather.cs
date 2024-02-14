using System.Collections;
using _WeAreAthomic.SCRIPTS.Player_Scripts;
using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.Props_Scripts
{
    public class Breather : MonoBehaviour
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
        [SerializeField] private GameObject volumeToActivate;


    
        private GameObject _playerObj;

        [SerializeField] private bool enableBreather;
        private bool _isHealing;

        [SerializeField] private float healthAmount = 4f;
        [SerializeField] private float healthPerTime = .1f;

        [SerializeField] private float energyAmount = 0.5f;
        [SerializeField] private float energyPerTime = 0.01f;

        [SerializeField] private AudioSource healthSound;

        private void Start()
        {
            _playerObj = GameObject.FindGameObjectWithTag(string.Format("Player"));
            _mainHealth = _playerObj.GetComponentInChildren<MainCHealthManager>();
            _mainCPlayer = _playerObj.GetComponent<MainCPlayerInterface>();
           

            if (!enableBreather)
            {
                DisableBreather();
            }
        }

        public void StartHeal()
        {
            if (enableBreather)
            {
                _isHealing = true;
                switch (_typeOfBreather)
                {
                    case TypeOfBreather.Health:
                        StartCoroutine(HealCoroutine());
                        volumeToActivate.SetActive(true);

                        break;

                    case TypeOfBreather.Energy:
                        StartCoroutine(EnergiCoroutine());
                        volumeToActivate.SetActive(true);

                        break;
                }


            }

        }

        public void EndHeal()
        {
            _isHealing = false;
            switch (_typeOfBreather)
            {

                case TypeOfBreather.Health:
                StopCoroutine(HealCoroutine());
                    volumeToActivate.SetActive(false);
                    break;
            case TypeOfBreather.Energy:
                StopCoroutine(EnergiCoroutine());
                    volumeToActivate.SetActive(false);
                    break;
            }
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
            healthSound.Play();

            while (_isHealing)
            {
                _mainHealth.GetHealth(healthAmount);
                

                yield return new WaitForSeconds(healthPerTime);
            }
        }
        private IEnumerator EnergiCoroutine()
        {
            healthSound.Play();

            while (_isHealing)
            {
                _mainCPlayer.ChargeEnergy(energyAmount);
                

                yield return new WaitForSeconds(energyPerTime);
            }
        }
    }
}
