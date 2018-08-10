using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Transforms;

public class PlayerMoveSystem : ComponentSystem
{
    public struct Data
    {
        public readonly int Length;
        public ComponentDataArray<Position> Position;
        public ComponentDataArray<Heading> Heading;
        public ComponentDataArray<Input> Input;
    }

    [Inject] private Data m_Data;

    protected override void OnUpdate()
    {
        var settings = SnakeBootstrap.Settings;

        float dt = Time.deltaTime;
        for (int index = 0; index < m_Data.Length; ++index)
        {
            var position = m_Data.Position[index].Value;
            var heading = m_Data.Heading[index].Value;

            var playerInput = m_Data.Input[index];
            float3 moveDirection = new float3(playerInput.leftJoystick.x, 0, playerInput.leftJoystick.y);
            position += dt * moveDirection * settings.playerMoveSpeed;

            m_Data.Position[index] = new Position { Value = position };
            m_Data.Heading[index] = new Heading { Value = heading };
            m_Data.Input[index] = playerInput;
        }
    }
}
