using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KalkuzSystems.DataStructures.AstronomicalNumbers
{
    public class AstroDebug : MonoBehaviour
    {
        public AstronomicalNumber number;
        public AstronomicalNumber increaseAmount;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                number += increaseAmount;
            }
        }

        private void OnGUI()
        {
            GUI.Label(new Rect(100, 100, 100, 100), number.ToString(), new GUIStyle() { fontSize = 72 });
        }
    }
}
