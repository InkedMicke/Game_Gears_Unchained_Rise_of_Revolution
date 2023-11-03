using System;
using _WeAreAthomic.SCRIPTS.Genericos;
using UnityEngine;
using UnityEngine.Video;
using _WeAreAthomic.SCRIPTS.Genericos;
using UnityEngine.SceneManagement;

namespace _WeAreAthomic.SCRIPTS.UI
{
    public class CheckVideoPlayer : MonoBehaviour
    {
        [SerializeField] private string sceneToLoad;

        private void Start()
        {
            var videoPlayer = GetComponent<VideoPlayer>();
            videoPlayer.loopPointReached += EndReached;
        }

        private void EndReached(VideoPlayer vp)
        {
            var loadScene = GetComponent<GLoadScene>();
            loadScene.LoadScene(sceneToLoad, LoadSceneMode.Single);
        }
    }
}
