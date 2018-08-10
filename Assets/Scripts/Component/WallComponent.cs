using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public struct Wall : ISharedComponentData
{
}

public class WallComponent : SharedComponentDataWrapper<Wall>
{
}
