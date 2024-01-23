using _WeAreAthomic.SCRIPTS.Genericos_Scripts;
using _WeAreAthomic.SCRIPTS.Player_Scripts;
using UnityEngine;
using UnityEngine.Events;

namespace _WeAreAthomic.SCRIPTS.Props_Scripts
{
    public class LightKillerHurtBox : HurtBox
    {
        private MainCHackingSystem _mainCHack;
        private LightKiller _lightK;

        /// <summary>
        /// If the bool "isConntectedToOtherLight" was set to true,
        /// will try to turn red the lightKillers inside array
        /// </summary>
        [SerializeField] private GameObject[] lightsToTurnRed;
        private GameObject _playerObj;
        /// <summary>
        /// Set this bool to true if u want to turn red another light around
        /// </summary>
        [SerializeField] private bool isConntectedToOtherLight;

        [SerializeField] private UnityEvent seEjecutaCuandoDetectaAlPlayer;

        private void Awake()
        {
            _playerObj = GameObject.FindGameObjectWithTag("Player");
            _mainCHack = _playerObj.GetComponent<MainCHackingSystem>();

            _lightK = transform.parent.GetComponent<LightKiller>();
        }

        protected override void GotEnterCollision(Collider col)
        {
            if (isConntectedToOtherLight)
            {
                for (int i = 0; i < lightsToTurnRed.Length; i++)
                {
                    lightsToTurnRed[i].GetComponent<LightKiller>().RedLight();
                }
            }

            seEjecutaCuandoDetectaAlPlayer.Invoke();
            if (_mainCHack.IsHacking || _mainCHack.isHackingAnim)
            {
                _mainCHack.StopHack();
            }

            _lightK.RedLight();
            base.GotEnterCollision(col);
        }
    }
}
