using _WeAreAthomic.SCRIPTS.Genericos_Scripts;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

public class GVideos : MonoBehaviour
{
    [SerializeField]SceneLoaderStart sceneLoaderStart;

    [SerializeField] private GLoadScene sceneLoader;


    [SerializeField] private VideoPlayer videoPlayer;

    [SerializeField] private List<VideoClip> videos;

    [SerializeField] private bool loadSceneOnFinish;

    [SerializeField] private Scenes sceneToLoad;

    [SerializeField] UnityEvent OnFinish;

    private int _currentVideo;

    private void Start()
    {
        sceneLoaderStart.StartLoadingScenes();
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

            OnFinish.Invoke();
        }
    }

    void EndReached(VideoPlayer vp)
    {
        NextVideo();
    }
}
