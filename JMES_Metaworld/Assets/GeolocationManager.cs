using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEngine.XR.ARFoundation;

public class GeolocationManager : MonoBehaviour
{
    [System.Serializable]
    public class Coords
    {
        public double latitude;
        public double longitude;
        [HideInInspector]
        public float vAccuracy;
        [HideInInspector]
        public float hAccuracy;
        public double altitude;
        public Coords(double latitude, double longitude)
        {
            this.longitude = longitude;
            this.latitude = latitude;
        }

        public Coords(double latitude, double longitude, double altitude, float vAccuracy, float hAccuracy)
        {
            this.latitude = latitude;
            this.longitude = longitude;
            this.vAccuracy = vAccuracy;
            this.hAccuracy = hAccuracy;
            this.altitude = altitude;
        }
    }

    const double equRadius = 6378137d;
    const double polRadius = 6356752.3142d;

    double vRadius = 6378137d;

    public bool useOffset = true;

    ARAnchorManager anchorManager;

    const double deg2rad = Math.PI / 180d;

    Coords originCoords;

    Vector2 accuracy = new Vector2(100f, 100f);

    List<GeolocationObject> objectList = new List<GeolocationObject>();

    [SerializeField]
    public UnityEvent<Coords> onNewData = new UnityEvent<Coords>();

    [SerializeField]
    Coords debugCoords;

    [SerializeField]
    float maxDistance = 200f;

    CompassOffset compassOffset;
    float rotOffset = 0f;

    bool locationIsReady = false;

    [SerializeField]
    public bool averaging = true;
    [SerializeField]
    int averagingCount = 30;

    // Start is called before the first frame update
    void Start()
    {
        //Camera.main.transform.parent.GetComponent<CompassOffset>().headingOffsetUpdated.AddListener((offset) =>
        //{
        //    rotOffset = offset;
        //    foreach (GeolocationObject geoObj in objectList)
        //    {
        //        UpdatePosition(geoObj, geoObj.coords);
        //    }
        //});

        //StartCoroutine(StartLocationService());

        anchorManager = this.GetComponent<ARAnchorManager>();
        locationIsReady = NativeGPSPlugin.StartLocation();
        Debug.Log(ComputeRadiusByLatitude(debugCoords.latitude) + debugCoords.altitude);
    }

    // Update is called once per frame
    void Update()
    {
        if (locationIsReady)
        {
#if UNITY_EDITOR
            foreach (GeolocationObject geoObj in objectList)
            {
                originCoords = debugCoords;
                UpdatePosition(geoObj, geoObj.coords);
            }
#else

            Coords coords = new Coords(NativeGPSPlugin.GetLatitude(), NativeGPSPlugin.GetLongitude(), NativeGPSPlugin.GetAltitude(), NativeGPSPlugin.GetAccuracy(), NativeGPSPlugin.GetVerticalAccuracyMeters());
            onNewData.Invoke(coords);
            if (true)
            {
                originCoords = coords;
                accuracy.x = coords.hAccuracy;
                accuracy.y = coords.vAccuracy;

                vRadius = ComputeRadiusByLatitude(coords.latitude) + coords.altitude;
                foreach (GeolocationObject geoObj in objectList)
                {
                    UpdatePosition(geoObj, geoObj.coords);
                }
            }

#endif
        }



    }

    public void ToggleArTracking(bool enable)
    {
        anchorManager.enabled = enable;
    }

    double ComputeRadiusByLatitude(double latitude)
    {
        var radius = Math.Sqrt(
            (
                Math.Pow((Math.Pow(equRadius, 2) * Math.Cos(deg2rad * latitude)), 2)
                + Math.Pow((Math.Pow(polRadius, 2) * Math.Sin(deg2rad * latitude)), 2)
            )
            / (
                Math.Pow(((equRadius) * Math.Cos(deg2rad * latitude)), 2)
                + Math.Pow(((polRadius) * Math.Sin(deg2rad * latitude)), 2)
            )
        );
        return radius;
    }

    double ComputeDistanceMeters(Coords src, Coords dest, double radius)
    {
        var dlongitude = deg2rad * (dest.longitude - src.longitude);
        var dlatitude = deg2rad * (dest.latitude - src.latitude);
        var a = (Math.Sin(dlatitude / 2) * Math.Sin(dlatitude / 2)) + Math.Cos(deg2rad * (src.latitude)) * Math.Cos(deg2rad * (dest.latitude)) * (Math.Sin(dlongitude / 2) * Math.Sin(dlongitude / 2));
        var angle = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        var distance = angle * radius;

        return distance;
    }

    public void AddObject(GeolocationObject geoObj)
    {
        objectList.Add(geoObj);
        UpdatePosition(geoObj, geoObj.coords);
        geoObj.gameObject.AddComponent<ARAnchor>();
    }


    void UpdatePosition(GeolocationObject obj, Coords coords)
    {
        if (originCoords == null)
            return;

        Vector3 position = new Vector3(0, 0, 0);

        // update position.x
        Coords dstCoords = new Coords(
            originCoords.latitude,
            coords.longitude
        );

        position.x = (float)ComputeDistanceMeters(originCoords, dstCoords, vRadius);
        position.x *= coords.longitude > originCoords.longitude ? 1 : -1;

        // update position.z
        dstCoords = new Coords(
            coords.latitude,
            originCoords.longitude
        );

        position.z = (float)ComputeDistanceMeters(originCoords, dstCoords, polRadius);
        position.z *= coords.latitude > originCoords.latitude ? 1 : -1;

        position.y = obj.height;

        // return position in 3D world
        

        if (useOffset)
        {
            var camPos = Camera.main.transform.position;
            //Move object by cameraoffset from center
            position = position + camPos;
        }

        Vector3 pos = new Vector3(0,0,0);

        if (averaging)
        {
            obj.positions.Add(position);
            if (obj.positions.Count > averagingCount)
                obj.positions.RemoveAt(0);
            foreach(Vector3 vec in obj.positions)
            {
                pos += vec;

            };

            pos /= obj.positions.Count;

            obj.transform.localPosition = pos;
        }
        //Only update geolocation if distance gets bigger than maxDistance
        else if (!anchorManager.enabled || Vector3.Distance(obj.transform.localPosition, position) > 5)
            obj.transform.localPosition = position;


        //obj.transform.localEulerAngles = new Vector3(obj.initialRot.x, obj.initialRot.y, obj.initialRot.z);
        //obj.transform.Rotate(Vector3.up, rotOffset);

        if (Vector3.Distance(obj.transform.position, this.transform.position) > maxDistance)
        {
            obj.gameObject.SetActive(false);
        }
        else
        {
            obj.gameObject.SetActive(true);
        }
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
        Input.location.Start();

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
            //onNewData.Invoke(Input.location.lastData);
        }
    }
}
