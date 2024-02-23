using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Broom
{
    public class Broom : MonoBehaviour
    {
        protected BroomAnimator broomAnimator;

        private void OnValidate()
        {
            broomAnimator = GetComponent<BroomAnimator>();
        }
    }
}