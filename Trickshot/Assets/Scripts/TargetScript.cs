using UnityEngine;

public class TargetScript : MonoBehaviour
{
    public TargetData data;

    private bool hasBeenHit = false;

    public bool TryHit()
    {
        if (hasBeenHit)
            return false;

        hasBeenHit = true;
        return true;
    }
}
