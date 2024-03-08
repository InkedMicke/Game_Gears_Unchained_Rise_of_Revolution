using Seth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SethEyeWeights : MonoBehaviour
{
    [SerializeField] SethEye sethEye;

    [SerializeField] float rotationSpeed;

    [SerializeField] float velocity;

    public Vector3 targetDirection = Vector3.forward;

    Vector3 currentDir = Vector3.forward;

    public void starteye()
    {
        StartCoroutine(MoveSethEye());
        StartCoroutine(UpdateDir());
    }

    IEnumerator MoveSethEye()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        while (true)
        {
            yield return new WaitForEndOfFrame();

            Vector2 size = new Vector2(transform.localScale.x, transform.localScale.z);
            float currentWheight = Mathf.Max((sethEye.transform.position.x - transform.position.x) / size.x, (sethEye.transform.position.z - transform.position.z) / size.y);

            Vector3 returnedDir = Vector3.Lerp(targetDirection, (sethEye.transform.position - transform.position).normalized, currentWheight);

            currentDir = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(returnedDir), rotationSpeed * Time.deltaTime) * currentDir;


            sethEye.transform.position += Time.deltaTime * velocity * currentDir;

            Debug.Log(currentWheight);
            //Vector3.Lerp();

        }
    }

    IEnumerator UpdateDir()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);
            //currentDir = Random.Range(0f,360f)

        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = new Color32(255, 255, 255, 120);
        Gizmos.DrawCube(Vector3.zero, Vector3.one);
    }
}
