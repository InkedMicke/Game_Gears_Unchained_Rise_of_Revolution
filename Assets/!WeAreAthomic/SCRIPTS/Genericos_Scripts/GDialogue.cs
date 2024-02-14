using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[Serializable]
public class DialogueClass
{
    [SerializeField] private string nameOfElement;
    public string title;
    public Sprite sprite;
    [TextArea] public string textArea;
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

    [SerializeField] private List<DialogueClass> dialoguesES;

    [SerializeField] private List<DialogueClass> dialoguesEN;

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
        GameManagerSingleton.Instance.SetIsOnDialogue(false);
    }

    private void ControlDownPC(InputAction.CallbackContext x)
    {
        if (!_isInDialogue) return;

        if (textComponent.text == m_genereicDialogue[index].textArea)
        {
            NextLine();
        }
        else
        {
            StopAllCoroutines();
            textComponent.text = m_genereicDialogue[index].textArea;
            m_genereicDialogue[index].onFinish.Invoke();
        }
    }

    public void StartDialogue(int index)
    {
        this.index = index;
        _isInDialogue = true;
        GameManagerSingleton.Instance.SetIsOnDialogue(true);
        switch (GameManagerSingleton.Instance.language)
        {
            case Language.es:
                foreach (var x in dialoguesES)
                {
                    m_genereicDialogue.Add(x);
                }
                break;
            case Language.en:
                foreach (var x in dialoguesEN)
                {
                    m_genereicDialogue.Add(x);
                }
                break;
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
        m_genereicDialogue[index].title = titleComponent.text;
        textComponent.text = string.Empty;
        foreach (char c in m_genereicDialogue[index].textArea.ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSecondsRealtime(textSpeed);
        }

        m_genereicDialogue[index].onFinish.Invoke();
    }
}
