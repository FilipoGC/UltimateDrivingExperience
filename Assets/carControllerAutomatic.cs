using UnityEngine;
using UnityEngine.InputSystem;

public class carControllerAutomatic : MonoBehaviour
{
    public WheelCollider[] WheelF;
    public WheelCollider[] WheelR;

    public float steeringMultiplier = 15;
    private float rollingResistance = 18;

    [SerializeField]
    private float maxTorque = 300f;

    [SerializeField]
    private AnimationCurve throttleCurve;
    [SerializeField]
    private AnimationCurve brakeCurve;

    [SerializeField]
    private float brakesForce = 5000f;
    public float steering;
    public bool isEngineRunning;
    private float gasPedal;
    private float brakes;

    InputSystem_Actions inputActions;
    public PanelController panel;

    void Start()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Enable();

        inputActions.VRDriving.RightPedal.performed += context =>
        {
            float pedalValue = context.ReadValue<Vector2>().y;

            gasPedal = Mathf.Clamp01(pedalValue);     // forward = accelerate
            brakes = Mathf.Clamp01(-pedalValue);      // backward = brake
        };

        isEngineRunning = true; // Auto engine
        if (panel == null)
        {
            panel = GetComponent<PanelController>();
            if (panel == null)
            {
                Debug.Log("Panel not found!!!");
            }
        }
    }

    public void UpdateSteering(float newSteer)
    {
        steering = (newSteer - 0.5f) * steeringMultiplier * 2;
    }

    void FixedUpdate()
    {
        //Steering
        foreach (var wheel in WheelF)
        {
            wheel.steerAngle = steering;
            wheel.brakeTorque = brakeCurve.Evaluate(brakes) * brakesForce + (rollingResistance * GetWheelKPH());
        }

        //Torque application
        float torque = throttleCurve.Evaluate(gasPedal) * maxTorque;

        foreach (var wheel in WheelR)
        {
            wheel.motorTorque = torque;
            wheel.brakeTorque = brakeCurve.Evaluate(brakes) * brakesForce + (rollingResistance * GetWheelKPH());
        }

        if (panel != null)
        {
            panel.UpdateDisplay(GetWheelKPH(), 0, 0); // No engine RPM or gears
        }
    }

    public float GetWheelKPH()
    {
        float speed = 0;
        foreach (var wheel in WheelF)
        {
            speed += wheel.rpm;
        }
        speed /= WheelF.Length;
        return speed * 0.11304f;
    }

    public float GetCarMPS()
    {
        float speed = 0;
        foreach (var wheel in WheelF) speed += wheel.rotationSpeed;
        foreach (var wheel in WheelR) speed += wheel.rotationSpeed;
        return (speed / (WheelF.Length + WheelR.Length)) * 0.00523f;
    }
}
