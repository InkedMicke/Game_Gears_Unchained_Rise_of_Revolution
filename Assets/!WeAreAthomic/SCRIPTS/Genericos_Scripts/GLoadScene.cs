using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _WeAreAthomic.SCRIPTS.Genericos
{
    public class GLoadScene : MonoBehaviour
    {
        public void LoadSceneMode(string scene, LoadSceneMode sceneMode)
        {
            SceneManager.LoadScene(scene, sceneMode);
        }
        
        public void LoadScene(string scene)
        {
            SceneManager.LoadScene(scene, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }
}
