using UnityEngine;

public class SethBustHurtBox : MonoBehaviour
{
    G_DitheringDisolve m_disolve;

    [SerializeField] GameObject brokenBust;

    private void Awake()
    {
        m_disolve = brokenBust.GetComponent<G_DitheringDisolve>();
    }

    public void StartBustDestroyEffect()
    {
        brokenBust.SetActive(true);
        brokenBust.transform.SetParent(null);
        m_disolve.StartDisolveDithering();
        Destroy(gameObject);
    }
}
