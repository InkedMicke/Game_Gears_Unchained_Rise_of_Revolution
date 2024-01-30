using UnityEngine;
using UnityEngine.SceneManagement;

namespace _WeAreAthomic.SCRIPTS.Genericos_Scripts
{

    public enum Scenes
    {
        MENU,
        LAB,
        POOR,
        SEWERS,
        SETHS,
        SURVIVE
    }

    public class GLoadScene : MonoBehaviour
    {
        private C_SceneLoaderWSeconds sceneLoader;

        public Scenes sceneToLoad;

        public void LoadSceneMode(Scenes scene, LoadSceneMode sceneMode)
        {
            switch (scene)
            {
                case Scenes.MENU:
                    SceneManager.LoadScene("S1_MAINMENU", sceneMode);
                    break;                
                case Scenes.LAB:
                    SceneManager.LoadScene("S2_LABTUTORIAL", sceneMode);
                    break;                
                case Scenes.POOR:
                    SceneManager.LoadScene("S3_POORCITY", sceneMode);
                    break;                
                case Scenes.SEWERS:
                    SceneManager.LoadScene("S4_SEWERS_UNDERTHECITY", sceneMode);
                    break;              
                case Scenes.SETHS:
                    SceneManager.LoadScene("S5_SETHS_PALACE", sceneMode);
                    break;
                case Scenes.SURVIVE:
                    SceneManager.LoadScene("S6_SURVIVALMODE", sceneMode);
                    break;
            }
        }
        
        public void LoadScene()
        {
            switch (sceneToLoad)
            {
                case Scenes.MENU:
                    SceneManager.LoadScene("S1_MAINMENU");
                    break;
                case Scenes.LAB:
                    SceneManager.LoadScene("S2_LABTUTORIAL");
                    break;
                case Scenes.POOR:
                    SceneManager.LoadScene("S3_POORCITY");
                    break;
                case Scenes.SEWERS:
                    SceneManager.LoadScene("S4_SEWERS_UNDERTHECITY");
                    break;
                case Scenes.SETHS:
                    SceneManager.LoadScene("S5_SETHS_PALACE");
                    break;
                case Scenes.SURVIVE:
                    SceneManager.LoadScene("S6_SURVIVALMODE");
                    break;
            }
        }

        public void LoadSceneWSeconds()
        {
            sceneLoader = GetComponent<C_SceneLoaderWSeconds>();
            sceneLoader.LoadSceneWaitForSeconds("S1_1_EscenaIntro", 5.0f);
        }

     
    }
}
