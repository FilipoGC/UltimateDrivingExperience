using UnityEngine;
using TMPro;

public class PanelController : MonoBehaviour
{
    public Rigidbody carRigidbody;

    public TextMeshProUGUI speedCounter;
    public TextMeshProUGUI RPMCounter;
    public TextMeshProUGUI gearCounter;

    float[] speedArray;
    float[] RPMArray;
    int arraySize = 100;
    int currentArrayPostion = 0;
    float shownSpeed=0;
    float shownRPM=0;
    int shownGear=0;

    void Start()
    {
        speedArray = new float[arraySize];
        RPMArray = new float[arraySize];
    }

    void Update()
    {
        shownSpeed = 0;
        shownRPM = 0;
        for(int i = 0; i< arraySize; i++){
            shownSpeed += speedArray[i];
            shownRPM += RPMArray[i];
        }
        shownSpeed = shownSpeed/arraySize;
        shownRPM = shownRPM/arraySize;
        
        speedCounter.text = "Speed: " + shownSpeed.ToString();
        RPMCounter.text = "RPM: " + shownRPM.ToString();
        gearCounter.text = "Gear: " + shownGear.ToString();
    }

    public void UpdateDisplay(float currentSpeed, float currentRPM, int currentGear)
    {
        shownGear = currentGear;

        speedArray[currentArrayPostion] = currentSpeed;
        RPMArray[currentArrayPostion] = currentRPM;

        currentArrayPostion++;
        if(currentArrayPostion >= arraySize){
            currentArrayPostion = 0;
        }
    }
}
