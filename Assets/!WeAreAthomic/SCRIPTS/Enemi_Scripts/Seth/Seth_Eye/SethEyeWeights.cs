using Seth;
using System.Collections;
using UnityEngine;

public class SethEyeWeights : MonoBehaviour
{
    [SerializeField] SethEye sethEye;

    [SerializeField] float rotationSpeed;
    [SerializeField] float rotSpeed = .1f;
    float m_desiredRot;

    bool m_isReturningToStart;

    [SerializeField] float velocity;

    public Vector3 targetDirection = Vector3.forward;

    Vector3 currentDir = Vector3.forward;

    public void starteye()
    {
        m_isReturningToStart = false;
        //StartCoroutine(UpdateDir());
        sethEye.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        StartCoroutine(RotateEye());
        StartCoroutine(MoveSethEye());
    }

    IEnumerator RotateEye()
    {
        while(true)
        {
            yield return null;
            Debug.Log(Vector3.Distance(sethEye.transform.position, transform.position));
            transform.Rotate(0f, rotSpeed, 0f);
        }
    }

    IEnumerator MoveSethEye()
    {
        sethEye.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        while (!m_isReturningToStart)
        {
            var random1 = Random.Range(200.0f, 300.0f);
            while (!m_isReturningToStart)
            {
                yield return null;

                if(Vector3.SqrMagnitude(sethEye.transform.position - transform.position) > 300f)
                {
                    break;
                }

                sethEye.transform.position += Time.deltaTime * velocity * transform.forward;

            }

            var random2 = Random.Range(30.0f, -300.0f);
            while (!m_isReturningToStart)
            {
                yield return null;

                if(Vector3.SqrMagnitude(sethEye.transform.position - transform.position) < -300f)
                {
                    break;
                }

                sethEye.transform.position -= Time.deltaTime * velocity * transform.forward;

            }

            yield return null;
        }
    }

    /*            yield return null;

            Vector2 size = new (transform.localScale.x, transform.localScale.z);
            float currentWheight = Mathf.Max((sethEye.transform.position.x - transform.position.x) / size.x, (sethEye.transform.position.z - transform.position.z) / size.y);

            Vector3 returnedDir = Vector3.Lerp(targetDirection, (sethEye.transform.position - transform.position).normalized, currentWheight);

            currentDir = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(returnedDir), rotationSpeed * Time.deltaTime) * currentDir;


            sethEye.transform.position += Time.deltaTime * velocity * currentDir;

            Debug.Log(currentWheight);*/



    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = new Color32(255, 255, 255, 120);
        Gizmos.DrawCube(Vector3.zero, Vector3.one);
        Vector2 size = new(transform.localScale.x, transform.localScale.z);
    }
}
