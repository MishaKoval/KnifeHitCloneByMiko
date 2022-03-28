using UnityEngine;
[CreateAssetMenu(fileName = "Apple Spawn Settings", menuName = "Gameplay/New Apple Spawn Settings ")]
public class AppleSpawner : ScriptableObject
{
    [SerializeField,Range(0,100)] private int spawnСhance;

    public int GetChance() => spawnСhance;
}