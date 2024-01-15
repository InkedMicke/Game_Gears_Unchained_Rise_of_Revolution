using System.Collections.Generic;
using UnityEngine;
using Hedenrag.ExVar;
using UnityEngine.EventSystems;

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

public class GameManagerSingleton : SingletonScriptableObject<GameManagerSingleton>, ICallOnAll
{
    public enum DifficultyLevel
    {
        historia,
        desafio,
        maestro
    }

    public enum Settings
    {
        SensivityX,
        SensivityY
    }

    public enum TypeOfEnemy
    {
        GreenSoldier,
        OrangeSoldier,
        RedSoldier,
        BlueSoldier
    }

    public Settings settings;
    public DifficultyLevel _currentDifficulty = DifficultyLevel.historia;
    public TypeOfInput typeOfInput = TypeOfInput.pc;
    public CurrentAbility currentAbility = CurrentAbility.None;

    public List<GameObject> gameObjectsList;
    public List<GameObject> closestGameObjectsList;
    public GameObject gamepadController;
    
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
    public bool IsProtectionAnilityUnlocked;
    public bool IsRapidFireUnlocked;
    public bool Is360AttackUnlocked;
    public bool IsNanoHealthUnlocked;

    public int maxSensivity;
    public int sensivityX;
    public int sensivityY;
    public int gearsItem;

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
    public void CursorMode(bool condition)
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
        CursorMode(false);
        FreezeTime(false);
    }

    public void OpenTotallyWindow()
    {
        GameState(true);
        CursorMode(true);
        FreezeTime(true);
    }

    public void ToggleTotallyWindow()
    {
        _toggledTotally = !_toggledTotally;
        if (_toggledTotally)
        {
            GameState(!true);
            CursorMode(true);
            FreezeTime(true);
        }
        else
        {
            GameState(false);
            CursorMode(false);
            FreezeTime(false);
        }
    }

    public void ToggleGodModeBool()
    {
        IsGodModeEnabled = !IsGodModeEnabled;
    }

    public float GetDamage(PlayerDamageData damageData, GameObject other)
    {
        switch (_currentDifficulty)
        {
            case DifficultyLevel.historia:
                if (other.gameObject.name == "DummieHurtBox")
                {
                    return damageData.easyDifficult.dummieDamage;
                }
                if (other.gameObject.name == "GreenHurtBox")
                {
                    return damageData.easyDifficult.greenDamage;
                }

                if (other.gameObject.name == "RedHurtBox")
                {
                    return damageData.easyDifficult.greenDamage;
                }

                if (other.gameObject.name == "OrangeHurtBox")
                {
                    return damageData.easyDifficult.greenDamage;
                }

                break;

            case DifficultyLevel.desafio:
                if (other.gameObject.name == "DummieHurtBox")
                {
                    return damageData.normalDifficult.dummieDamage;
                }

                if (other.gameObject.name == "GreenHurtBox")
                {
                    return damageData.normalDifficult.greenDamage;
                }                
                
                if (other.gameObject.name == "RedHurtBox")
                {
                    return damageData.normalDifficult.greenDamage;
                }               
                
                if (other.gameObject.name == "OrangeHurtBox")
                {
                    return damageData.normalDifficult.greenDamage;
                }
                break;

            case DifficultyLevel.maestro:
                if (other.gameObject.name == "DummieHurtBox")
                {
                    return damageData.hardDifficult.dummieDamage;
                }
                if (other.gameObject.name == "GreenHurtBox")
                {
                    return damageData.hardDifficult.greenDamage;
                }

                if (other.gameObject.name == "RedHurtBox")
                {
                    return damageData.hardDifficult.greenDamage;
                }

                if (other.gameObject.name == "OrangeHurtBox")
                {
                    return damageData.hardDifficult.greenDamage;
                }
                break;
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


}
