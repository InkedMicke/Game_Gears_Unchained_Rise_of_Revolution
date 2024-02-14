using System.Collections.Generic;
using UnityEngine;
using Hedenrag.ExVar;
using UnityEngine.EventSystems;
using NaughtyAttributes;
using Unity.VisualScripting;

public enum TypeOfInput
{
    gamepad,
    pc
}

public enum Language
{
    es,
    en
}

public enum CurrentAbility
{
    None,
    RapidFire,
    ThreeSixtyAttack,
    Protection,
    Heal
}

public enum DifficultyLevel
{
    historia,
    desafio,
    maestro
}

[System.Serializable]
public class AbilitiesStruct
{
    public int value;
    public int priceToBuy;
    public CurrentAbility currentAbility;
    public Sprite sprite;
    public bool IsUnlocked;
}
public enum TypeOfEnemy
{
    GreenSoldier,
    OrangeSoldier,
    RedSoldier,
    BlueSoldier
}

public class GameManagerSingleton : SingletonScriptableObject<GameManagerSingleton>, ICallOnAll
{


    public enum Settings
    {
        SensivityX,
        SensivityY
    }

 

    public Settings settings;
    public DifficultyLevel _currentDifficulty = DifficultyLevel.historia;
    public TypeOfInput typeOfInput = TypeOfInput.pc;
    public CurrentAbility currentAbility = CurrentAbility.None;

    public List<GameObject> gameObjectsList;
    public List<GameObject> closestGameObjectsList;
    public List<AbilitiesStruct> abiltiesList = new();
    public GameObject gamepadController;
    [SerializeField] private GameObject greenSoldierPrefab;
    [SerializeField] private GameObject orangeSoldierPrefab;
    [SerializeField] private GameObject redSoldierPrefab;
    [SerializeField] private GameObject blueSoldierPrefab;


    public Vector3 currentCheckpoint;

    public bool IsGamePaused;
    public bool thereIsCanvasBelow;
    public bool HasUnlockedBastetAttack;
    public bool IsUnlimitedEnergy;
    public bool IsGodModeEnabled;
    public bool IsStopMenuEnabled;
    public bool IsSettingsMenuEnabled;
    public bool IsGameOverEnabled;
    public bool IsAbilityMenuEnabled;
    public bool SkippedTutorial;
    public bool IsFullscreen;
    public bool _toggledTotally;
    public bool IsLoadingStartVideos;

    public int maxSensivity;
    public int sensivityX;
    public int sensivityY;
    public int gearsItem;
    public int generatorsDestroyed;

    public float currentHealth = 100f;
    public float bastetEnergy = 100f;
    public float brightness;


    private void Awake()
    {

        gameObjectsList = new List<GameObject>();

        gameObjectsList.Clear();
        closestGameObjectsList.Clear();

        IsGamePaused = false;

    }
    public void GeneratorsDestroyed()
    {
        generatorsDestroyed += 1; 
    }


    public void AddGameObject(GameObject gameObject)
    {
        gameObjectsList.Add(gameObject);
    }

    public void AddClosestGameObject(GameObject gameObject)
    {
        closestGameObjectsList.Add(gameObject);
    }

    public void RemoveGameObject(GameObject gameObject)
    {
        gameObjectsList.Remove(gameObject);
    }

    public void RemoveClosestsGameObject(GameObject gameObject)
    {
        closestGameObjectsList.Remove(gameObject);
    }

    public void ClearClosestsGameObject()
    {
        closestGameObjectsList.Clear();
    }

    public void ClearGameObject()
    {
        gameObjectsList.Clear();
    }

    public List<GameObject> GetGameObjectsList()
    {
        return gameObjectsList;
    }

    public GameObject GetClosestGameObjectsList()
    {
        return closestGameObjectsList[0];
    }

    /// <summary>
    /// If true game is paused, if not is "unpaused"
    /// </summary>
    /// <param name="condition"></param>
    public void PauseGame(bool condition)
    {
        IsGamePaused = condition;
    }

    public void SetThereIsCanvasBelow(bool condition)
    {
        thereIsCanvasBelow = condition;
    }

    public void SetHasUnlockedBastetAttack(bool condition)
    {
        HasUnlockedBastetAttack = condition;
    }

    public void TakeEnergy(float value)
    {
        bastetEnergy -= value;
    }

    public void SetIsUnlimitedEnergy(bool condition)
    {
        IsUnlimitedEnergy = condition;
    }

    public void SetGodModeBool(bool condition)
    {
        IsGodModeEnabled = condition;
    }

    public void SetIsStopMenuEnabled(bool condition)
    {
        IsStopMenuEnabled = condition;
    }

    public void SetIsSettingsMenuEnabled(bool condition)
    {
        IsSettingsMenuEnabled = condition;
    }    
    
    public void SetIsGameOverMenuEnabled(bool condition)
    {
        IsGameOverEnabled = condition;
    }

