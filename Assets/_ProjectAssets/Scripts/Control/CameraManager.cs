using System;
using UnityEngine;
using UnityEngine.UI;

namespace TrafficSimulation.Control
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private float clickTimeRange;
        [SerializeField] private Camera mainCam;
        [SerializeField] private LayerMask vehicleLayer;

        [SerializeField] private Button escapeButton;
        
        private float clickTime;
        private Camera activeCam;

        private void Start()
        {
            activeCam = mainCam;
            escapeButton.gameObject.SetActive(false);
            escapeButton.onClick.AddListener(Escape);
        }

        void Escape()
        {
            escapeButton.gameObject.SetActive(false);

            activeCam.enabled = false;
            mainCam.enabled = true;
            activeCam = mainCam;
        }
        
        private void Update()
        {
            if (activeCam.gameObject.activeInHierarchy == false) Escape();
            
            if (Input.GetMouseButtonUp(0))
            {
                if (clickTime < clickTimeRange && activeCam == mainCam)
                {
                    Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out var hit, 1000, vehicleLayer))
                    {
                        var vehicleCam = hit.collider.GetComponentInChildren<Camera>();
                        
                        activeCam.enabled = false;
                        vehicleCam.enabled = true;
                        activeCam = vehicleCam;
                        
                        escapeButton.gameObject.SetActive(true);
                    }
                }
                
                clickTime = 0f;
            }
            else if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
            {
                clickTime += Time.deltaTime;
            }
        }
    }
}