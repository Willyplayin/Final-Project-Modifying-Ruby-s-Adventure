using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    void OnTriggerStay2D(Collider2D other)
    {
        RubyController controller = other.GetComponent<RubyController>();

        if (controller != null)
        {
            StartCoroutine(WaitForSeconds());
            controller.ChangeHealth(-1);
        }
    }

    IEnumerator WaitForSeconds()
    {
        yield return new WaitForSecondsRealtime(2);
    }
}
