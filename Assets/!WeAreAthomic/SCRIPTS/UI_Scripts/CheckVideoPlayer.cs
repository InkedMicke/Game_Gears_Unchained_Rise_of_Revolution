using Generics.Scene;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

namespace _WeAreAthomic.SCRIPTS.UI
{
    public class CheckVideoPlayer : MonoBehaviour
    {
        [SerializeField] private Scenes sceneToLoad;

        private void Start()
        {
            var videoPlayer = GetComponent<VideoPlayer>();
            videoPlayer.loopPointReached += EndReached;
        }

        private void EndReached(VideoPlayer vp)
        {
            var loadScene = GetComponent<GLoadScene>();
            loadScene.LoadSceneMode(sceneToLoad, LoadSceneMode.Single);
        }
    }
}
