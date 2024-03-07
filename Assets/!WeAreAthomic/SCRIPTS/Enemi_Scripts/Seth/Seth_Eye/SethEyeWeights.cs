using Seth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SethEyeWeights : MonoBehaviour
{
    [SerializeField] SethEye sethEye;

    [SerializeField] float rotationSpeed;

    [SerializeField] float velocity;

    Vector3 targetDirection = Vector3.forward;

    

    IEnumerator MoveSethEye()
    {
        while (true)
        {
            yield return null;

            Vector2 size = new Vector2(transform.localScale.x, transform.localScale.z);
            float currentWheight = Mathf.Lerp((sethEye.transform.position.x - transform.position.x) / size.x, (sethEye.transform.position.z - transform.position.z) / size.y, 0.5f);

            //sethEye.transform.position =+ Time.deltaTime * velocity * sethEye.transform.forward;
            Debug.Log(currentWheight);
            //Vector3.Lerp();

        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = new Color32(255, 255, 255, 120);
        Gizmos.DrawCube(Vector3.zero, Vector3.one);
    }
}
