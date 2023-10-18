using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.LightKiller
{
    public class LightKiller : MonoBehaviour
    {
        CCTVController controller;

        public GameObject luzObj;
        public GameObject cctv;
        Light _luz;

        [System.NonSerialized] public bool isFocusingPlayer;

        private void Awake()
        {
            _luz = luzObj.GetComponent<Light>();
            controller = cctv.GetComponent<CCTVController>();
        }

        public void WhiteLight()
        {
            _luz.color = Color.white;
            isFocusingPlayer = false;
        }

        public void RedLight()
        {
            if (!controller.rayGotObstruction)
            {
                _luz.color = Color.red;
                isFocusingPlayer = true;
            }
        }
    }
}
