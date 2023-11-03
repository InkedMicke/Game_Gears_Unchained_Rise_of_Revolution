using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _WeAreAthomic.SCRIPTS.Genericos
{
    public class GLoadScene : MonoBehaviour
    {
        public void LoadScene(string scene, LoadSceneMode sceneMode)
        {
            SceneManager.LoadScene(scene, sceneMode);
        }
    }
}
