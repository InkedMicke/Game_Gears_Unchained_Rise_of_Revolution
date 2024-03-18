using System;
using System.Collections;
using UnityEngine;

namespace Generics
{
    public static class Utilities
    {
        public static void Invoke(this MonoBehaviour mb, Action f, float delay)
        {
            mb.StartCoroutine(InvokeRoutine(f, delay));
        }

        private static IEnumerator InvokeRoutine(Action f, float delay)
        {
            yield return new WaitForSeconds(delay);
            f();
        }
    }
}