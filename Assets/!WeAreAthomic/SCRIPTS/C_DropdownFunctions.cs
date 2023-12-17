using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class C_DropdownFunctions : MonoBehaviour
{
    [SerializeField] private GameObject[] objectsES;
    [SerializeField] private GameObject[] objectsEN;

    [SerializeField] TMP_Dropdown dropdown;
    [SerializeField] List<string> idiomasES;
    [SerializeField] List<string> idiomasEN;

    

    private void Start()
    {
        idiomasES.Add("ESPAÑOL");
        idiomasES.Add("INGLES");

        
        idiomasEN.Add("SPANISH");
        idiomasEN.Add("ENGLISH");


        dropdown.AddOptions(idiomasES);

    }

    public void OnChangeLanguage (int index)
    {
       
       
        switch (index)   
        {
            case 0 : //Español (DESDE ESPAÑOL)
             
                dropdown.ClearOptions();
                dropdown.AddOptions(idiomasES);
                ActivarObjetosES();
                DesactivarObjetosEN();
                dropdown.SetValueWithoutNotify(0);
                
                 break;

            case 1: //Ingles (DESDE ESPAÑOL)
                dropdown.ClearOptions();
                dropdown.AddOptions(idiomasEN);
                ActivarObjetosEN();
                DesactivarObjetosES();
                dropdown.SetValueWithoutNotify(1);

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
