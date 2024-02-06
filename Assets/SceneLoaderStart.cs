using Hedenrag.SceneLoader;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderStart : MonoBehaviour
{
    SceneLoaderMultipleAsync _sceneLoaderMultipleAsync;
     [SerializeField] private List<LoadSceneAsyncAsset> loadSceneAsyncAssets = new List<LoadSceneAsyncAsset>();
    [SerializeField] private GameObject player;
    private int currentSceneIndex;
    [SerializeField] private float timeToLoad;
    private Scene currentSceneLoaded;
    

    // Start is called before the first frame update
    private void Awake()
    {
        _sceneLoaderMultipleAsync = GetComponent<SceneLoaderMultipleAsync>();   
    }
  
    
    public void StartLoadingScenes()
    {
        player.SetActive(true);
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
    
    }
}
