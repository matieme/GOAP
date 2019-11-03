using GameUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedikitManager : SingletonObject<MedikitManager>
{
    public List<MedikitBox> medikits = new List<MedikitBox>();

    private void Awake()
    {
        foreach (Transform xf in transform)
        {
            var wp = xf.GetComponent<MedikitBox>();
            if (wp != null)
                medikits.Add(wp);
        }
    }

    public void RemoveMediKit(MedikitBox m)
    {
        if (medikits.Contains(m))
        {
            medikits.Remove(m);
        }
    }
}
