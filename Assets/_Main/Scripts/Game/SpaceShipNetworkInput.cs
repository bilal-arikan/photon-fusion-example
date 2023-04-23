using Fusion;
using UnityEngine;

public struct SpaceShipNetworkInput : INetworkInput
{
    public float rotation;
    public float acceleration;
    public NetworkBool isShooting;
}