using _WeAreAthomic.SCRIPTS.PP_Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class MainCPlayerInterface : MonoBehaviour
    {
        [SerializeField] private PP m_PP;
        [SerializeField] private GameObject m_interface;

        [SerializeField] private Slider furySlider;

        public float localEnergy;
        public float maxEnergy = 100;

        private void Awake()
        {
            localEnergy = GameManagerSingleton.Instance.bastetEnergy;
            SetEnergySlider(GameManagerSingleton.Instance.bastetEnergy);
            m_PP.AddObjToCurrentUIGameObjectList(m_interface);
        }

        [SerializeField] private Slider energySlider;


        public void SetEnergySlider(float value)
        {
            energySlider.value = value;
        }        
        
        public void SetFurySlider(float value)
        {
            furySlider.value = value;
        }
        public void ChargeEnergy(float energy)
        {
            localEnergy += energy;
            if(localEnergy > maxEnergy)
            {
                localEnergy = maxEnergy;
            }
            GameManagerSingleton.Instance.bastetEnergy = localEnergy;
            SetEnergySlider(localEnergy);
        }

        public void TakeEnergy(float energy)
        {
            localEnergy -= energy;
            GameManagerSingleton.Instance.bastetEnergy = localEnergy;
            SetEnergySlider(localEnergy);

        }
    }
}
