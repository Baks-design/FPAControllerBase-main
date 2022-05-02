using UnityEngine;

namespace Baks
{
    [RequireComponent(typeof(BoxCollider))]
    [DisallowMultipleComponent]
    public class DoorTrigger : MonoBehaviour
    {
        [SerializeField] Door Door;

        void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out CharacterController controller))
                if (!Door.IsOpen)
                    Door.OpenDoor(other.transform.position);
        }

        void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out CharacterController controller))
                if (Door.IsOpen)
                    Door.CloseDoor();
        }
    }
}