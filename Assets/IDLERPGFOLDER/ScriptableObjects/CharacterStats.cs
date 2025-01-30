using UnityEngine;

[CreateAssetMenu(fileName = "CharacterStats", menuName = "Scriptable Objects/CharacterStats")]
public class CharacterStats : ScriptableObject
{
    public int baseStr;
    public int baseDex;
    public int baseVit;
    public int baseInt;
    public int baseAgi;
}
