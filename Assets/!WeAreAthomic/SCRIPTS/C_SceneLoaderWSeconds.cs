using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class C_SceneLoaderWSeconds : MonoBehaviour
{
    [SerializeField] private SceneAsset _scene;
    // Start is called before the first frame update
    public void LoadSceneWaitForSeconds( float delayInSeconds)
    {
        StartCoroutine(LoadSceneAfterDelay( delayInSeconds));
    }

    private IEnumerator LoadSceneAfterDelay( float delayInSeconds)
    {
        yield return new WaitForSeconds(delayInSeconds);
        SceneManager.LoadScene(_scene.name, LoadSceneMode.Single);
    }
}
