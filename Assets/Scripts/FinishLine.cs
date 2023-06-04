using System.Collections;
using UnityEngine;

public class FinishLine : Checkpoint
{
    protected override void OnTriggerEnter(Collider other)
    {
        if (passed) return;
        passed = true;
        StartCoroutine(WaitBeforeEnabling());
        
        if (!other.transform.parent.CompareTag("Player")) return;
        GameManager.Instance.CheckForNewLap();
    }
    
    private IEnumerator WaitBeforeEnabling()
    {
        yield return new WaitForSeconds(2.0f);
        passed = false;
    }
}