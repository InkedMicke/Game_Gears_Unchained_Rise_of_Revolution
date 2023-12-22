using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _WeAreAthomic.SCRIPTS.Genericos_Scripts
{
    public class GLoadScene : MonoBehaviour
    {
        private C_SceneLoaderWSeconds sceneLoader;

      

        public void LoadSceneMode(string scene, LoadSceneMode sceneMode)
        {
            SceneManager.LoadScene(scene, sceneMode);
        }
        
        public void LoadScene(string scene)
        {
            SceneManager.LoadScene(scene, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }

        public void LoadSceneWSeconds()
        {
            sceneLoader = GetComponent<C_SceneLoaderWSeconds>();
            sceneLoader.LoadSceneWaitForSeconds("S1_1_EscenaIntro", 5.0f);
        }

     
    }
}
