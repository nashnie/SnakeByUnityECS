using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public enum FoodType
{
    Normal = 0,
    Special = 1,//吃了涨两段
}

public struct Food : IComponentData
{
    public FoodType type;
    public float timeToLive;
}