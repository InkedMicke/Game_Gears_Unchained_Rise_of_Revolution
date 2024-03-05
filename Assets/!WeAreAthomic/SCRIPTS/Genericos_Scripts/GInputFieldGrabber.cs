using UnityEngine;
using TMPro;

public class GInputFieldGrabber : MonoBehaviour
{
    private enum TypesOfInput
    {
        MouseSensivityX,
        MouseSensivityY
    }

    [SerializeField] private TMP_InputField field;

    [SerializeField] private TypesOfInput typesOfInput;

    public void GrabFromInputField()
    {
        if(field.text == string.Empty)
            return;
        
        if(typesOfInput == TypesOfInput.MouseSensivityX)
        {
            //GameManager.Instance.sensivityX = int.Parse(field.text);
        }

        if (typesOfInput == TypesOfInput.MouseSensivityY)
        {
            //GameManager.Instance.sensivityY = int.Parse(field.text);
        }
    }
}
