using _WeAreAthomic.SCRIPTS.Genericos_Scripts;
using Hedenrag.SceneLoader;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

public class VideoManagerStart : MonoBehaviour
{
    [SerializeField] SceneLoaderStart sceneLoaderStart;

    [SerializeField] private GLoadScene sceneLoader;


    [SerializeField] private VideoPlayer videoPlayer;

    [SerializeField] private List<VideoClip> videos;

    [SerializeField] private GameObject loadingScreen;

    [SerializeField] private bool loadSceneOnFinish;

    [SerializeField] private Scenes sceneToLoad;

    [SerializeField] UnityEvent OnFinish;

    private int _currentVideo;

    private void Start()
    {
        sceneLoaderStart.StartLoadingScenes();
        GameManagerSingleton.Instance.SetIsLoadingStartVideos(true);
        videoPlayer.clip = videos[_currentVideo];
        videoPlayer.Play();
        _currentVideo++;
        videoPlayer.loopPointReached += EndReached;
    }

    public void NextVideo()
    {
        if (_currentVideo < videos.Count)
        {
            videoPlayer.clip = videos[_currentVideo];
            videoPlayer.Play();
            _currentVideo++;
        }
        else
        {
            if (sceneLoaderStart.currentSceneIndex == sceneLoaderStart.loadSceneAsyncAssets.Count)
            {
                OnFinish.Invoke();
            }
            else
            {
                //Cargar pantalla de Carga
                sceneLoaderStart.timeToLoad = 1.5f;
                loadingScreen.SetActive(true);
                sceneLoaderStart.StartCheckingLoadedScenes(() =>
                {
                    loadingScreen.SetActive(false);
                    OnFinish.Invoke();
                });
            }
        }
    }

    void EndReached(VideoPlayer vp)
    {
        NextVideo();
    }
}
