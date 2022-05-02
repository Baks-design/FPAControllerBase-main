using System;
using System.Collections;
using Baks.Runtime.Utils;
using UnityEngine;

namespace Baks
{
    public static class ExtensionMethods
    {
        public static void CallWithDelay(this MonoBehaviour mono, Action method, float delay) => mono.StartCoroutine(CallWithDelayRoutine(method, delay));

        static IEnumerator CallWithDelayRoutine(Action method, float delay)
        {
            yield return Helpers.GetWait(delay);
            method();
        }
    }
}
