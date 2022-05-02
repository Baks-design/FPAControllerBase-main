using UnityEngine;

namespace Baks
{
    public enum TransformTarget
    {
        Position,
        Rotation,
        Both
    }

    [CreateAssetMenu(menuName = "Player/Data/PerlinNoiseData")]
    public class PerlinNoiseData : ScriptableObject
    {
        public TransformTarget transformTarget;
        public float amplitude, frequency;
    }
}