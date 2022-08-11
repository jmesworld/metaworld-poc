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
        
    }
}
