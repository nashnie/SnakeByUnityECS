using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

class FoodSpawnSystem : ComponentSystem
{
    struct SpawnState
    {
        public readonly int Length;
        public ComponentDataArray<FoodSpawnCooldown> Cooldown;
        public ComponentDataArray<FoodSpawnSystemState> State;
    }

    SpawnState m_State;

    public static void SetupComponentData(EntityManager entityManager)
    {
        var arch = entityManager.CreateArchetype(typeof(FoodSpawnCooldown), typeof(FoodSpawnSystemState));
        var stateEntity = entityManager.CreateEntity(arch);
        var oldState = Random.state;
        Random.InitState(0xaf77);
        entityManager.SetComponentData(stateEntity, new FoodSpawnCooldown { Value = 0.0f });
        entityManager.SetComponentData(stateEntity, new FoodSpawnSystemState
        {
            SpawnedFoodCount = 0,
            RandomState = Random.state
        });
        Random.state = oldState;
    }

    protected override void OnUpdate()
    {
        if (m_State.Length > 0)
        {
            float cooldown = m_State.Cooldown[0].Value;

            cooldown = Mathf.Max(0.0f, m_State.Cooldown[0].Value - Time.deltaTime);
            bool spawn = cooldown <= 0.0f;

            if (spawn)
            {
                cooldown = ComputeCooldown();
            }

            m_State.Cooldown[0] = new FoodSpawnCooldown { Value = cooldown };

            if (spawn)
            {
                SpawnFood();
            }
        }
    }

    void SpawnFood()
    {
        var state = m_State.State[0];
        var oldState = Random.state;
        Random.state = state.RandomState;

        float3 spawnPosition = ComputeSpawnLocation();
        state.SpawnedFoodCount++;

        PostUpdateCommands.CreateEntity(SnakeBootstrap.FoodArchetype);
        PostUpdateCommands.SetComponent(new Position { Value = spawnPosition });
        PostUpdateCommands.SetComponent(new Heading { Value = new float3(0.0f, 0f, -1.0f) });
        PostUpdateCommands.SetComponent(new Health { Value = SnakeBootstrap.Settings.enemyInitialHealth });
        PostUpdateCommands.SetComponent(new Food { type = FoodType.Normal, timeToLive = 15f });

        PostUpdateCommands.AddSharedComponent(SnakeBootstrap.FoodLook);
        state.RandomState = Random.state;

        m_State.State[0] = state;
        Random.state = oldState;
    }

    float ComputeCooldown()
    {
        return 0.5f;
    }

    float3 ComputeSpawnLocation()
    {
        var settings = SnakeBootstrap.Settings;

        float r = Random.value;
        float x0 = settings.playfield.xMin;
        float x1 = settings.playfield.xMax;
        float x = x0 + (x1 - x0) * r;

        r = Random.value;
        float y0 = settings.playfield.yMin;
        float y1 = settings.playfield.yMax;
        float y = y0 + (y1 - y0) * r;

        return new float3(x, 0f, y); 
    }
}