using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _WeAreAthomic.SCRIPTS.Player_Scripts
{
    public class MainCPlayerInterface : MonoBehaviour
    {
        [SerializeField] private Slider energySlider;


        public void SetEnergySlider(float value)
        {
            energySlider.value = value;
        }
    }
}
