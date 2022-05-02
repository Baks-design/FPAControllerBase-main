using UnityEngine;

namespace Baks.Runtime.Misc
{
    [RequireComponent(typeof(MeshRenderer))]
    [DisallowMultipleComponent]
    public class RandomObjectColor : MonoBehaviour
    {
        MeshRenderer m_MeshRenderer;

        void Start()
        {
            TryGetComponent(out m_MeshRenderer);
            m_MeshRenderer.material.color = Random.ColorHSV(0, 1, .75f, 1, .5f, 1);
        }
    }
}