using Hedenrag.SceneLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderStart : MonoBehaviour
{
    SceneLoaderMultipleAsync _sceneLoaderMultipleAsync;
     [SerializeField] public List<LoadSceneAsyncAsset> loadSceneAsyncAssets = new ();
    [SerializeField] private List <GameObject> player;
    [NonSerialized] public int currentSceneIndex;
    [SerializeField] public float timeToLoad;
    

    // Start is called before the first frame update
    private void Awake()
    {
        _sceneLoaderMultipleAsync = GetComponent<SceneLoaderMultipleAsync>();   
    }
  
    
    public void StartLoadingScenes()
    {
        foreach (var x in player)
        {
            x.SetActive(false);
        }

        StartCoroutine(LoadingScenes());
    }

    IEnumerator LoadingScenes()
    {
        yield return new WaitForSeconds(timeToLoad);
        while (currentSceneIndex < loadSceneAsyncAssets.Count )
        {
            _sceneLoaderMultipleAsync.loadSceneAsyncAsset = loadSceneAsyncAssets[currentSceneIndex];
            _sceneLoaderMultipleAsync.LoadSceneAsync();
            currentSceneIndex++;
            yield return new WaitForSeconds(timeToLoad);

        }
        GameManagerSingleton.Instance.SetIsLoadingStartVideos(false);

        foreach (var x in player)
        {
            x.SetActive(true);
        }
        
    }

    public void StartCheckingLoadedScenes(Action onFinishedAction)
    {
        StartCoroutine(CheckScenesLoad(onFinishedAction));
    }

    private IEnumerator CheckScenesLoad(Action onFinishedAction)
    {
        while(currentSceneIndex < loadSceneAsyncAssets.Count)
        {
            yield return new WaitForEndOfFrame();
        }

        onFinishedAction();
    }
}
