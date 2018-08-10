using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(FoodSpawnSystem))]
public class FoodDestroySystem : ComponentSystem
{
    public struct Data
    {
        public readonly int Length;
        public EntityArray Entities;
        public ComponentDataArray<Food> Food;
    }

    [Inject] private Data m_Data;

    private struct PlayerCheck
    {
        public readonly int Length;
        [ReadOnly] public ComponentDataArray<Input> PlayerInput;
    }

    [Inject] private PlayerCheck m_PlayerCheck;

    protected override void OnUpdate()
    {
        bool playerDead = m_PlayerCheck.Length == 0;
        float dt = Time.deltaTime;

        for (int i = 0; i < m_Data.Length; ++i)
        {
            Food food = m_Data.Food[i];
            food.timeToLive -= dt;
            if (food.timeToLive <= 0.0f || playerDead)
            {
                PostUpdateCommands.DestroyEntity(m_Data.Entities[i]);
            }
            m_Data.Food[i] = food;
        }
    }
}