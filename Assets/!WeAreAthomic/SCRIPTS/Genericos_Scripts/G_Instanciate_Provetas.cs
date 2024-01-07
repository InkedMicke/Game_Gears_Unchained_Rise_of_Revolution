

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class G_Instanciate_Provetas : MonoBehaviour
{
    [SerializeField] private List<GameObject> provetas;
    [SerializeField] private Transform objPosition;
    [SerializeField] private float secondsDelay = 3f;

    private void Start()
    {
        StartCoroutine(InstanciateObj());
    }

    IEnumerator InstanciateObj()
    {
        while (true)  // Este bucle permite que la corrutina se ejecute continuamente
        {
            var random = Random.Range(0, provetas.Count);

            Instantiate(provetas[random], objPosition.position, Quaternion.identity);

            yield return new WaitForSeconds(secondsDelay);
        }
    }
}