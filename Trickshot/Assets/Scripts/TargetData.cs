using UnityEngine;

public enum TargetCategory
{
    Single,
    Multiple
}

public enum TargetObjectType
{
    solid,
    glass
}

[CreateAssetMenu(fileName = "New Target", menuName = "Game/Target Data")]
public class TargetData : ScriptableObject
{
    public string targetName;

    [Header("Target Classification")]
    public TargetCategory category;
    public TargetObjectType objectType;

    [Header("Scoring")]
    public int points;

    [Header("Visual Effects")]
    public GameObject hitEffect;
}