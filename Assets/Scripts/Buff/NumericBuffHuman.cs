using UnityEngine;

[CreateAssetMenu(fileName = "NumericBuffHuman", menuName = "Scriptable Objects/NumericBuffHuman")]
public class NumericBuffHuman : ScriptableObject
{
    enum NumericBuffHumanType
    {
        MaxHealth, Regen, Dodge,Repopulation
    }
}
