using System;
using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.Props_Scripts
{
    public class LightKiller : MonoBehaviour
    {
        private CctvController _controller;

        public Action OnWhiteLight;

        public GameObject luzObj;
        public GameObject cctv;
        private Light _luz;

        [SerializeField] private AudioSource redLightSound;

        [System.NonSerialized] public bool IsFocusingPlayer;

        private void Awake()
        {
            _luz = luzObj.GetComponent<Light>();
            _controller = cctv.GetComponent<CctvController>();
        }

        public void WhiteLight()
        {

            _luz.color = Color.white;
            OnWhiteLight?.Invoke();
            IsFocusingPlayer = false;
            if (redLightSound.isPlaying)
            {
                redLightSound.Stop();
            }
        }

        public void RedLight()
        {
            _luz.color = Color.red;
            IsFocusingPlayer = true;
            if (!redLightSound.isPlaying)
            {
                redLightSound.Play();
            }

        }
    }
}
