using UnityEngine;

namespace Baks
{
    public class PerlinNoiseScroller
    {
        PerlinNoiseData m_data;
        Vector3 m_noiseOffset, m_noise;
     
        public Vector3 Noise => m_noise;
       
        public PerlinNoiseScroller(PerlinNoiseData data)
        {
            m_data = data;

            var rand = 32f;
            m_noiseOffset.x = Random.Range(0f, rand);
            m_noiseOffset.y = Random.Range(0f, rand);
            m_noiseOffset.z = Random.Range(0f, rand);
        }

        public void UpdateNoise()
        {
            var scrollOffset = Time.deltaTime * m_data.frequency;
            m_noiseOffset.x += scrollOffset;
            m_noiseOffset.y += scrollOffset;
            m_noiseOffset.z += scrollOffset;

            m_noise.x = Mathf.PerlinNoise(m_noiseOffset.x, 0f);
            m_noise.y = Mathf.PerlinNoise(m_noiseOffset.x, 1f);
            m_noise.z = Mathf.PerlinNoise(m_noiseOffset.x, 2f);

            m_noise -= Vector3.one * .5f;
            m_noise *= m_data.amplitude;
        }
    }
}
