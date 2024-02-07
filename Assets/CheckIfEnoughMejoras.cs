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
                Debug.Log("hola2");
                continue;
            }
            if (GameManagerSingleton.Instance.gearsItem >= ability.priceToBuy)
            {
                Debug.Log("hola3");
                auraToActivate.SetActive(true);
                break;
            }
            else
            {
                Debug.Log("hola4");
                auraToActivate.SetActive(false);
            }
        }
    }
}
