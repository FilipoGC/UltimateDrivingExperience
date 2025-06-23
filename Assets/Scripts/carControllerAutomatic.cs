using UnityEngine;
using UnityEngine.InputSystem;

public class carControllerAutomatic : MonoBehaviour
{
    public WheelCollider[] WheelF;
    public WheelCollider[] WheelR;

    public float steeringMultiplier = 15;

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

    private float engineRPM = 0;
    [SerializeField]
    private float RPMIncreaseRatio = 30f;
    [SerializeField]
    private float RPMDecreaseRatio = 15f;
    [SerializeField]
    private float rollingResistance = 18;


    [SerializeField]
    private float driveTrainMultiplier = 3f;

    [SerializeField]
    private int[] Modes;

    private int currentMode;
    private float currentGear;

    InputSystem_Actions inputActions;
    public PanelController panel;


    public bool isOnline = false;
    public void Turn()
    {
        if (isOnline)
        {
            isOnline = false;
            isEngineRunning = false;
            engineRPM = 0;
            currentGear = 0;
        }
        else
        {
            isOnline = true;
            isEngineRunning = false;
            engineRPM = 0;
            currentGear = 0;
        }
    }
    public void Turn(bool set)
    {
        if (!set)
        {
            isOnline = false;
            isEngineRunning = false;
            engineRPM = 0;
            currentGear = 0;
        }
        else
        {
            isOnline = true;
            isEngineRunning = false;
            engineRPM = 0;
            currentGear = 0;
        }
    }

    private void Start()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Enable();

        inputActions.VRDriving.RightPedal.performed += context =>
        {
            float pedalValue = context.ReadValue<Vector2>().y;

            if (pedalValue > 0)
            {
                gasPedal = pedalValue;
                brakes = 0;
            }
            else if (pedalValue < 0)
            {
                gasPedal = 0;
                brakes = -pedalValue;
            }
            else
            {
                gasPedal = 0;
                brakes = 0;
            }
        };

        inputActions.VRDriving.TurnEngine.performed += context =>
        {
            SwitchEngine();
        };

        inputActions.VRDriving.ChangeGearUp.performed += context =>
        {
            ChangeMode(currentMode + 1);
        };
        inputActions.VRDriving.ChangeGearDown.performed += context =>
        {
            ChangeMode(currentMode - 1);
        };

        isEngineRunning = false; // Auto engine
        if (panel == null)
        {
            panel = GetComponent<PanelController>();
            if (panel == null)
            {
                Debug.Log("Panel not found!!!");
            }
        }
    }
    /*
    void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
    */
    void ChangeMode(int newMode)
    {
        if (newMode >= 0 && newMode < Modes.Length)
        {
            currentMode = newMode;
        }
    }
    public void SwitchEngine()
    {
        if (isEngineRunning)
        {
            isEngineRunning = false;
        }
        else
        {
            isEngineRunning = true;
            engineRPM = 1000f;
        }
    }

    public void UpdateSteering(float newSteer)
    {
        steering = (newSteer - 0.5f) * steeringMultiplier * 2;
    }

    void FixedUpdate()
    {
        if (!isOnline)
        {
            return;
        }
        //Steering
        for (int i = 0; i < WheelF.Length; i++)
        {
            WheelF[i].steerAngle = steering;
            WheelF[i].brakeTorque = brakeCurve.Evaluate(brakes) * brakesForce;
        }



        //Calculate torque on the wheels
        float engineTorque;
        if (isEngineRunning)
        {
            engineTorque = throttleCurve.Evaluate(gasPedal);
        }
        else
        {
            engineTorque = 0;
        }

        //Update engine RPM according to the torque and inertia
        engineRPM += (engineTorque * RPMIncreaseRatio) - RPMDecreaseRatio - (rollingResistance * engineRPM * 0.001f);

        if(engineRPM < 1000f)
        {
            engineRPM = 1000f;
        }

        if (Modes[currentMode] > 0)
        {
            currentGear = maxTorque / (1 + GetWheelKPH());
        }
        else if (Modes[currentMode] < 0) {
            currentGear = maxTorque / ((-1 - GetWheelKPH()) * 2);
        }
        else
        {
            currentGear = 0;
        }


        //Apply said torque on the wheels
        for (int i = 0; i < WheelR.Length; i++)
        {
            WheelR[i].motorTorque = engineTorque * driveTrainMultiplier * maxTorque * currentGear;
            WheelF[i].brakeTorque = brakeCurve.Evaluate(brakes) * brakesForce;
        }

        
        if (panel != null)
        {
            panel.UpdateDisplay(GetWheelKPH(), engineRPM, currentMode);
        }
    }

    public float GetEngineRPM()
    {
        return engineRPM;
    }

    //Get the actual rate of rotation on the wheels (used in the future to calculate car velocity)
    public float GetWheelKPH()
    {
        float currentSpeed = 0;
        for (int i = 0; i < WheelF.Length; i++)
        {
            currentSpeed += WheelF[i].rpm;
        }
        currentSpeed = Mathf.Abs(currentSpeed);
        currentSpeed = currentSpeed / WheelR.Length;

        currentSpeed = currentSpeed * 0.11304f; //* 0.3f * 60f * 2f * 3.14f / 1000f;
        return currentSpeed;
    }

    public float GetCarMPS()
    {
        float currentSpeed = 0;
        for (int i = 0; i < WheelF.Length; i++)
        {
            currentSpeed += WheelF[i].rotationSpeed;
        }
        for (int i = 0; i < WheelR.Length; i++)
        {
            currentSpeed += WheelR[i].rotationSpeed;
        }

        currentSpeed = currentSpeed / (WheelR.Length + WheelF.Length);

        currentSpeed = currentSpeed * 0.00523f;// 2 * pi * radius / 360

        return currentSpeed;
    }

}