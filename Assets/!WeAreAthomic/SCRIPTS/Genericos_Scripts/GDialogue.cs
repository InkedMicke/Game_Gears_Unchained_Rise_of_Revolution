using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Generics
{
    [Serializable]
    public class DialogueClass
    {
        [SerializeField] private string nameOfElement;
        public string title;
        public Sprite sprite;
        [TextArea] public string textAreaES;
        [TextArea] public string textAreaEN;
        public UnityEvent onStart;
        public UnityEvent onFinish;
    }

    public class GDialogue : MonoBehaviour
    {
        private PlayerInputActions _inputActions;

        [SerializeField] private TextMeshProUGUI textComponent;
        [SerializeField] private TextMeshProUGUI titleComponent;

        [SerializeField] private Image imageToChange;

        [SerializeField] private float textSpeed = 0.3f;

        [SerializeField] private List<DialogueClass> dialogues;

        [SerializeField] private UnityEvent onFinishAllDialogues;

        private readonly List<DialogueClass> m_genereicDialogue = new();

        private bool _isInDialogue;

        private int index;

        private void Awake()
        {
            _inputActions = new PlayerInputActions();
            _inputActions.Enable();
            _inputActions.PlayerPC.Attack.performed += ControlDownPC;
            _inputActions.PlayerPC.Jump.performed += ControlDownPC;
        }

        private void Start()
        {
            textComponent.text = string.Empty;
        }

        private void ControlDownPC(InputAction.CallbackContext x)
        {
            if (!_isInDialogue) return;

            if (textComponent.text == (GameManagerSingleton.Instance.language == Language.es ? m_genereicDialogue[index].textAreaES : m_genereicDialogue[index].textAreaEN))
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = (GameManagerSingleton.Instance.language == Language.es ? m_genereicDialogue[index].textAreaES : m_genereicDialogue[index].textAreaEN);
                m_genereicDialogue[index].onFinish.Invoke();
            }
        }

        public void StartDialogue(int index)
        {
            this.index = index;
            _isInDialogue = true;
            GameManagerSingleton.Instance.SetIsOnDialogue(true);

            foreach (var x in dialogues)
            {
                m_genereicDialogue.Add(x);
            }

            StartCoroutine(TypeLine());
        }

        private void NextLine()
        {
            if (index < m_genereicDialogue.Count - 1)
            {
                index++;
                StartCoroutine(TypeLine());
            }
            else
            {
                m_genereicDialogue[index].onFinish.Invoke();
                _isInDialogue = false;
                GameManagerSingleton.Instance.SetIsOnDialogue(false);
                onFinishAllDialogues.Invoke();
                gameObject.SetActive(false);
            }

        }

        private IEnumerator TypeLine()
        {

            if (m_genereicDialogue[index].sprite != null)
            {
                imageToChange.enabled = true;
                imageToChange.sprite = m_genereicDialogue[index].sprite;
            }
            else
            {
                imageToChange.enabled = false;
            }
            m_genereicDialogue[index].onStart.Invoke();
            titleComponent.text = m_genereicDialogue[index].title;

            textComponent.text = string.Empty;

            var x = GameManagerSingleton.Instance.language == Language.es ? m_genereicDialogue[index].textAreaES : m_genereicDialogue[index].textAreaEN;

            for (int i = 0; i < x.Length; i++)
            {

                textComponent.text = x.Substring(0, i);
                yield return new WaitForSecondsRealtime(textSpeed);
            }

            m_genereicDialogue[index].onFinish.Invoke();
        }
    }
}