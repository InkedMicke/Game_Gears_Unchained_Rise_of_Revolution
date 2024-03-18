using _WeAreAthomic.SCRIPTS.LightKiller;
using _WeAreAthomic.SCRIPTS.Player_Scripts;
using _WeAreAthomic.SCRIPTS.Props_Scripts;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


namespace Player
{
    public class MaterialChangeOnDetected : MonoBehaviour
    {
        private MainCHackingSystem _mainCHacking;

        private MeshRenderer m_MeshRenderer;

        [SerializeField] private Material neutralMat;
        [SerializeField] private Material catchMat;

        [SerializeField] private GameObject buttonObj;
        private GameObject _playerObj;

        [SerializeField] private bool ifCached;
        [SerializeField] private bool ifCannotHack;

        private void Awake()
        {
            m_MeshRenderer = GetComponent<MeshRenderer>();
            _playerObj = GameObject.FindGameObjectWithTag("Player");
            _mainCHacking = _playerObj.GetComponent<MainCHackingSystem>();
        }

        private void Update()
        {
            if (ifCached && !ifCannotHack)
            {
                if (_mainCHacking.GotCached)
                {
                    m_MeshRenderer.material = catchMat;
                }
                else
                {
                    m_MeshRenderer.material = neutralMat;
                }
            }

            if (ifCannotHack)
            {

                if (_mainCHacking.GotCached)
                {
                    m_MeshRenderer.material = catchMat;
                }
                else
                {
                    m_MeshRenderer.material = buttonObj.GetComponent<ButtonInteractable>().canHack ? neutralMat : catchMat;
                }
            }
        }



    }
}