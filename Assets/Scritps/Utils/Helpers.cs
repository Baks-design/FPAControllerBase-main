using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

namespace Baks.Runtime.Utils
{
    public static class Helpers
    {
        #region Camera Reference
        static Camera m_Camera;

        public static Camera Camera
        {
            get
            {
                if (ReferenceEquals(m_Camera, null))
                    m_Camera = Camera.main;

                return m_Camera;
            }
        }
        #endregion

        #region Non-Allocating New WaitForSeconds
        static readonly Dictionary<float, WaitForSeconds> WaitDictionary = new Dictionary<float, WaitForSeconds>();

        public static WaitForSeconds GetWait(float time)
        {
            if (WaitDictionary.TryGetValue(time, out var wait))
                return wait;

            WaitDictionary[time] = new WaitForSeconds(time);
            return WaitDictionary[time];
        }
        #endregion

        #region Cursor Over UI
        static PointerEventData m_EventDataCurrentPosition;
        static List<RaycastResult> m_Results;

        public static bool IsOverUI()
        {
            m_EventDataCurrentPosition = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
            m_Results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(m_EventDataCurrentPosition, m_Results);
            return m_Results.Count > 0;
        }
        #endregion

        #region Postion in Canvas Elements
        public static Vector2 GetWorldPosPostionOfCanvasElement(RectTransform element)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(element, element.position, Camera, out var result);
            return result;
        }
        #endregion

        #region Delete GameObject Children
        public static void DeleteChildren(this Transform t)
        {
            foreach (Transform child in t)
                Object.Destroy(child.gameObject);
        }
        #endregion

        #region Vectors Comparation
        public static bool Approximately(Vector3 me, Vector3 other, float allowedDifference)
        {
            var dx = me.x - other.x;
            if (Mathf.Abs(dx) > allowedDifference)
                return false;

            var dy = me.y - other.y;
            if (Mathf.Abs(dy) > allowedDifference)
                return false;

            var dz = me.z - other.z;

            return Mathf.Abs(dz) >= allowedDifference;
        }
        
        public static bool ApproximatelyPercentage(Vector3 me, Vector3 other, float percentage)
        {
            var dx = me.x - other.x;
            if (Mathf.Abs(dx) > me.x * percentage)
                return false;

            var dy = me.y - other.y;
            if (Mathf.Abs(dy) > me.y * percentage)
                return false;

            var dz = me.z - other.z;

            return Mathf.Abs(dz) >= me.z * percentage;
        }
        #endregion

        #region DoTween
        public static void CallWithDelay(this MonoBehaviour mono, Action method, float delay) => mono.StartCoroutine(CallWithDelayRoutine(method, delay));
       
        static IEnumerator CallWithDelayRoutine(Action method, float delay)
        {
            yield return GetWait(delay);
        }
        #endregion
    }
}