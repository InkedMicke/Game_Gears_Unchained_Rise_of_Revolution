using _WeAreAthomic.SCRIPTS.Enemi_Scripts.Dummie;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ElevatorLabChecker : MonoBehaviour
{
    [SerializeField] private UnityEvent AllDead;

   public List<GameObject> dummies = new();
  public void CheckIfAllDeath()
    {
        StartCoroutine(CheckDumieDead());
    }
    IEnumerator CheckDumieDead()
    {
        while (true) 
        {
            yield return new WaitForSeconds(2F);


            if (dummies.Count == 0)
            {
                AllDead.Invoke();
                break;
            }

            dummies.RemoveAll(item => item == null);

        }

    }

    public void CheckForDummies()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if(transform.GetChild(i).GetComponent<DummieController>())
            {
                dummies.Add(item: transform.GetChild(i).gameObject);
            }
            
        }
    }
     
}
