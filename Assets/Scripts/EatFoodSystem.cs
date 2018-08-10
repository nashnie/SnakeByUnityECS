using System.ComponentModel.Design;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Transforms;

class EatFoodSystem : JobComponentSystem
{
    struct Snake
    {
        public readonly int Length;
        public ComponentDataArray<Health> Health;
        [ReadOnly] public ComponentDataArray<Input> Input;
        [ReadOnly] public ComponentDataArray<Position> Position;
    }

 Snake m_Players;

    struct Foods
    {
        public readonly int Length;
        public ComponentDataArray<Health> Health;
        public ComponentDataArray<Food> Food;
        [ReadOnly] public ComponentDataArray<Position> Position;
    }

 Foods m_Foods;

    [BurstCompile]
    struct CollisionJob : IJobParallelFor
    {
        public float CollisionRadiusSquared;

        public ComponentDataArray<Health> Health;
        [ReadOnly] public ComponentDataArray<Position> Positions;

        [NativeDisableParallelForRestriction]
        public ComponentDataArray<Food> Foods;
        [NativeDisableParallelForRestriction]
        [ReadOnly] public ComponentDataArray<Position> FoodPositions;

        public void Execute(int index)
        {
            float health = 0.0f;

            float3 snakePosition = Positions[index].Value;

            for (int si = 0; si < Foods.Length; ++si)
            {
                float3 foodPosition = FoodPositions[si].Value;
                float3 delta = foodPosition - snakePosition;
                float distSquared = math.dot(delta, delta);
                if (distSquared <= CollisionRadiusSquared)
                {
                    var food = Foods[si];
                    food.timeToLive = 0.0f;
                    Foods[si] = food;
                    health += 1f;
                }
            }

            var h = Health[index];
            h.Value = h.Value + health;
            Health[index] = h;
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var settings = SnakeBootstrap.Settings;

        if (settings == null)
        {
            return inputDeps;
        }
        if (m_Players.Length <= 0)
        {
            return inputDeps;
        }
        if (m_Foods.Length <= 0)
        {
            return inputDeps;
        }
        var playersVsEnemies = new CollisionJob
        {
            Foods = m_Foods.Food,
            FoodPositions = m_Foods.Position,
            CollisionRadiusSquared = settings.enemyCollisionRadius * settings.enemyCollisionRadius,
            Health = m_Players.Health,
            Positions = m_Players.Position,
        }.Schedule(m_Players.Length, 1, inputDeps);

        return playersVsEnemies;
    }
}