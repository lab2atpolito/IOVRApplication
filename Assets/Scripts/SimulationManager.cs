using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SimulationManager
{
    public static bool IsHandTrackingEnabled()
    {
        return OVRPlugin.GetHandTrackingEnabled();
    }
}
