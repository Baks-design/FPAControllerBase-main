using System.Collections.Generic;
using System.Text;
using Unity.Profiling;
using Unity.Profiling.LowLevel.Unsafe;
using UnityEngine;

namespace Baks.Runtime.Utils
{
    public class ProfilerStats
    {
        struct StatInfo
        {
            public string Name;
            public ProfilerCategory Cat;
            public ProfilerMarkerDataUnit Unit;
        }

        public static void EnumerateProfilerStats()
        {
            var availableStatHandles = new List<ProfilerRecorderHandle>();
            ProfilerRecorderHandle.GetAvailable(availableStatHandles);

            var availableStats = new List<StatInfo>(availableStatHandles.Count);
            foreach (var h in availableStatHandles)
            {
                var statDesc = ProfilerRecorderHandle.GetDescription(h);
                var statInfo = new StatInfo()
                {
                    Cat = statDesc.Category,
                    Name = statDesc.Name,
                    Unit = statDesc.UnitType
                };
                availableStats.Add(statInfo);
            }
            availableStats.Sort((a, b) =>
            {
                var result = string.Compare(a.Cat.ToString(), b.Cat.ToString());
                if (result != 0)
                    return result;

                return string.Compare(a.Name, b.Name);
            });

            var sb = new StringBuilder("Available stats:\n");
            foreach (var s in availableStats)
                sb.AppendLine($"{s.Cat.ToString()}\t\t - {s.Name}\t\t - {s.Unit}");

            FileManager.WriteToFile("AvailableStats.txt", sb.ToString());
#if UNITY_EDITOR
            Debug.Log(sb.ToString());
#endif
        }
    }
}