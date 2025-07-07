using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class VisualWheels : MonoBehaviour
{
    public Transform leftWheel;
    public Transform rightWheel;
    public XRKnob steeringKnob;

    public float maxWheelRotation = 35f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float knobValue = steeringKnob.value;

        // Convert knob value (0..1) to -1..1 (centered at 0)
        float steeringAmount = (knobValue - 0.5f) * 2f;

        // Steering angle in degrees
        float rotationY = steeringAmount * maxWheelRotation;

        // Apply to left wheel (original Y = 0)
        Vector3 leftEuler = leftWheel.localEulerAngles;
        leftEuler.y = rotationY;
        leftWheel.localEulerAngles = leftEuler;

        // Apply to right wheel (original Y = 180)
        Vector3 rightEuler = rightWheel.localEulerAngles;
        rightEuler.y = 180f + rotationY;
        rightWheel.localEulerAngles = rightEuler;
    }
}
