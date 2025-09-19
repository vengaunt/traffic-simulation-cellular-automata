using System;
using TrafficSimulation.DataStructures.Grids;
using UnityEngine;

namespace KalkuzSystems.Analysis.Debugger
{
    /// <summary>
    /// An extended version of the <see cref="Debug"/>.
    /// </summary>
    public class Debugger : MonoBehaviour
    {
        /// <summary>
        /// Instance of the Debugger
        /// </summary>
        private static Debugger Instance { get; set; }
        
        /// <summary>
        /// Used to determine how the <see cref="Debugger"/> going to behave.
        /// </summary>
        [SerializeField, Tooltip("Debugger settings to be obeyed.")] 
        private DebuggerSettings settings;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            if (settings == null) throw new Exception("Debugger settings are required.");
        }

        /// <summary>
        /// Logs the message to Unity's console.
        /// </summary>
        /// <param name="message">Message to be written</param>
        public static void Log(string message)
        {
            if (Instance == null)
            {
                Debug.LogWarning($"Debugger should be initialized in scene.");
                return;
            }

            if (!Application.isEditor && Instance.settings.EditorOnly) return;
            if (!Instance.settings.LogsEnabled) return;

            var color = ColorUtility.ToHtmlStringRGBA(Instance.settings.LogColor);
            Debug.Log($"<color=#{color}>{message}</color>");
        }

        /// <summary>
        /// Prints Warning to the Unity console.
        /// </summary>
        /// <param name="message">Message to be written</param>
        public static void Warning(string message)
        {
            if (Instance == null)
            {
                Debug.LogWarning("Debugger should be initialized in scene.");
                return;
            }

            if (!Application.isEditor && Instance.settings.EditorOnly) return;
            if (!Instance.settings.WarningsEnabled) return;

            var color = ColorUtility.ToHtmlStringRGBA(Instance.settings.WarningColor);
            Debug.LogWarning($"<color=#{color}>{message}</color>");
        }

        /// <summary>
        /// Prints Error to the Unity console.
        /// </summary>
        /// <param name="message">Message to be written</param>
        public static void Error(string message)
        {
            if (Instance == null)
            {
                Debug.LogWarning("Debugger should be initialized in scene.");
                return;
            }

            if (!Application.isEditor && Instance.settings.EditorOnly) return;
            if (!Instance.settings.ErrorsEnabled) return;

            var color = ColorUtility.ToHtmlStringRGBA(Instance.settings.ErrorColor);
            Debug.LogError($"<color=#{color}>{message}</color>");
        }
    }
}