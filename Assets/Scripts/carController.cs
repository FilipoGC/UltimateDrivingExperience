using UnityEngine;
using UnityEngine.InputSystem;

public class carController : MonoBehaviour
{
    
    public WheelCollider[] WheelF;
    public WheelCollider[] WheelR;

    public float steeringMultiplier = 15;

    private float engineRPM = 0;
    [SerializeField]
    private float RPMIncreaseRatio = 30f;
    [SerializeField]
    private float RPMDecreaseRatio = 15f;
    [SerializeField]
    private float rollingResistance = 18;
    [SerializeField]
    private float airResistance = 0.35f;
    [SerializeField]
    private float maxRPM = 8000;
    [SerializeField]
    private float minRPM = 0;
    [SerializeField]
    private float killEngineRPM = 900;
    [SerializeField]
    private float wheelMatchupRate = 10;

    public bool isEngineRunning;

    [SerializeField]
    private float[] gearRatio;
    private int currentGear;
    [SerializeField]
    private float maxTorque = 300f;
    [SerializeField]
    private float driveTrainMultiplier = 3f;

    [SerializeField]
    private AnimationCurve maxThrottleCurve;

    [SerializeField]
    private AnimationCurve minThrottleCurve;

    [SerializeField]
    private AnimationCurve brakeCurve;

    [SerializeField]
    private AnimationCurve clutchCurve;
    

    private float gasPedal;
    private float brakes;
    private float clutchPedal;

    [SerializeField]
    private float brakesForce = 5000f;
    public float steering;

    InputSystem_Actions inputActions;

    public PanelController panel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Enable();

        inputActions.VRDriving.ClutchPedal.performed += context =>
        {
            clutchPedal = clutchCurve.Evaluate(Mathf.Abs(context.ReadValue<Vector2>().x));
        };

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
            ChangeGears(currentGear + 1);
        };
        inputActions.VRDriving.ChangeGearDown.performed += context =>
        {
            ChangeGears(currentGear - 1);
        };

        isEngineRunning = false;
        engineRPM = 0;
        currentGear = 0;

        if(panel == null){
            panel = GetComponent<PanelController>();
            if(panel = null){
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
        for (int i = 0; i < WheelF.Length; i++)
        {
            WheelF[i].steerAngle = steering;
            WheelF[i].brakeTorque = brakeCurve.Evaluate(brakes) * brakesForce;
        }



        //Calculate torque on the wheels
        float engineTorque;
        if(isEngineRunning)
        {
            if((engineRPM - minRPM)/(maxRPM - minRPM) < gasPedal)
            {
                engineTorque = gasPedal * maxThrottleCurve.Evaluate(engineRPM / maxRPM);
            }
            else{
                engineTorque = (gasPedal * maxThrottleCurve.Evaluate(engineRPM / maxRPM)) + ((1-gasPedal) * minThrottleCurve.Evaluate(engineRPM / maxRPM));
            }
        }
        else
        {
            engineTorque = 0;
        }
        
        //Update engine RPM according to the torque and inertia
        engineRPM += (engineTorque * RPMIncreaseRatio) - RPMDecreaseRatio - (rollingResistance * engineRPM * 0.001f);

        //Verify engine status
        if (engineRPM < minRPM)
        {
            engineRPM = minRPM;
        }
        else if (engineRPM < killEngineRPM)
        {
            Debug.Log("Car engine stalling!");
            isEngineRunning = false;
        }
        else if (engineRPM > maxRPM)
        {
            engineRPM = maxRPM;
            Debug.Log("Car engine at dangerous RPM!");
        }

        //Apply said torque on the wheels
        for (int i = 0; i < WheelR.Length; i++)
        {
            WheelR[i].motorTorque = engineTorque * driveTrainMultiplier * maxTorque * gearRatio[currentGear] * (1 - clutchPedal);
        }

        if (currentGear != 0)
        {
            float matchupRPM = GetWheelMatchupRPM();
            //Match the engine RPM according to the rate in which the wheels are turning
            engineRPM = wheelMatchupRate * ((engineRPM * clutchPedal) - (matchupRPM * (1 - clutchPedal)));
        }
        if(panel != null){
            panel.UpdateDisplay(GetWheelKPH(),engineRPM,GetCurrentGear());
        }
    }

    public float GetEngineRPM()
    {
        return engineRPM;
    }
    
    //Calculate at which RPM should the engine be running to perfectly match the wheels' RPM
    public float GetWheelMatchupRPM()
    {
        float currentSpeed = 0;
        for (int i = 0; i < WheelR.Length; i++)
        {
            currentSpeed += WheelR[i].rpm;
        }

        currentSpeed /= WheelR.Length;

        float matchupRPM = currentSpeed * gearRatio[currentGear] * driveTrainMultiplier;
        return matchupRPM;
    }

    public int GetCurrentGear()
    {
        return currentGear;
    }

    //Get the actual rate of rotation on the wheels (used in the future to calculate car velocity)
    public float GetWheelKPH()
    {
        float currentSpeed = 0;
        for (int i = 0; i < WheelF.Length; i++)
        {
            currentSpeed += WheelF[i].rpm;
        }

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

    public void SwitchEngine()
    {
        if (isEngineRunning)
        {
            isEngineRunning = false;
        }
        else
        {
            isEngineRunning = true;
            engineRPM = killEngineRPM;
        }
    }

    public void ChangeGears(int newGear)
    {
        //if(clutchPedal > 0.8)
        //{
            if (newGear >= 0 && newGear < gearRatio.Length)
            {
                currentGear = newGear;
            }
        //}
        
    }
}
