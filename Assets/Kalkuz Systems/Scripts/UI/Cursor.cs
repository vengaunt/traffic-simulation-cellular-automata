using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    public static Cursor Instance { get; set; }
    [SerializeField] private Canvas highlightCanvas;
    [SerializeField] private TextMeshProUGUI highlightText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    private void Start()
    {
        highlightCanvas.enabled = false;
    }

    private void LateUpdate()
    {
        transform.position = Input.mousePosition;
    }

    public void ToggleHighlight(bool enable)
    {
        highlightCanvas.enabled = enable;
    }
}
