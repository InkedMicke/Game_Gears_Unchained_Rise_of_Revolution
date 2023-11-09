using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Hedenrag 
{
    namespace SceneLoader
    {
        public class SceneLoaderMultipleAsync : MonoBehaviour
        {
            [SerializeField] LoadSceneAsyncAsset loadSceneAsyncAsset;

            public void LoadSceneAsync(Action onFinishAction)
            {
                _InternalLoadScene(onFinishAction);
            }

            public void LoadSceneAsync()
            {
                _InternalLoadScene(LogLoadLevel);
            }

            void _InternalLoadScene(Action action)
            {
                GameObject g = new GameObject("LoaderObject");
                g.AddComponent<LoaderObject>().StartCoroutine(LoadScene(action, g));
                //DontDestroyOnLoad(g);
            }

            void LogLoadLevel() { Debug.Log("Finished loading scene"); }

            IEnumerator LoadScene(Action onFinishAction, GameObject loaderObject)
            {
#pragma warning disable CS0618
                List<Scene> scenes = new(SceneManager.GetAllScenes());
#pragma warning restore CS0618

                List<string> scenesToLoad = new(loadSceneAsyncAsset.SceneNames);
                List<string> loadedScenes = new List<string>();
                List<AsyncOperation> loadedOperations = new List<AsyncOperation>();

                {
                    bool loadEmpty = true;
                    
                    string[] neverUnloadingScenes = SceneLoaderManager.GetNeverUnloadingScenes();

                    for (int i = scenes.Count - 1; i >= 0; i--)
                    {
                        if (scenes[i].name == SceneLoaderManager.emptySceneName)
                        {
                            scenes.RemoveAt(i);
                            loadEmpty = false;
                        }
                        else if (scenesToLoad.Contains(scenes[i].name))
                        {
                            scenesToLoad.Remove(scenes[i].name);
                            scenes.RemoveAt(i);
                        }else if (neverUnloadingScenes.Contains(scenes[i].name))
                        {
                            scenesToLoad.Remove(scenes[i].name);
                            scenes.RemoveAt(i);
                        }
                    }
                    if (loadEmpty)
                    {
                        SceneManager.LoadSceneAsync(SceneLoaderManager.emptySceneName, LoadSceneMode.Additive);
                    }
                }

                foreach (Scene scene in scenes)
                {
                    if (!scenesToLoad.Contains(scene.name) && scene.name != SceneLoaderManager.emptySceneName)
                    {
                        loadedOperations.Add(SceneManager.UnloadSceneAsync(scene));
                    }
                    else
                    {
                        loadedScenes.Add(scene.name);
                    }
                }

                while (!SceneLoaderAsync.AllOperationsFinished(loadedOperations))
                {
                    yield return new WaitForEndOfFrame();
                }

                foreach (string sceneName in scenesToLoad)
                {
                    loadedOperations.Add(SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive));
                }

                while (!SceneLoaderAsync.AllOperationsFinished(loadedOperations))
                {
                    yield return new WaitForEndOfFrame();
                }

                onFinishAction();
                Destroy(loaderObject);

#pragma warning disable CS0618
                Scene[] endScenes = SceneManager.GetAllScenes();
#pragma warning restore CS0618
                foreach (Scene scene in endScenes) 
                {
                    if(scene.name == loadSceneAsyncAsset.SceneNames[0])
                    {
                        SceneManager.SetActiveScene(scene);
                        break;
                    }
                }
                
            }
        }
    }
}