using Baks.Runtime.Utils;
using UnityEngine;

namespace Baks.Runtime.CameraSystem
{
    [RequireComponent(typeof(Camera))]
    [DisallowMultipleComponent]
    public class CullingGroupCameraBehaviour : MonoBehaviour
    {
        [SerializeField] float m_SetBoundingDistances = 50f;
        [SerializeField] float m_MultiplierBoundingRadius = 2f;

        CullingGroup m_CullingGroup;
        Transform[] m_Targets;
        BoundingSphere[] m_Bounds;

        void Start()
        {
            var gobjs = GameObject.FindGameObjectsWithTag(GlobalTags.CullingTag);
            m_Targets = new Transform[gobjs.Length];
            for (var i = 0; i < gobjs.Length; i++)
                m_Targets[i] = gobjs[i].transform;

            m_CullingGroup = new CullingGroup();
            m_CullingGroup.targetCamera = Camera.main;
            m_CullingGroup.SetDistanceReferencePoint(transform.position);
            m_CullingGroup.SetBoundingDistances(new float[] { m_SetBoundingDistances });

            m_Bounds = new BoundingSphere[m_Targets.Length];
            for (int i = 0; i < m_Bounds.Length; i++)
                m_Bounds[i].radius = Camera.main.rect.width * m_MultiplierBoundingRadius;
            m_CullingGroup.SetBoundingSpheres(m_Bounds);

            m_CullingGroup.SetBoundingSphereCount(m_Targets.Length);
            m_CullingGroup.onStateChanged = OnChange;
        }

        void Update()
        {
            for (var i = 0; i < m_Bounds.Length; i++)
                m_Bounds[i].position = m_Targets[i].position;
        }

        void OnDestroy()
        {
            m_CullingGroup.Dispose();
            m_CullingGroup = null;
        }

        void OnChange(CullingGroupEvent ev)
        {
            m_Targets[ev.index].gameObject.TryGetComponent(out MeshRenderer meshrenderer);
            meshrenderer.enabled = ev.hasBecomeVisible ? true : false;
        }
    }
}