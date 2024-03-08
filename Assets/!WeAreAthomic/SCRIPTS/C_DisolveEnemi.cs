using Enemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generics
{
    public class C_DisolveEnemi : MonoBehaviour
    {
        [SerializeField] SoldierHurtBox hurtbox;

        public SkinnedMeshRenderer[] skinnedMesh;
        public float dissolveRate = 0.001f;
        public float refreshRate = 0.001f;

        private Material[] skinnedMaterials;

        void OnEnable()
        {
            CollectAllMaterials();
            hurtbox.OnDeath += StartDisolving;
        }

        private void OnDisable()
        {
            hurtbox.OnDeath -= StartDisolving;
        }

        void CollectAllMaterials()
        {
            List<Material> materialsList = new List<Material>();

            foreach (SkinnedMeshRenderer renderer in skinnedMesh)
            {
                if (renderer != null)
                {
                    materialsList.AddRange(renderer.materials);
                }
            }

            skinnedMaterials = materialsList.ToArray();

            // Inicia la disolución después de recoger los materiales si el objeto ya está activo
            if (gameObject.activeSelf)
            {
                StartDisolving();
            }
        }

        public void StartDisolving()
        {
            StartCoroutine(DisolveCo());
        }

        IEnumerator DisolveCo()
        {
            if (skinnedMaterials != null && skinnedMaterials.Length > 0)
            {
                float counter = 0;

                while (skinnedMaterials[0].GetFloat("_DisolveAmount") < 1)
                {
                    counter += dissolveRate;
                    for (int i = 0; i < skinnedMaterials.Length; i++)
                    {
                        skinnedMaterials[i].SetFloat("_DisolveAmount", counter);
                    }
                    yield return null;  // Utiliza Time.deltaTime para un control más preciso
                }
            }
        }
    } 
}