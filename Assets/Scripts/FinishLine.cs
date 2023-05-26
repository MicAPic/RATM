using System.Collections;
using UnityEngine;

public class FinishLine : Checkpoint
{
    protected override void OnTriggerEnter(Collider other)
    {
        if (passed) return;
        base.OnTriggerEnter(other);
        GameManager.Instance.CheckForNewLap();
        StartCoroutine(WaitBeforeEnabling());
    }
    
    private IEnumerator WaitBeforeEnabling()
    {
        yield return new WaitForSeconds(2.0f);
        passed = false;
    }
}