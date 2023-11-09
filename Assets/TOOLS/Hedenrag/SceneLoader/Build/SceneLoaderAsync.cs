using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Hedenrag
{
    namespace SceneLoader
    {

        public class SceneLoaderAsync : MonoBehaviour
        {
#if UNITY_EDITOR
            [SerializeField] SceneAsset sceneAssets;
#endif
            [SerializeField, HideInInspector] string sceneName;

#if UNITY_EDITOR
            private void OnValidate()
            {
                sceneName = sceneAssets.name;
            }
#endif

            public void LoadScene(Action onFinishAction)
            {
                GameObject g = new GameObject("LoaderObject");
                g.AddComponent<LoaderObject>().StartCoroutine(LoadSceneAsync(sceneName, onFinishAction, g));
            }
            public void LoadSceneAsync()
            {
                GameObject g = new GameObject("LoaderObject");
                g.AddComponent<LoaderObject>().StartCoroutine(LoadSceneAsync(sceneName, LogLoadLevel, g));
            }

            void LogLoadLevel() { Debug.Log("Finished loading scene"); }

            static IEnumerator LoadSceneAsync(string sceneName, Action onFinishAction, GameObject g)
            {
#pragma warning disable CS0618 
                List<Scene> scenes = new(SceneManager.GetAllScenes());
#pragma warning restore CS0618

                List<AsyncOperation> loadedOperations = new List<AsyncOperation>();
                bool alreadyLoaded = false;
                string[] neverUnloadingScenes = SceneLoaderManager.GetNeverUnloadingScenes();

                // Remove all necessary scenes
                for (int i = scenes.Count - 1; i >= 0; i--)
                {
                    if (scenes[i].name == SceneLoaderManager.emptySceneName)
                    {
                        scenes.RemoveAt(i);
                    }
                    else if (scenes[i].name == sceneName)
                    {
                        scenes.RemoveAt(i);
                        alreadyLoaded = true;
                    }
                    else if (neverUnloadingScenes.Contains(scenes[i].name))
                    {
                        scenes.RemoveAt(i);
                    }
                }

                //Unload not needed scenes
                foreach (Scene scene in scenes)
                {
                    loadedOperations.Add(SceneManager.UnloadSceneAsync(scene));
                }

                while (!AllOperationsFinished(loadedOperations))
                {
                    yield return new WaitForEndOfFrame();
                    Debug.Log(loadedOperations.Count);
                }

                //Look if desired scene is already loaded
                if (!alreadyLoaded)
                {
                    //Load desired scene
                    AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

                    while (!operation.isDone)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                }
                else
                {
                    Debug.Log("sceneToLoad already loaded");
                }

                onFinishAction();

#pragma warning disable CS0618
                Scene[] endScenes = SceneManager.GetAllScenes();
#pragma warning restore CS0618
                foreach (Scene scene in endScenes)
                {
                    if(scene.name == sceneName)
                    {
                        SceneManager.SetActiveScene(scene);
                    }
                }
                Destroy(g);
            }

            public static bool AllOperationsFinished(List<AsyncOperation> asyncOperations)
            {
                for (int i = asyncOperations.Count - 1; i >= 0; i--)
                {
                    if (asyncOperations[i].isDone)
                    {
                        asyncOperations.RemoveAt(i);
                    }
                }
                if (asyncOperations.Count == 0) return true;
                return false;
            }


        }
    }
}
           
