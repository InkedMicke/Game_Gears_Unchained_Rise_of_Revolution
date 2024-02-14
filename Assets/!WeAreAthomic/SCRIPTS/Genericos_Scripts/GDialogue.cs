using NaughtyAttributes;
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

    [SerializeField] private List<DialogueClass> dialogues;

    private bool _isInDialogue;

    private int index;

    private void Awake()
    {
        _inputActions = new PlayerInputActions();
        _inputActions.Enable();
        _inputActions.PlayerPC.Attack.performed += ControlDownPC;
    }

    private void Start()
    {
        textComponent.text = string.Empty;
    }

    private void ControlDownPC(InputAction.CallbackContext x)
    {
        if (!_isInDialogue) return;

        if (textComponent.text == dialogues[index].textArea)
        {
            NextLine();
        }
        else
        {
            StopAllCoroutines();
            textComponent.text = dialogues[index].textArea;
        }
    }

    public void StartDialogue(int index)
    {
        this.index = index;
        _isInDialogue = true;
        StartCoroutine(TypeLine());
        GameManagerSingleton.Instance.ShowCursor(true);
    }

    private void NextLine()
    {

    }

    private IEnumerator TypeLine()
    {
        while (index < dialogues.Count)
        {
            foreach (char c in dialogues[index].textArea.ToCharArray())
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
