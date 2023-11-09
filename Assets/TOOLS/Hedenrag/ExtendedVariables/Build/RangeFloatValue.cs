using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Hedenrag
{
    namespace ExVar
    {
        [Serializable]
        public struct RangeFloatValue : ISerializationCallbackReceiver
        {
            static System.Random rng = new System.Random();

            float currentVal;

            [SerializeField] float minVal;
            [SerializeField] float maxVal;

            public readonly float value => currentVal;
            public readonly float possibleValue => UnityEngine.Random.Range(minVal, maxVal);

            public void RecalculateValue()
            {
                currentVal = (float) rng.NextDouble();
                maxVal = (currentVal * (maxVal - minVal)) + minVal;
            }


            public RangeFloatValue(float minVal = 0f, float maxVal = 1f)
            {
                this.minVal = minVal;
                this.maxVal = maxVal;

                currentVal = UnityEngine.Random.Range(minVal, maxVal);
                Debug.Log("Object Created");
            }
            public RangeFloatValue(RangeFloatValue rangeIntValue)
            {
                currentVal = rangeIntValue.value;
                minVal = rangeIntValue.minVal;
                maxVal = rangeIntValue.maxVal;
            }

            public void OnBeforeSerialize(){}
            public void OnAfterDeserialize(){ RecalculateValue(); }
        }
    }
}
