using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
            if (ability.priceToBuy <= GameManagerSingleton.Instance.gearsItem)
            {
                Debug.Log("Socorro");
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
