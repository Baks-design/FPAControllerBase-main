using System.Text;
using System.Collections.Generic;
using UnityEngine;
using Unity.Profiling;
using Baks.Runtime.Utils;

namespace Baks.Runtime.Controller
{
    [DisallowMultipleComponent]
    public class ProfilerController : MonoBehaviour
    {
        string m_StatsText;
        ProfilerRecorder m_SystemMemoryRecorder, m_GCMemoryRecorder, m_MainThreadTimeRecorder, m_DrawCallsCountRecorder;

        static double GetRecorderFrameAverage(ProfilerRecorder recorder)
        {
            var samplesCount = recorder.Capacity;
            if (samplesCount == 0)
                return 0;

            double r = 0;
            var samples = new List<ProfilerRecorderSample>(samplesCount);
            recorder.CopyTo(samples);
            for (var i = 0; i < samples.Count; ++i)
                r += samples[i].Value;
            r /= samplesCount;

            return r;
        }

        void OnEnable()
        {
            m_SystemMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "System Used Memory");
            m_GCMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Reserved Memory");
            m_MainThreadTimeRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "Main Thread", 15);
            m_DrawCallsCountRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count");

            ProfilerStats.EnumerateProfilerStats();
        }

        void OnDisable()
        {
            m_SystemMemoryRecorder.Dispose();
            m_GCMemoryRecorder.Dispose();
            m_MainThreadTimeRecorder.Dispose();
            m_DrawCallsCountRecorder.Dispose();
        }

        void Update()
        {
            var sb = new StringBuilder(500);
            sb.AppendLine($"Frame Time: {GetRecorderFrameAverage(m_MainThreadTimeRecorder) * (1e-6f):F1} ms");
            sb.AppendLine($"GC Memory: {m_GCMemoryRecorder.LastValue / (1024 * 1024)} MB");
            sb.AppendLine($"System Memory: {m_SystemMemoryRecorder.LastValue / (1024 * 1024)} MB");
            sb.AppendLine($"Draw Calls: {m_DrawCallsCountRecorder.LastValue}");
            m_StatsText = sb.ToString();
        }

        void OnGUI() => GUI.TextArea(new Rect(10, 30, 250, 65), m_StatsText);
    }
}