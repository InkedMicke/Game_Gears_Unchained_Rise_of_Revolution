using UnityEngine;

namespace Enemy
{
    public class SoldierWarnings : MonoBehaviour
    {
        EnemyP enemy;

        Animator m_animExclamation;
        Animator m_animQuestion;

        [SerializeField] GameObject exclamationObj;
        [SerializeField] GameObject interrogacionObj;

        bool m_firstTime = true;

        private void Awake()
        {
            enemy = GetComponent<EnemyP>();
            m_animQuestion = interrogacionObj.GetComponent<Animator>();
            m_animExclamation = exclamationObj.GetComponent<Animator>();
        }

        private void OnEnable()
        {
            enemy.OnCached += StartExclamationMark;
            enemy.OnWarning += StartQuestionMark;
        }

        private void OnDisable()
        {
            enemy.OnCached -= StartExclamationMark;
            enemy.OnWarning -= StartQuestionMark;
        }

        public void StartQuestionMark()
        {
            interrogacionObj.SetActive(true);
            m_animQuestion.SetTrigger("start");
            enemy.OnWarning -= StartQuestionMark;
        }

        public void StartExclamationMark()
        {
            exclamationObj.SetActive(true);
            m_animExclamation.SetTrigger("start");
            enemy.OnCached -= StartExclamationMark;
        }

        public void EndAnim()
        {
            exclamationObj.SetActive(false);
            interrogacionObj.SetActive(false);
        }
    }
}