using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Hedenrag.ExVar;

namespace Hedenrag
{

    [CustomPropertyDrawer(typeof(RangeFloatValue))]
    [CustomPropertyDrawer(typeof(RangeIntValue))]
    public class RangeValueEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //base.OnGUI(position, property, label);

            var minValProperty = property.FindPropertyRelative("minVal");
            var maxValProperty = property.FindPropertyRelative("maxVal");
            var currentValProperty = property.FindPropertyRelative("currentVal");

            
            if(minValProperty.boxedValue is int)
            {
                if (minValProperty.intValue > maxValProperty.intValue)
                {
                    (minValProperty.intValue, maxValProperty.intValue) = (maxValProperty.intValue, minValProperty.intValue);
                }
            }
            else
            {
                if (minValProperty.floatValue > maxValProperty.floatValue)
                {
                    (minValProperty.floatValue, maxValProperty.floatValue) = (maxValProperty.floatValue, minValProperty.floatValue);
                }
            }

            EditorGUIUtility.labelWidth = 58f;

            position.width /= 3f;

            position.width /= 2f;
            EditorGUI.LabelField(position, label);
            position.x += position.width;
            //can addStuff
            position.x += position.width;
            position.width += position.width;
            
            EditorGUI.PropertyField(position, minValProperty);
            position.x += position.width;
            EditorGUI.PropertyField(position, maxValProperty);

        }

    }

}