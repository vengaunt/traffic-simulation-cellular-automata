using System;
using KalkuzSystems;
using KalkuzSystems.Attributes;
using UnityEngine;
using Header = KalkuzSystems.Attributes.HeaderAttribute;

namespace TrafficSimulation.Control
{
    public class CameraController : MonoBehaviour
    {
        [Header("General")] 
        [SerializeField] private float interpolationMultiplier;
        [SerializeField] private float panSensitivityAtMinZoom = 200;
        [SerializeField] private float panSensitivityAtMaxZoom = 30;
        
        [Header("Orthographic Settings")]        
        [SerializeField] private float orthographicZoomSensitivity;
        [SerializeField] private float minOrthographicSize = 1;
        [SerializeField] private float maxOrthographicSize = 8;

        [Header("Perspective Settings")] 
        [SerializeField] private float perspectiveZoomSensitivity;
        [SerializeField] private float minYLevel = 10;
        [SerializeField] private float maxYLevel = 150;
        
        private Camera cam;
        private Transform cameraTransform;
        private float targetCameraY;
        private float targetOrthographicSize;

        private Vector3 touchStart;

        private void Awake()
        {
            cam = GetComponent<Camera>();
            if (cam != null)
            {
                cameraTransform = cam.transform;
                targetCameraY = cameraTransform.position.y;
                targetOrthographicSize = cam.orthographicSize;
            }
        }

        void Update()
        {
            if (cam == null) return;

            if (Input.GetMouseButtonDown(0))
            {
                touchStart = cam.ScreenToViewportPoint(Input.mousePosition);
            }

            if (Input.touchCount == 2)
            {
                Touch firstTouch = Input.GetTouch(0);
                Touch secondTouch = Input.GetTouch(1);

                Vector2 touchZeroPrevPos = firstTouch.position - firstTouch.deltaPosition;
                Vector2 touchOnePrevPos = secondTouch.position - secondTouch.deltaPosition;

                float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float currentMagnitude = (firstTouch.position - secondTouch.position).magnitude;

                float difference = currentMagnitude - prevMagnitude;

                Zoom(difference * 0.01f);
            }
            else if (Input.GetMouseButton(0))
            {
                var currentMousePosition = cam.ScreenToViewportPoint(Input.mousePosition);
                Vector3 direction = touchStart - currentMousePosition;
                
                // adjust aspectRatio
                direction = new Vector3(direction.x, direction.y * Screen.height * (1f / Screen.width));

                var factoredSensitivity = 0f;
                if (cam.orthographic) factoredSensitivity = Maths.Remap(targetOrthographicSize, maxOrthographicSize, minOrthographicSize, panSensitivityAtMinZoom, panSensitivityAtMaxZoom);
                else factoredSensitivity = Maths.Remap(targetCameraY, maxYLevel, minYLevel, panSensitivityAtMinZoom, panSensitivityAtMaxZoom);
                
                cameraTransform.position -= new Vector3(direction.x, 0f, direction.y) * factoredSensitivity;
                
                touchStart = currentMousePosition;
            }

            Zoom(Input.GetAxis("Mouse ScrollWheel"));
        }

        private void LateUpdate()
        {
            bool isOrtho = cam.orthographic;
            
            if (isOrtho) cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetOrthographicSize, Time.unscaledDeltaTime * interpolationMultiplier);
            else
            {
                var position = cameraTransform.position;
                position = Vector3.Lerp(position, new Vector3(position.x, targetCameraY, position.z), Time.unscaledDeltaTime * interpolationMultiplier);
                cameraTransform.position = position;
            }
        }

        void Zoom(float increment)
        {
            bool isOrtho = cam.orthographic;
            
            if (isOrtho) targetOrthographicSize = Mathf.Clamp(targetOrthographicSize - increment * orthographicZoomSensitivity, minOrthographicSize, maxOrthographicSize);
            else
            {
                targetCameraY = Mathf.Clamp(targetCameraY - increment * perspectiveZoomSensitivity, minYLevel, maxYLevel);
            }
        }
    }
}