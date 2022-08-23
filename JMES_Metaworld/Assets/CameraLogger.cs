using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class CameraLogger : MonoBehaviour
{
    TextMeshProUGUI textMesh;
    Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        textMesh = this.GetComponent<TextMeshProUGUI>();
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        textMesh.text = "CamInfo:" +
            "\nPos: " + cam.transform.localPosition.ToString("F4") +
            "\nRot: " + cam.transform.localEulerAngles.ToString("F4") +
            "\nRootRot: " + cam.transform.parent.localEulerAngles.ToString("F4");
    }
}
