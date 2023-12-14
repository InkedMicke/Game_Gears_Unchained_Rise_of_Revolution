using _WeAreAthomic.SCRIPTS.Genericos_Scripts;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class GVideos : MonoBehaviour
{
    [SerializeField] private GLoadScene sceneLoader;

    [SerializeField] private VideoPlayer videoPlayer;

    [SerializeField] private List<VideoClip> videos;

    [SerializeField] private bool loadSceneOnFinish;

    [SerializeField] private string sceneString = "sample";

    private int _currentVideo;

    private void Start()
    {
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
            if(loadSceneOnFinish)
            {
                sceneLoader.LoadScene(sceneString);
            }
        }
    }

    void EndReached(VideoPlayer vp)
    {
        NextVideo();
    }
}
