using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.UI.CanvasScaler;

namespace Generics.Scene
{

    public enum Scenes
    {
        MENU,
        LAB,
        POOR,
        SEWERS,
        SETHS,
        BATTLE,
        CREDITS
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
                    SceneManager.LoadScene("N1_MAINMENU", sceneMode);
                    break;                
                case Scenes.LAB:
                    SceneManager.LoadScene("N2_LAB", sceneMode);
                    break;                
                case Scenes.POOR:
                    SceneManager.LoadScene("N3_DISTRITO_III", sceneMode);
                    break;                
                case Scenes.SEWERS:
                    SceneManager.LoadScene("N4_SEWERS", sceneMode);
                    break;              
                case Scenes.SETHS:
                    SceneManager.LoadScene("N5_SETHS_PALACE", sceneMode);
                    break;
                case Scenes.BATTLE:
                    SceneManager.LoadScene("N6_SETHS_BATTLE", sceneMode);
                    break;
                case Scenes.CREDITS:
                    SceneManager.LoadScene("N7_Creditos", sceneMode);
                    break;
            }
        }
        
        public void LoadScene()
        {
            switch (sceneToLoad)
            {
                case Scenes.MENU:
                    SceneManager.LoadScene("N1_MAINMENU");
                    break;
                case Scenes.LAB:
                    SceneManager.LoadScene("N2_LAB");
                    break;
                case Scenes.POOR:
                    SceneManager.LoadScene("N3_DISTRITO_III");
                    break;
                case Scenes.SEWERS:
                    SceneManager.LoadScene("N4_SEWERS");
                    break;
                case Scenes.SETHS:
                    SceneManager.LoadScene("N5_SETHS_PALACE");
                    break;
                case Scenes.BATTLE:
                    SceneManager.LoadScene("N6_SETHS_BATTLE");
                    break;
                case Scenes.CREDITS:
                    SceneManager.LoadScene("N7_Creditos");
                    break;
            }
        }


     
    }
}
