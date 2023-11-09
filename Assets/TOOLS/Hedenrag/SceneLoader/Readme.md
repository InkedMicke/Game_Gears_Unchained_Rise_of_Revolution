# Scene loader

Scene loader is a set of tools that facilitate the loading of scenes.

## How to use

Scene Loader has four objects to load scenes:
- LoadAlwaysActiveScenes
- SceneLoaderAsync
- SceneLoaderMultipleAsync
- SceneLoader (Tagged as obsolete)


### Scene Loader
Scene loder is the easyest to use but is a bit limited, it only allows one scene at a time and is not compatible with the rest of the SceneLoaders. Recomended to begninners but not recomended for big projects.
It works by unloading all scenes and loading the desired scene.

### LoadAlwaysActiveScenes
In the same folder of this readme there is an asset named "AlwaysLoadedScenesAsset" you can add there all the scenes you want to manually load and unload (they will still be loaded if you add them to any scene loader but they will never be unloaded). You can manually unload a scene by using "SceneManager.UnloadSceneAsync()". "LoadSceneAsyinc()" without additive tag, "LoadScene()" and "SceneLoader.LoadScene()" will unload all scenes regardless of any setting of this tool so try to not use it. If you use it everyting should still work but only the loaded scene will be active.

### SceneLoaderAsync
Loads only the given scene and unloads the rest only keeping the always active scenes enabled. The loaded scene will be the active scene once all scenes are completed loading.

### SceneLoaderMultipleAsync
SceneLoaderMultipleAsync is the best way to load scenes, even if you only want to load a single scene thanks to the "LoadSceneAsset".
It Will load all the scenes stated in the provided asset. The overlapping scenes from already loaded scenes will not be reloaded but kept as they where, both to save resources and keep continuity. if you wish to reload some scene you have a couple options:
- if you only want to reload a singular scene unload and load that singular scene with "SceneManager.UnloadSceneAsyinc()" and reload with "SceneManager.LoadSceneAsync("sceneName", LoadSceneMode.Additive)" or by recalling the SceneLoaderMultipleAsync.
- if you want to reload all the scenes the best way i to call an intermediate scene with "SceneLoaderAsync" and then calling the desired SceneLoaderMultipleAsync. 
The active scene on the completion of the loader will be the first scene of the "LoadSceneAsset".

### LoadSceneAsset
The LoadSceneAsset is the place where you store all the scenes to load with SceneLoaderMultipleAsync.
The scenes in LoadSceneAsset can be validated in the menubar in "Hedenrag/SceneManager/Link Scenes" this will make all scenes work again in cas you changed the name or build index.
 
## Things to have in mind
While LoadSceneAsst has a hady button for repairing itself the other methods dont so you will have to go SceneLoader by SceneLoader clicking the button (If you want for me to add a button make a complelling argument and if enough people tell me that ill probably will add one).
There is a scene in the tool called emptyScene do not delete it nor remove it from build settings, its very necessary for all to work correctly.
The tool "FindObjectsOfType is inclueded in this package since its needed for the link scenes button"

