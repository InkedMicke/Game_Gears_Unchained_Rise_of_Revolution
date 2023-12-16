using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class C_DropdownFunctions : MonoBehaviour
{
    [SerializeField] private GameObject[] objectsES;
    [SerializeField] private GameObject[] objectsEN;

  


    public void OnChangeLanguage (int index)
    {
       
        switch (index)
            
        {
            case 0 :
               
                ActivarObjetosES();
                DesactivarObjetosEN();    
                
                    break;

            case 1:
               
                ActivarObjetosEN();
                DesactivarObjetosES();

                break;
                
        }
    }

    private void ActivarObjetosES()
    {
        foreach (var obj in objectsES)
        {
            obj.SetActive(true);
        }

        
    }
    private void DesactivarObjetosES()
    {
        foreach (var obj in objectsES)
        {
            obj.SetActive(false);
        }

    }

    private void ActivarObjetosEN()
    {
        foreach (var obj in objectsEN)
        {
            obj.SetActive(true);
        }


    }
    private void DesactivarObjetosEN()
    {
        foreach (var obj in objectsEN)
        {
            obj.SetActive(false);
        }

    }


}
