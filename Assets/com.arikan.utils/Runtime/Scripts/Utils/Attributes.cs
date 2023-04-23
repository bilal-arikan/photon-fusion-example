using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class ScriptOrder : System.Attribute
{
    //Example: [ScriptOrder(-100)]
    public int Order;

    public ScriptOrder(int order)
    {
        this.Order = order;
    }
}

