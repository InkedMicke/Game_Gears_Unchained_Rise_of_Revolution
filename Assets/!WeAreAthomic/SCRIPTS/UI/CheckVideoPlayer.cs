using _WeAreAthomic.SCRIPTS.Genericos;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;

public class CheckVideoPlayer : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;

    private void Start()
    {

        var videoPlayer = GetComponent<VideoPlayer>();

        videoPlayer.loopPointReached += EndReached;
    }

    void EndReached(VideoPlayer vp)
    {
        var loadScene = FindObjectOfType<GLoadScene>();
        loadScene.LoadScene(sceneToLoad);
    }
}
