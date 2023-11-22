using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "EasyEnemyDiff", menuName = "Enemy/EasyDifficulty")]
public class EasyDifficultySettings : ScriptableObject
{
    [Header("GreenSoldier")]
    [Header("MovementVariables")]
    public float walkSpeed; 

    [Header("AttackVariables")]
    public float distanceToDetectPlayer;
    public float zSizeOfDecal;



}

[CreateAssetMenu(fileName = "NormalEnemyDiff", menuName = "Enemy/NormalDifficulty")]
public class NormalEnemyDifficultySettings : ScriptableObject
{
    [Header("GreenSoldier")]
    public float attack;



}

[CreateAssetMenu(fileName = "HardEnemyDiff", menuName = "Enemy/HardDifficulty")]
public class HardEnemyDifficultySettings : ScriptableObject
{
    [Header("GreenSoldier")]
    public float attack;



}

