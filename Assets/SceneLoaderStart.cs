using Hedenrag.SceneLoader;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoaderStart : MonoBehaviour
{
    SceneLoaderMultipleAsync _sceneLoaderMultipleAsync;
     [SerializeField] private List<LoadSceneAsyncAsset> loadSceneAsyncAssets = new List<LoadSceneAsyncAsset>();
    [SerializeField] private GameObject player;
    private int currentSceneIndex;
    [SerializeField] private float timeToLoad;

    // Start is called before the first frame update
    private void Awake()
    {
        _sceneLoaderMultipleAsync = GetComponent<SceneLoaderMultipleAsync>();   
    }
    void Start()
    {
        player.SetActive(true);
        StartCoroutine(LoadingScenes());
    }

    IEnumerator LoadingScenes()
    {
        while (currentSceneIndex < loadSceneAsyncAssets.Count )
        {

            _sceneLoaderMultipleAsync.loadSceneAsyncAsset = loadSceneAsyncAssets[currentSceneIndex];
            _sceneLoaderMultipleAsync.LoadSceneAsync();
            currentSceneIndex++;
            yield return new WaitForSeconds(timeToLoad);
        }
    
    }
}
