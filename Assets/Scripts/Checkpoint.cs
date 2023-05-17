using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public bool passed;

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!other.transform.parent.CompareTag("Player")) return;
        passed = !passed;
    }
}
