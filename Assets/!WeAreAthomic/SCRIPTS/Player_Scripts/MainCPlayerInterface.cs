using UnityEngine;
using UnityEngine.UI;

namespace _WeAreAthomic.SCRIPTS.Player_Scripts
{
    public class MainCPlayerInterface : MonoBehaviour
    {
        [SerializeField] private PP m_PP;
        [SerializeField] private GameObject m_interface;

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
        public void ChargeEnergy(int energy)
        {
            localEnergy += energy;
            GameManagerSingleton.Instance.bastetEnergy = localEnergy;
            SetEnergySlider(localEnergy);
        }

        public void BastetAbylitiesConsume(int consumeAbility)
        {
            
                localEnergy -= consumeAbility;
                GameManagerSingleton.Instance.bastetEnergy = localEnergy;
                SetEnergySlider(localEnergy);

        }
    }
}
