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

    [SerializeField]
    CompassOffset compassOffset;
    [SerializeField]
    TextMeshProUGUI headingInfo;

    float heading;

    float headingOffset;

    // Start is called before the first frame update
    void Start()
    {
        if (geolocationManager != null)
        {
            geolocationManager.onNewData.AddListener(OnLocationUpdated);
        }

        if (compassOffset != null)
        {
            compassOffset.headingUpdated.AddListener(OnHeadingUpdated);
            compassOffset.headingOffsetUpdated.AddListener(OnHeadingOffsetUpdated);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnLocationUpdated(GeolocationManager.Coords info)
    {
        locationInfo.text =
            "Latitude: " + info.latitude +
            "\nLongitude: " + info.longitude +
            "\nAltitude: " + info.altitude + 
            "\nHAccuracy: " + info.hAccuracy +
            "\nVAccuracy: " + info.vAccuracy;
    }

    void OnHeadingUpdated(float heading)
    {
        this.heading = heading;

        headingInfo.text = "Heading: " + heading.ToString("F2") + "\nHeading Offset: " + headingOffset.ToString("F2");
    }

    void OnHeadingOffsetUpdated(float headingOffset)
    {
        this.headingOffset = headingOffset;

        headingInfo.text = "Heading: " + heading.ToString("F2") + "\nHeadingOffset: " + headingOffset.ToString("F2");
    }


}
