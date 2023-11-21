using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _WeAreAthomic.SCRIPTS.Player_Scripts
{
    public class MainCPlayerInterface : MonoBehaviour
    {

        public float localEnergy;

        private void Awake()
        {
            localEnergy = GameManagerSingleton.Instance.bastetEnergy;
            SetEnergySlider(GameManagerSingleton.Instance.bastetEnergy);
        }

        [SerializeField] private Slider energySlider;


        public void SetEnergySlider(float value)
        {
            energySlider.value = value;
        }
    }
}
