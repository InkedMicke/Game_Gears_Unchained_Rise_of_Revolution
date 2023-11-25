using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct EasyDifficult
{
    [Header("DummieSoldier")]
    public float dummieDamage;

    [Header("GreenSoldier")]
    public float greenDamage; 
    
    [Header("OrangeSoldier")]
    public float orangeDamage;
    
    [Header("RedSoldier")]
    public float redDamage;
    
    [Header("BlueSoldier")]
    public float blueDamage;
}

[System.Serializable]
public struct NormalDifficult
{
    [Header("DummieSoldier")]
    public float dummieDamage;

    [Header("GreenSoldier")]
    public float greenDamage; 
    
    [Header("OrangeSoldier")]
    public float orangeDamage;
    
    [Header("RedSoldier")]
    public float redDamage;
    
    [Header("BlueSoldier")]
    public float blueDamage;
}
[System.Serializable]

public struct HardDifficult
{
    [Header("DummieSoldier")]
    public float dummieDamage;

    [Header("GreenSoldier")]
    public float greenDamage; 
    
    [Header("OrangeSoldier")]
    public float orangeDamage;
    
    [Header("RedSoldier")]
    public float redDamage;
    
    [Header("BlueSoldier")]
    public float blueDamage;
}

[CreateAssetMenu(fileName = "PlayerDamageData", menuName = "Player/PlayerDamageData")]
public class PlayerDamageData : ScriptableObject
{
    public EasyDifficult easyDifficult;
    public NormalDifficult normalDifficult;
    public HardDifficult hardDifficult;
}
