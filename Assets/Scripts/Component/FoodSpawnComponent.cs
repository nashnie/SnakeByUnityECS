using UnityEngine;
using Unity.Entities;

public struct FoodSpawnCooldown : IComponentData
{
    public float Value;
}

public struct FoodSpawnSystemState : IComponentData
{
    public int SpawnedFoodCount;
    public Random.State RandomState;
}