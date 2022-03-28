using UnityEngine;

[CreateAssetMenu(fileName = "LogRotationOption", menuName = "Gameplay/New Log Rotation Option")]
public class RotationOption : ScriptableObject
{
    [SerializeField] private float speed;
    [SerializeField] private AnimationCurve moveCurve;

    public float Speed => speed;
    public AnimationCurve MoveCurve => moveCurve;
}