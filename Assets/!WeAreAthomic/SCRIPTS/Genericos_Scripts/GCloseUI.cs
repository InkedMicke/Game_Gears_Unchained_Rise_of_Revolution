using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GCloseUI : MonoBehaviour
{
    [SerializeField] private GameObject stopMenu;
    public void Close(GameObject obj)
    {
        if (GameManagerSingleton.Instance.IsGamePaused) 
        {
            stopMenu.SetActive(true);
        }
        else
        {

        }

        obj.SetActive(false);
    }
}
