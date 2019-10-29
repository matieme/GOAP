using GameUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryManager : SingletonObject<BatteryManager>
{
    public List<DuracellBattery> batteries = new List<DuracellBattery>();

    private void Awake()
    {
        foreach (Transform xf in transform)
        {
            var wp = xf.GetComponent<DuracellBattery>();
            if (wp != null)
                batteries.Add(wp);
        }
    }

    public void RemoveBattery(DuracellBattery b)
    {
        if(batteries.Contains(b))
        {
            batteries.Remove(b);
        }
    }
}
