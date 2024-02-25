using System.Collections;
using UnityEngine;

public class LoadingScreenLAB : MonoBehaviour
{
    [SerializeField] private SceneLoaderStart m_sceneLoaderStart;

    public void StartCheckingScenesLoad()
    {
        StartCoroutine(CheckScenesLoad());
    }

    private IEnumerator CheckScenesLoad()
    {
        while(true)
        {

        }
    }
}
