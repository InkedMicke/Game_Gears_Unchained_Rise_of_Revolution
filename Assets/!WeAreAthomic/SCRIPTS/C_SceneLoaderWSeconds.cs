using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class C_SceneLoaderWSeconds : MonoBehaviour
{
    [SerializeField] private string sceneName;

    // Start is called before the first frame update
    public void LoadSceneWaitForSeconds(float delayInSeconds)
    {
        StartCoroutine(LoadSceneAfterDelay(delayInSeconds));
    }

    private IEnumerator LoadSceneAfterDelay(float delayInSeconds)
    {
        yield return new WaitForSeconds(delayInSeconds);
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
