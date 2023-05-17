using UnityEngine;

public class FinishLine : Checkpoint
{
    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        GameManager.Instance.CheckForNewLap();
    }
}