using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class C_SceneLoaderWSeconds : MonoBehaviour
{
    // Start is called before the first frame update
    public void LoadSceneWaitForSeconds(string scene, float delayInSeconds)
    {
        StartCoroutine(LoadSceneAfterDelay(scene, delayInSeconds));
    }

    private IEnumerator LoadSceneAfterDelay(string scene, float delayInSeconds)
    {
        yield return new WaitForSeconds(delayInSeconds);
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }
}