    public void FreezeTime(bool condition)
    {
        Time.timeScale = condition ? 0 : 1;
    }

    public void GameState(bool condition)
    {
        IsGamePaused = condition;
    }

    public void SetSkippedTutorial(bool condition)
    {
        SkippedTutorial = condition;
    }    
    public void SetIsAbilityMenuTutorial(bool condition)
    {
        IsAbilityMenuEnabled = condition;
    }
    
    public void SetIsLoadingStartVideos(bool isLoadingVideos)
    {
        IsLoadingStartVideos = isLoadingVideos;
    }

    public void AddGearItem(int value)
    {
        gearsItem += value;
    }

    public void SetEventSystemSelectedObj(GameObject obj)
    {
        EventSystem.current.SetSelectedGameObject(obj);
    }

    public void ToggleIsUnlimitedEnergy()
    {
        IsUnlimitedEnergy = !IsUnlimitedEnergy;

        if (IsUnlimitedEnergy)
        {
            bastetEnergy = 100f;
        }
    }


    /// <summary>
    /// If True Cursor Free, If False Cursor Locked
    /// </summary>
    /// <param name="condition"></param>
    public void ShowCursor(bool condition)
    {
        if (condition)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

    }

    public void CloseTotallyWindow()
    {
        GameState(false);
        ShowCursor(false);
        FreezeTime(false);
    }

    public void OpenTotallyWindow()
    {
        GameState(true);
        ShowCursor(true);
        FreezeTime(true);
    }
    [Button]
    public void ToggleTotallyWindow()
    {
        _toggledTotally = !_toggledTotally;
        if (_toggledTotally)
        {
            GameState(!true);
            ShowCursor(true);
            FreezeTime(true);
        }
        else
        {
            GameState(false);
            ShowCursor(false);
            FreezeTime(false);
        }
    }

    public void ToggleGodModeBool()
    {
        IsGodModeEnabled = !IsGodModeEnabled;
    }

    public float GetPlayerDamage(PlayerDamageData damageData, GameObject other)
    {
        switch (_currentDifficulty)
        {
            case DifficultyLevel.historia:
                if (other.name == "DummieHurtBox")
                {
                    return damageData.easyDifficult.dummieDamage;
                }
                if (other.name == "GreenHurtBox")
                {
                    return damageData.easyDifficult.greenDamage;
                }

                if (other.name == "RedHurtBox")
                {
                    return damageData.easyDifficult.redDamage;
                }

                if (other.name == "OrangeHurtBox")
                {
                    return damageData.easyDifficult.orangeDamage;
                }

                break;

            case DifficultyLevel.desafio:
                if (other.name == "DummieHurtBox")
                {
                    return damageData.normalDifficult.dummieDamage;
                }

                if (other.name == "GreenHurtBox")
                {
                    return damageData.normalDifficult.greenDamage;
                }                
                
                if (other.name == "RedHurtBox")
                {
                    return damageData.normalDifficult.redDamage;
                }               
                
                if (other.name == "OrangeHurtBox")
                {
                    return damageData.normalDifficult.orangeDamage;
                }
                break;

            case DifficultyLevel.maestro:
                if (other.name == "DummieHurtBox")
                {
                    return damageData.hardDifficult.dummieDamage;
                }
                if (other.name == "GreenHurtBox")
                {
                    return damageData.hardDifficult.greenDamage;
                }

                if (other.name == "RedHurtBox")
                {
                    return damageData.hardDifficult.redDamage;
                }

                if (other.name == "OrangeHurtBox")
                {
                    return damageData.hardDifficult.orangeDamage;
                }
                break;
        }

        return 0;
    }

    public float GetEnemyDamage(EnemyDamageData enemyData)
    {
        switch (GameManagerSingleton.Instance._currentDifficulty)
        {
            case DifficultyLevel.historia:
                return enemyData.easyDifficult.playerDamage;
            case DifficultyLevel.desafio:
                return enemyData.normalDifficult.playerDamage;
            case DifficultyLevel.maestro:
                return enemyData.hardDifficult.playerDamage;
        }

        return 0;
    }

    public void EnumToValue(Settings currentSetting, int value)
    {
        if (currentSetting == Settings.SensivityX)
        {
            sensivityX = value;
        }
        else if (currentSetting == Settings.SensivityY)
        {
            sensivityY = value;
        }
    }

    public GameObject TypeOfEnemyToPrefab(TypeOfEnemy enemy)
    { 

    switch(enemy)
        { 
        case TypeOfEnemy.GreenSoldier: return greenSoldierPrefab;
        case TypeOfEnemy.OrangeSoldier: return orangeSoldierPrefab;
        case TypeOfEnemy.RedSoldier: return redSoldierPrefab;
        case TypeOfEnemy.BlueSoldier: return blueSoldierPrefab;
        }
        return new GameObject();
    }

}
