using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class MainCRailTwo : MonoBehaviour
{
    private Spline spline;

    [SerializeField] private LayerMask railLayer;

    [SerializeField] private Transform railCheck;


    [System.NonSerialized] public bool IsSliding;

    public float speed = 5f;
    public float distanceThreshold = 0.1f;
    private float currentDistance = 0f;
    private float m_slidingCooldown = 1f;
    private float m_totalSlidingCooldown;

    private int currentSplineIndex = 0;
    private int currentPointIndex;

    private void Update()
    {
        if (IsOnRail() && !IsSliding && Time.time > m_totalSlidingCooldown)
        {

            StartSlide();
        }

        if(IsSliding)
        {
            MoveAlongSpline();
        }
    }

    void MoveAlongSpline()
    {
        if (spline == null || spline.Count == 0)
        {
            Debug.LogWarning("No se ha asignado una spline válida en el SplineContainer.");
            return;
        }

        float step = speed * Time.deltaTime;
        Vector3 targetPosition = spline.EvaluatePosition(currentPointIndex);

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

        if (Vector3.Distance(transform.position, targetPosition) < distanceThreshold)
        {
            currentPointIndex++;

            if (currentPointIndex >= spline.Count)
            {
                currentPointIndex = 0;
            }
        }
    }

    private void StartSlide()
    {
        var col = Physics.OverlapSphere(railCheck.position, 0.2f);
        spline = col[0].transform.GetChild(0).GetComponent<Spline>();
        IsSliding = true;
        //_mainCSounds.PlayRailSound();
    }

    public bool IsOnRail()
    {
        return Physics.CheckSphere(railCheck.position, .1f, railLayer);
    }
}
