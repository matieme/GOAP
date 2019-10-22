using System.Collections;
using UnityEngine;

public class MedikitBox : MonoBehaviour
{
    public int hpHealed;

    public int GetHealed()
    {
        StartCoroutine(DestroyOnUse());
        return hpHealed;
    }

    IEnumerator DestroyOnUse()
    {
        yield return new WaitForEndOfFrame();
        Destroy(gameObject);
    }
}
