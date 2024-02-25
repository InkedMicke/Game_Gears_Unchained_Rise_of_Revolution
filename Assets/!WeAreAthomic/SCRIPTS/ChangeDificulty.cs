using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeDificulty : MonoBehaviour
{
    [SerializeField] DifficultyLevel difficultylevel;

    // Start is called before the first frame update
    public void SetDificulty()
    {
        GameManagerSingleton.Instance._currentDifficulty = difficultylevel;
    }
}
