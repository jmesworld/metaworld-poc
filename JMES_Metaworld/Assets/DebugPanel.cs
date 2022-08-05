using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugPanel : MonoBehaviour
{

    [SerializeField]
    GeolocationManager geolocationManager;

    [SerializeField]
    TextMeshProUGUI locationInfo;

    // Start is called before the first frame update
    void Start()
    {
        if(geolocationManager != null)
        {
            geolocationManager.onNewData.AddListener(UpdateFields);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateFields(LocationInfo info)
    {
        locationInfo.text = "Lat: " + info.latitude + "\nLon: " + info.longitude + "\nHeading: NaN";
    }
}
