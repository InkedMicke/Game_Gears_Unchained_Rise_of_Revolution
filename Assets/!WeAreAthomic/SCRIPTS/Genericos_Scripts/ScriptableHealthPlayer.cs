using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="PlayerHealthData",menuName = "Player/PlayerHealthData")]

public class ScriptableHealthPlayer : ScriptableObject
{
    [System.Serializable]
    public struct EasyDifficult
    {
        public float health;
    }
    [System.Serializable]
    public struct NormalDifficult
    {
        public float health;
    }
    [System.Serializable]
    public struct HardDifficult
    {
        public float health;
    }

    public EasyDifficult easyDifficult;
    public NormalDifficult normalDifficult;   
    public HardDifficult hardDifficult; 

}
