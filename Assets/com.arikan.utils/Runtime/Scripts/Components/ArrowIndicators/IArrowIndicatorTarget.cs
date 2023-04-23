using UnityEngine;

namespace Name
{
    public interface IArrowIndicatorTarget
    {
        string name { get; }
        Transform transform { get; }
        GameObject gameObject { get; }
    }
}
