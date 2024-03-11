using Seth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SethEyeWeights : MonoBehaviour
{
    [SerializeField] SethEye sethEye;

    [SerializeField] bool seeSquare = true;

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
            Vector3 sethOffset = sethEye.transform.position - transform.position;
            float currentWheight = Mathf.Max(Mathf.Abs((sethOffset.x * 4f) / size.x), Mathf.Abs((sethOffset.y * 4f) / size.y));

            Vector3 returnedDir = Vector3.Lerp(targetDirection, -sethOffset.normalized, currentWheight);

            currentDir = Vector3.RotateTowards(currentDir, returnedDir, rotationSpeed * Time.deltaTime, 0f);

            sethEye.transform.position += Time.deltaTime * velocity * currentDir;

            //Vector3.Lerp();
            sethEye.transform.position = new Vector3(sethEye.transform.position.x, transform.position.y, sethEye.transform.position.z);
        }
    }

    IEnumerator UpdateDir()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            targetDirection = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f) * Vector3.forward;
        }
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (seeSquare)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = new Color32(255, 255, 255, 120);
            Gizmos.DrawCube(Vector3.zero, Vector3.one);
        }
    }
#endif
}
