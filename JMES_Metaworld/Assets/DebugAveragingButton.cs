using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugAveragingButton : MonoBehaviour
{
    GeolocationManager geoManager;

    TextMeshProUGUI textMesh;

    bool active = true;
    // Start is called before the first frame update
    void Start()
    {

        geoManager = FindObjectOfType<GeolocationManager>();
        active = geoManager.useOffset;

        Button button = this.GetComponent<Button>();

        button.onClick.AddListener(ButtonClicked);

        textMesh = button.GetComponentInChildren<TextMeshProUGUI>();
        textMesh.text = "Averaging: " + active.ToString();
    }

    void ButtonClicked()
    {
        active = !active;
        geoManager.averaging = active;

        textMesh.text = "Averaging: " + active.ToString();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
