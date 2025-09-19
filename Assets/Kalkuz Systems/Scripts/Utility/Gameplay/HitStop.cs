using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KalkuzSystems.Utility.Gameplay
{
    public static class HitStop
    {
        private static bool m_timeSlowed = false;

        public static void Apply(MonoBehaviour caller, float timeSpeed, float duration)
        {
            if (m_timeSlowed) return;
            caller.StartCoroutine(HitStopCoroutine(timeSpeed, duration));
        }

        static IEnumerator HitStopCoroutine(float timeSpeed, float duration)
        {
            float defaultTimeSpeed = Time.timeScale;

            m_timeSlowed = true;
            Time.timeScale = timeSpeed;
            yield return new WaitForSecondsRealtime(duration);
            Time.timeScale = defaultTimeSpeed;
            m_timeSlowed = false;
        }
    }
}
