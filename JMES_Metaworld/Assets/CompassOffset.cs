using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;

public class CompassOffset : MonoBehaviour
{

    float heading, headingOffset;

    [SerializeField]
    public UnityEvent<float> headingUpdated = new UnityEvent<float>();

    [SerializeField]
    public UnityEvent<float> headingOffsetUpdated = new UnityEvent<float>();

    bool first = true;

    // Start is called before the first frame update
    void Start()
    {
        Input.compass.enabled = true;
#if UNITY_EDITOR
        OnSessionStateChanged(new ARSessionStateChangedEventArgs(ARSessionState.SessionTracking));
#else
        ARSession.stateChanged += OnSessionStateChanged;
#endif
    }

    void OnSessionStateChanged(ARSessionStateChangedEventArgs args)
    {
        Debug.Log("AR State changed to " + args.state.ToString());
        switch (args.state)
        {
            case ARSessionState.SessionTracking:
                if (first)
                {
                    ApplyCompassOffset();
                    first = false;
                }
                break;
        }
    }

    void ApplyCompassOffset()
    {
        
#if UNITY_EDITOR
        headingOffset = Camera.main.transform.rotation.eulerAngles.y;
#else
        headingOffset = Input.compass.trueHeading;
#endif
        Debug.Log("Applying CompassOffset: "+headingOffset);
        headingOffsetUpdated.Invoke(headingOffset);
        this.transform.rotation = Quaternion.Euler(0, headingOffset, 0);
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        heading = 45f;
#else
        heading = Input.compass.trueHeading;
#endif
        headingUpdated.Invoke(heading);

        //this.transform.localPosition = -Camera.main.transform.localPosition;
    }
}
