using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;


namespace UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets
{
    public class MoveController : MonoBehaviour
    {
        public carController CarScript;
        public carControllerAutomatic Automatic;
        public DynamicMoveProvider MovementScript;
        public XROrigin PlayerOrigin;
        public Transform TargetPositionIn;
        public Transform TargetPositionOut;
        //public Vector3 targetPostion;
        //public Vector3 targetRotation;

        // Start is called before the first frame update
        public void enterCar()
        {
            
            PlayerOrigin.transform.SetParent(CarScript.gameObject.transform, true);
            MovementScript.enabled = false;
            //CarScript.enabled = true;
            Automatic.enabled = true;
            Automatic.Turn(true);
            PlayerOrigin.MoveCameraToWorldLocation(TargetPositionIn.position);
            //PlayerTransform.rotation = Quaternion.Euler(targetRotation);


        }
        public void exitCar()
        {
            PlayerOrigin.transform.SetParent(null, true);
            MovementScript.enabled = true;
            //CarScript.enabled = true;
            Automatic.enabled = false;
            Automatic.Turn(false);
            CarScript.enabled = false;
            CarScript.Turn(false);
            PlayerOrigin.MoveCameraToWorldLocation(TargetPositionOut.position);

        }
        public void cellinghand()
        {
            MovementScript.enabled = true;
            //CarScript.enabled = false;
            Automatic.enabled = false;

        }
        public void cellinghandOut()
        {
            MovementScript.enabled = false;
            //CarScript.enabled = true;
            Automatic.enabled = true;

        }
    }
}