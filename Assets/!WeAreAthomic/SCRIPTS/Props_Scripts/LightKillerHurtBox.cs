using Player;
using Generics.Tween;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Generics.Collision;

namespace _WeAreAthomic.SCRIPTS.Props_Scripts
{
    public class LightKillerHurtBox : HurtBox
    {
        private MainCHackingSystem _mainCHack;
        private LightKiller _lightK;
        private CctvController ctvController;

        private TypeOfLightKillerHurtBox TypeOfLightHurtBox => ctvController.TypeOfLightHurtBox;

        /// <summary>
        /// If the bool "isConntectedToOtherLight" was set to true,
        /// will try to turn red the lightKillers inside array
        /// </summary>
        [SerializeField] private GameObject[] lightsToTurnRed;
        private GameObject m_playerObj;
        /// <summary>
        /// Set this bool to true if u want to turn red another light around
        /// </summary>
        [SerializeField] private bool isConntectedToOtherLight;

        private float m_hitCooldown = 1.5f;

        private float m_totalTimeCooldown;

        public UnityEvent seEjecutaCuandoDetectaAlPlayer;

        protected void Awake()
        {
            m_playerObj = GameObject.FindGameObjectWithTag("Player");
            _mainCHack = m_playerObj.GetComponent<MainCHackingSystem>();
            ctvController = transform.parent.parent.GetChild(0).GetComponent<CctvController>();

            _lightK = transform.parent.GetComponent<LightKiller>();
        }

        public override void GotEnterCollision(Collider col = null)
        {
            if (Time.time > m_totalTimeCooldown)
            {
                m_totalTimeCooldown = Time.time + m_hitCooldown;
                StartCoroutine(CollisionThings());

            }
            base.GotEnterCollision(col);
        }

        private IEnumerator CollisionThings()
        {
            switch (TypeOfLightHurtBox)
            {
                case TypeOfLightKillerHurtBox.tutorial:
                    if (isConntectedToOtherLight)
                    {
                        for (int i = 0; i < lightsToTurnRed.Length; i++)
                        {
                            lightsToTurnRed[i].GetComponent<LightKiller>().RedLight();
                        }
                    }

                    if (ctvController.GroupCCtvController != null)
                    {
                        if (ctvController.HasGroupCamera)
                        {
                            _mainCHack.SetGotCached(true);
                            if (_mainCHack.IsHacking || _mainCHack.isHackingAnim)
                            {
                                _mainCHack.StopHack();
                            }

                            _lightK.RedLight();
                            yield return new WaitForSeconds(0.1f);
                            ctvController.MainCMove.DisableMovement();
                            ctvController.MainCAnim.SetMoveSpeed(0);
                            ctvController.PP.FadeBlackTutorial.GetComponent<GTweenDoFade>().Fade();

                            yield return new WaitForSeconds(1f);
                            _lightK.WhiteLight();
                            yield return new WaitForSeconds(.5f);

                            ctvController.MainCHealth.Revive();
                            ctvController.GroupCCtvController.SendColToHurtBox();
                            break;
                        }
                    }

                    _mainCHack.SetGotCached(true);
                    if (_mainCHack.IsHacking || _mainCHack.isHackingAnim)
                    {
                        _mainCHack.StopHack();
                    }

                    _lightK.RedLight();
                    yield return new WaitForSeconds(0.1f);
                    ctvController.MainCMove.DisableMovement();
                    ctvController.MainCAnim.SetMoveSpeed(0);
                    ctvController.PP.FadeBlackTutorial.GetComponent<GTweenDoFade>().Fade();

                    yield return new WaitForSeconds(1f);
                    _lightK.WhiteLight();
                    yield return new WaitForSeconds(.5f);

                    seEjecutaCuandoDetectaAlPlayer.Invoke();

                    break;

                case TypeOfLightKillerHurtBox.normal:
                    seEjecutaCuandoDetectaAlPlayer.Invoke();
                    _mainCHack.SetGotCached(true);
                    if (_mainCHack.IsHacking || _mainCHack.isHackingAnim)
                    {
                        _mainCHack.StopHack();
                    }

                    _lightK.RedLight();
                    break;
            }
        }
    }
}
