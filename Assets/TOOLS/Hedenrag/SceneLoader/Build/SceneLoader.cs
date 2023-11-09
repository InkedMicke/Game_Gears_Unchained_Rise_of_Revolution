using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Hedenrag
{
    namespace SceneLoader
    {

        [Obsolete]
        public class SceneLoader : MonoBehaviour
        {
#if UNITY_EDITOR
            public void ReLink()
            {
                OnValidate();
            }
#endif
            [SerializeField, HideInInspector] int sceneToLoad;
#if UNITY_EDITOR
            [SerializeField] SceneAsset asset;

            [SerializeField] bool working;
            private void OnValidate()
            {
                sceneToLoad = SceneUtility.GetBuildIndexByScenePath(AssetDatabase.GetAssetPath(asset));
                working = (sceneToLoad != -1);
            }
#endif
            public void LoadScene()
            {
                if (sceneToLoad == -1) { Debug.LogError("sceneNotInBuildSettings"); return; }
                SceneManager.LoadScene(sceneToLoad);
            }

        }
    }
}
