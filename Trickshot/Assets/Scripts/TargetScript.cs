using UnityEngine;

public class TargetScript : MonoBehaviour
{
    public TargetData data;

    private bool hasBeenHit = false;

    private Outline outline;

    private void Awake()
    {
        outline = GetComponent<Outline>();

        if (outline == null)
        {
            outline = gameObject.AddComponent<Outline>();
        }

        outline.enabled = true;
    }

    public bool TryHit()
    {
        if (hasBeenHit)
            return false;

        hasBeenHit = true;

        outline.enabled = false;

        return true;
    }
}
