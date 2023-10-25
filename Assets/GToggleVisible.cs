using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GToggleVisible : MonoBehaviour
{
    public void ToggleVisible()
    {
        this.gameObject.SetActive(!this.gameObject);
    }
}
