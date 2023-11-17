using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenSoliderHurtBox : MonoBehaviour
{
    public bool CheckIfPlayerIsNear(float radius)
    {
        return Physics.CheckSphere(transform.position, radius);
    }
}
