using UnityEngine;

namespace Baks
{
    [CreateAssetMenu(menuName = "DeveloperConsole/Commands/Log Command")]
    public class LogCommand : ConsoleCommand
    {
        public override bool Process(string[] args)
        {
            var logText = string.Join(" ", args);
            Debug.Log(logText);
            return true;
        }
    }
}
