using UnityEngine;



[CreateAssetMenu(fileName = "EnemyDamageData", menuName = "Enemy/EnemyDamageData")]
public class EnemyDamageData : ScriptableObject
{
    [System.Serializable]
    public struct EasyDifficult
    {
        public float playerDamage;
    }
    [System.Serializable]
    public struct NormalDifficult
    {
        public float playerDamage;
    }
    [System.Serializable]
    public struct HardDifficult
    {
        public float playerDamage;
    }

    public EasyDifficult easyDifficult;
    public NormalDifficult normalDifficult;
    public HardDifficult hardDifficult;
}

