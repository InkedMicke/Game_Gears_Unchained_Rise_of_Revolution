using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Hedenrag
{
    namespace SceneLoader
    {
        // Create asset menu is diseabled since only one asset of this type should exist at a time
        //[CreateAssetMenu(menuName = "Scriptable Objects/Scene Loader/Static Loader")]
        public class SceneLoaderManager : ScriptableObject
        {
#if UNITY_EDITOR
            [SerializeField] SceneAsset[] sceneAssets;

            public void ReLink()
            {
                OnValidate();
            }

            private void OnValidate()
            {
                string scenes = "";

                foreach (SceneAsset sceneAsset in sceneAssets)
                {
                    if (sceneAsset == null) return;
                    scenes += sceneAsset.name + "\n";
                }

                using (FileStream fs = new FileStream("Assets/Resources/"+ savePath +".txt", FileMode.Create))
                {
                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        writer.Write(scenes);
                    }
                }
            }
#endif
            const string savePath = "SceneLoader/NeverUnloadingScenes";
            [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
            static void Init()
            {
                SceneManager.LoadSceneAsync(emptySceneName, LoadSceneMode.Additive);
                foreach(string scene in GetNeverUnloadingScenes())
                {
                    SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
                }
            }
            public static readonly string emptySceneName = "SceneLoaderEmptyScene";

            public static string[] GetNeverUnloadingScenes()
            {
                var v = Resources.Load<TextAsset>(savePath);
                return v.text.Split('\n');
            }
        }
    }
}