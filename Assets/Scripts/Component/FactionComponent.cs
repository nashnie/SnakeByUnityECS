using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public struct Faction : ISharedComponentData
{
    public FactionType type;
}

public enum FactionType
{
    Player = 1,
    Enemy
}
