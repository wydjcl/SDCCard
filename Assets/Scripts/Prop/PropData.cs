using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "道具", menuName = "SO/道具")]
public class PropData : ScriptableObject
{
    public string propName;
    public Sprite propSprite;
    public PropType propType;
    public PropRate propRate;
    public int maxStack = 1;
    public List<PropEffect> effects = new List<PropEffect>();
}
//[System.Serializable]
//public class Prop
//{
//    public string propName;
//}
