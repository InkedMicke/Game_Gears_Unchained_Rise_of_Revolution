using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class DialogueClass
{
    [SerializeField] private string nameOfElement;
    public string title;
    [TextArea] public string textArea;
}

public class GDialogue : MonoBehaviour
{
    private PlayerInputActions _inputActions;

    [SerializeField] private TextMeshProUGUI textComponent;

    [SerializeField] private float textSpeed = 0.3f;

    [SerializeField] private List<DialogueClass> dialoguesES;

    [SerializeField] private List<DialogueClass> dialoguesEN;

    private List<DialogueClass> m_genereicDialogue;

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

        if (textComponent.text == m_genereicDialogue[index].textArea)
        {
            NextLine();
        }
        else
        {
            StopAllCoroutines();
            textComponent.text = m_genereicDialogue[index].textArea;
        }
    }

    public void StartDialogue(int index)
    {
        this.index = index;
        _isInDialogue = true;
        switch(GameManagerSingleton.Instance.language)
        {
            case Language.es:
                foreach(var x in dialoguesES)
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
        GameManagerSingleton.Instance.ShowCursor(true);
    }

    private void NextLine()
    {

    }

    private IEnumerator TypeLine()
    {
        while (index < m_genereicDialogue.Count)
        {
            foreach (char c in m_genereicDialogue[index].textArea.ToCharArray())
            {
                textComponent.text += c;
                yield return new WaitForSeconds(textSpeed);
            }

            index++;
            yield return new WaitForEndOfFrame();
        }

        _isInDialogue = false;
    }
}
