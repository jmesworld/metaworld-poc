using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeolocationObject : MonoBehaviour
{
    [SerializeField]
    public GeolocationManager.Coords coords;

    [SerializeField]
    public float height = 0f;

    public Vector3 initialRot;

    GeolocationManager geolocationManager;

    public List<Vector3> positions = new List<Vector3>();

    [SerializeField]
    public Vector3 targetPos = new Vector3(0,0,0);
    [SerializeField]
    float smoothingSpeed = 2f;

    // Start is called before the first frame update
    void Start()
    {
        initialRot = this.transform.localEulerAngles;

        geolocationManager = GameObject.FindObjectOfType<GeolocationManager>();
        geolocationManager.AddObject(this);
    }

    // Update is called once per frame
    void Update()
    {
        bool approxEqual = Mathf.Approximately(transform.localPosition.x, targetPos.x) && Mathf.Approximately(transform.localPosition.y, targetPos.y) && Mathf.Approximately(transform.localPosition.z, targetPos.z);
        if (targetPos != null && !approxEqual)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * smoothingSpeed);
        }
    }
}
