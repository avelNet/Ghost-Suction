using UnityEngine;

[CreateAssetMenu(fileName = "VacuumData", menuName = "Scriptable Objects/VacuumData")]
public class VacuumData : ScriptableObject
{
    [Header("Physics")]
    public float suctionForce = 15f;
    public float suctionRange = 10f;
    [Range(0, 90)] public float coanAngle = 45f;
    public float stopDistance = 0.7f;

    [Header("Stats")]
    public float tankSize = 100f;

    [Header("Visual")]
    public Color glowColor = Color.white;
}
