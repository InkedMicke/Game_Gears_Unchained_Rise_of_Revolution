using UnityEngine;
using UnityEditor;
using _WeAreAthomic.SCRIPTS.Props;
using UnityEngine.Events;

namespace EditorScripts
{
    [CustomEditor(typeof(RequiredActionForButton))]
    public class InteractButtonHiddenVariables : Editor
    {
        public override void OnInspectorGUI()
        {
            RequiredActionForButton miScript = (RequiredActionForButton)target;

            // Dibuja el campo mostrarParametro
            miScript.isRequiredAction = EditorGUILayout.Toggle("Se necesita algo para activarse?", miScript.isRequiredAction);

            // Si mostrarParametro está activo, muestra el campo parametroOculto
            if (miScript.isRequiredAction)
            {
                miScript.requiredObject = (GameObject)EditorGUILayout.ObjectField("Objeto referencia", miScript.requiredObject, typeof(GameObject), true);
            }
            // Actualiza los cambios hechos en el Inspector
            if (GUI.changed)
            {
                EditorUtility.SetDirty(miScript);
            }
        }
    }
}
