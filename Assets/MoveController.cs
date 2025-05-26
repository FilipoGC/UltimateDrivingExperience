using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;


namespace UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets
{
    public class MoveController : MonoBehaviour
    {
        public carController CarScript;
        public DynamicMoveProvider MovementScript;
        public Transform PlayerTransform;
        public Vector3 targetPostion;
        //referencia carscript e movementscript, transoform do jogador
        //duas funções: enter car: habilita car script, desabilita movment script, transoform do jogador as filho do car, exit car: oposto do enter car.
        //doorhandle enter, putamerda: exit

        // Start is called before the first frame update
        public void enterCar()
        {
            PlayerTransform.position = targetPostion;
            PlayerTransform.SetParent(CarScript.gameObject.transform, true);
            MovementScript.enabled = false;
            CarScript.enabled = true;


        }
        public void exitCar()
        {


        }
        public void cellinghand()
        {



        }
    }
}