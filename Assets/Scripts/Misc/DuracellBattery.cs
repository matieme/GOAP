using System.Collections;
using UnityEngine;

public class DuracellBattery : MonoBehaviour
{
    public int batteryChargeAmount;

    public int GetBattery()
    {
        BatteryManager.Instance.RemoveBattery(this);
        StartCoroutine(DestroyOnUse());
        return batteryChargeAmount;
    }

    IEnumerator DestroyOnUse()
    {
        yield return new WaitForEndOfFrame();
        Destroy(gameObject);
    }
}
