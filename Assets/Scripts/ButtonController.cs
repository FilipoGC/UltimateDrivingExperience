using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets
{
    public class ButtonController : MonoBehaviour
    {
        public carController Manual;
        public carControllerAutomatic Automatic;

        // Start is called before the first frame update
        public void PressedButton()
        {
            Manual.enabled = false;
            Manual.Turn(false);
            Automatic.enabled = true;
            Automatic.Turn(true);
        }
        public void RealesedButton()
        {
            Manual.enabled = true;
            Manual.Turn(true);
            Automatic.enabled = false;
            Automatic.Turn(false);

        }
    }
}