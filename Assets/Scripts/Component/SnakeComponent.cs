using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using System;

[Serializable]
public struct Snake : ISharedComponentData
{
    public string name;
    public float speed;
}

public class SnakeComponent : SharedComponentDataWrapper<Snake>
{
}
