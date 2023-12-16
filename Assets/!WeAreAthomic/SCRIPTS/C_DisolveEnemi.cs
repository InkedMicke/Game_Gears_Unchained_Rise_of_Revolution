using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class C_DisolveEnemi : MonoBehaviour
{
    public SkinnedMeshRenderer[] skinnedMesh;
    //public VisualEffect VFXGraph;

    public float dissolveRate = 0.001f;
    public float refreshRate = 0.001f;

    public Material[] skinnedMaterials;


    void Start()
    {
        if (skinnedMesh != null)
            skinnedMaterials = skinnedMesh[0].materials;
    }


    public void StartDisolving()
    {

        StartCoroutine(DisolveCo());
    }

    IEnumerator DisolveCo()
    {
        //if (VFXGraph != null)
        //{
        //    VFXGraph.Play();

        //}
        if (skinnedMaterials.Length > 0)
        {

            float counter = 0;

            while (skinnedMaterials[0].GetFloat("_DisolveAmount") < 1)
            {
                //Decrese
                counter += dissolveRate;
                for (int i = 0; i < skinnedMaterials.Length; i++)
                {
                    skinnedMaterials[i].SetFloat("_DisolveAmount", counter);
                }
                yield return new WaitForSeconds(refreshRate);

            }
        }
    }

}
