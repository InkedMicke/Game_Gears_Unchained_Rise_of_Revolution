using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Video;
using Generics.Scene;

public class VideoManagerStart : MonoBehaviour
{
    private PlayerInputActions _inputActions;

    [SerializeField] SceneLoaderStart sceneLoaderStart;

    [SerializeField] private GLoadScene sceneLoader;

    [SerializeField] private VideoPlayer videoPlayer;

    [SerializeField] private List<VideoClip> videos;

    [SerializeField] private GameObject loadingScreen;

    [SerializeField] private bool loadSceneOnFinish;

    [SerializeField] private Scenes sceneToLoad;

    [SerializeField] UnityEvent OnFinish;

    private int _currentVideo;

    private void Awake()
    {
        _inputActions = new PlayerInputActions();
        _inputActions.Enable();
        _inputActions.PlayerPC.SkipVideoStart.performed += InputPC;
        _inputActions.PlayerGamepad.SkipVideoStart.performed += InputGamepad;
    }

    private void Start()
    {
        sceneLoaderStart.StartLoadingScenes();
        GameManagerSingleton.Instance.SetIsLoadingStartVideos(true);
        videoPlayer.clip = videos[_currentVideo];
        videoPlayer.Play();
        _currentVideo++;
        videoPlayer.loopPointReached += EndReached;
    }

    private void InputPC(InputAction.CallbackContext x)
    {
        if(GameManagerSingleton.Instance.typeOfInput == TypeOfInput.pc)
        {
            NextVideo();
        }
    }    
    
    private void InputGamepad(InputAction.CallbackContext x)
    {
        if(GameManagerSingleton.Instance.typeOfInput == TypeOfInput.gamepad)
        {
            NextVideo();
        }
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
