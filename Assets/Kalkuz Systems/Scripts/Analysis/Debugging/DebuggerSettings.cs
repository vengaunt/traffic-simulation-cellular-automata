using KalkuzSystems.Attributes;
using UnityEngine;

namespace KalkuzSystems.Analysis.Debugger
{
    /// <summary>
    /// Settings for how the <see cref="Debugger"/> going to behave.
    /// </summary>
    [CreateAssetMenu(menuName = "Kalkuz Systems/Settings/Debugger Settings", fileName = "New Debugger Settings")]
    public class DebuggerSettings : ScriptableObject
    {
        [SerializeField, Tooltip("Logs messages only in the Editor if true.")] private bool editorOnly = true;
        [SerializeField, Tooltip("Whether or not the Logs should be printed.")] private bool logsEnabled = true;
        [SerializeField, Tooltip("Whether or not the Warnings should be printed.")] private bool warningsEnabled = true;
        [SerializeField, Tooltip("Whether or not the Errors should be printed.")] private bool errorsEnabled = true;

        [LineSeparator(1, 20)]
        [SerializeField, Tooltip("Logs' Text Color")] private Color logColor = Color.white;
        [SerializeField, Tooltip("Logs' Text Color")] private Color warningColor = Color.yellow;
        [SerializeField, Tooltip("Logs' Text Color")] private Color errorColor = Color.red;

        /// <summary>
        /// Logs messages only in the Editor if true.
        /// </summary>
        public bool EditorOnly => editorOnly;
        
        /// <summary>
        /// Whether or not the Logs should be printed.
        /// </summary>
        public bool LogsEnabled => logsEnabled;
        
        /// <summary>
        /// Whether or not the Warnings should be printed.
        /// </summary>
        public bool WarningsEnabled => warningsEnabled;
        
        /// <summary>
        /// Whether or not the Errors should be printed.
        /// </summary>
        public bool ErrorsEnabled => errorsEnabled;
        
        /// <summary>
        /// The color of texts in log messages.
        /// </summary>
        public Color LogColor => logColor;
        
        /// <summary>
        /// The color of texts in warning messages.
        /// </summary>
        public Color WarningColor => warningColor;
        
        /// <summary>
        /// the color of texts in error messages.
        /// </summary>
        public Color ErrorColor => errorColor;
    }
}