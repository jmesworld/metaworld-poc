using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GeolocationManager : MonoBehaviour
{

    public class Coords
    {
        public float longitude;
        public float latitude;
        public Coords(float longitude, float latitude)
        {
            this.longitude = longitude;
            this.latitude = latitude;
        }
    }

    Coords originCoords;

    Vector2 accuracy = new Vector2(100f,100f);

    List<GeolocationObject> objectList = new List<GeolocationObject>();

    [SerializeField]
    public UnityEvent<LocationInfo> onNewData = new UnityEvent<LocationInfo>();

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartLocationService());
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.location.status == LocationServiceStatus.Running)
        {
            LocationInfo lastData = Input.location.lastData;
            if (lastData.horizontalAccuracy < accuracy.x || lastData.verticalAccuracy < accuracy.y)
            {
                originCoords = new Coords(lastData.longitude, lastData.latitude);
                accuracy.x = lastData.horizontalAccuracy;
                accuracy.y = lastData.verticalAccuracy;

                foreach (GeolocationObject geoObj in objectList)
                {
                    UpdatePosition(geoObj.transform, geoObj.coords);
                }
            }
        }

    }

    float ComputeDistanceMeters(Coords src, Coords dest)
    {
        var dlongitude = Mathf.Deg2Rad * (dest.longitude - src.longitude);
        var dlatitude = Mathf.Deg2Rad * (dest.latitude - src.latitude);
        var a = (Mathf.Sin(dlatitude / 2) * Mathf.Sin(dlatitude / 2)) + Mathf.Cos(Mathf.Deg2Rad * (src.latitude)) * Mathf.Cos(Mathf.Deg2Rad * (dest.latitude)) * (Mathf.Sin(dlongitude / 2) * Mathf.Sin(dlongitude / 2));
        var angle = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));
        var distance = angle * 6378160;

        return distance;
    }

    void AddObject(GeolocationObject geoObj)
    {
        objectList.Add(geoObj);
        UpdatePosition(geoObj.transform, geoObj.coords);
    }

    void UpdatePosition(Transform transform, Coords coords)
    {
        if (originCoords == null)
            return;

        Vector3 position = new Vector3(0, 0, 0);

        // update position.x
        Coords dstCoords = new Coords(coords.longitude, originCoords.latitude);

        position.x = ComputeDistanceMeters(originCoords, dstCoords);
        position.x *= coords.longitude > originCoords.longitude ? 1 : -1;

        // update position.z
        dstCoords = new Coords(
            originCoords.longitude,
            coords.latitude);

        position.z = ComputeDistanceMeters(originCoords, dstCoords);
        position.z *= coords.latitude > originCoords.latitude ? -1 : 1;

        // return position in 3D world
        transform.localPosition = position;

        return;
    }

    IEnumerator StartLocationService()
    {
        // Check if the user has location service enabled.
        if (!Input.location.isEnabledByUser)
        {
            Debug.LogWarning("Location Service Not Enabled by User.");
            yield break;
        }

        // Starts the location service.
        Input.location.Start(1);

        // Waits until the location service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // If the service didn't initialize in 20 seconds this cancels location service use.
        if (maxWait < 1)
        {
            Debug.LogWarning("Location Service Timed out.");
            yield break;
        }

        // If the connection failed this cancels location service use.
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogWarning("Unable to determine device location");
            yield break;
        }
        else
        {
            // If the connection succeeded, this retrieves the device's current location and displays it in the Console window.
            print("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
        }
    }
}
