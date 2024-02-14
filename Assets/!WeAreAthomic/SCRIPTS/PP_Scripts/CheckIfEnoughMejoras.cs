using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.PP_Scripts
{
    public class CheckIfEnoughMejoras : MonoBehaviour
    {
        [SerializeField] private GameObject auraToActivate;
        public void CheckIfYouHaveEnoughToBuy()
        {
            foreach (var ability in GameManagerSingleton.Instance.abiltiesList)
            {
                if (ability.IsUnlocked)
                {
                    continue;
                }
                if (GameManagerSingleton.Instance.gearsItem >= ability.priceToBuy)
                {
                    auraToActivate.SetActive(true);
                    break;
                }
                else
                {
                    auraToActivate.SetActive(false);
                }
            }
        }
    }
}
