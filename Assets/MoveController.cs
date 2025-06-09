using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;


namespace UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets
{
    public class MoveController : MonoBehaviour
    {
        public carController CarScript;
        public carControllerAutomatic Automatic;
        public DynamicMoveProvider MovementScript;
        public Transform PlayerTransform;
        public Vector3 targetPostion;
        public Vector3 targetRotation;

        // Start is called before the first frame update
        public void enterCar()
        {
            
            PlayerTransform.SetParent(CarScript.gameObject.transform, true);
            MovementScript.enabled = false;
            //CarScript.enabled = true;
            Automatic.enabled = true;
            PlayerTransform.position = targetPostion;
            PlayerTransform.rotation = Quaternion.Euler(targetRotation);


        }
        public void exitCar()
        {


        }
        public void cellinghand()
        {



        }
    }
}