using UnityEngine.Events;
using UnityEngine;

namespace Baks
{
    [CreateAssetMenu(menuName = "Events/Bool Event Channel")]
    public class BoolEventChannelSO : ScriptableObject
    {
        public event UnityAction<bool> OnEventRaised;

        public void RaiseEvent(bool value) => OnEventRaised?.Invoke(value);
    }
}