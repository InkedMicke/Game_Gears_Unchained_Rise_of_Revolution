using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class MainCHurtedMaterial : MonoBehaviour
    {
        [SerializeField] private SkinnedMeshRenderer _mattSkinned;
        private Material[] originalMaterials;
        [SerializeField] private Material HurtMaterials;
        [SerializeField] private float hurtDuration;

        private bool _isHurtEnabled;
        private void Awake()
        {
            originalMaterials = _mattSkinned.materials;
        }

        public void HurtEffects()
        {

            var newMat = new Material[_mattSkinned.materials.Length];

            for (int i = 0; i < newMat.Length; i++)
            {
                newMat[i] = HurtMaterials;
            }

            _mattSkinned.materials = newMat;

            StartCoroutine(HurtDuration());


        }

        private IEnumerator HurtDuration()
        {
            yield return new WaitForSeconds(hurtDuration);
            _mattSkinned.materials = originalMaterials;
        }
    }
}