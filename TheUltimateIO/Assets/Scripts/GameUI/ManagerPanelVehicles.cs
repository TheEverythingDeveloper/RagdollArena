using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameUI
{
    public class ManagerPanelVehicles : MonoBehaviour
    {
        Vehicles vehicleActive;
        public PanelVehicles[] panels;

        public enum Vehicles
        {
            Ram,
            Catapult
        }

        public void PanelOn(Vehicles vehicle)
        {
            panels[(int)vehicle].gameObject.SetActive(true);
            vehicleActive = vehicle;
        }

        public void PanelOff()
        {
            panels[(int)vehicleActive].gameObject.SetActive(false);
        }
    }
}
