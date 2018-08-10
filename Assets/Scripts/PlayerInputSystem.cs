using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

public class PlayerInputSystem : ComponentSystem
{
    struct PlayerData
    {
        public readonly int Length;

        public ComponentDataArray<Input> Input;
    }

    [Inject] private PlayerData m_Players;

    protected override void OnUpdate()
    {
        float dt = Time.deltaTime;

        for (int i = 0; i < m_Players.Length; ++i)
        {
            UpdatePlayerInput(i, dt);
        }
    }

    private void UpdatePlayerInput(int i, float dt)
    {
        Input playerInput;

        float x = UnityEngine.Input.GetAxis("Horizontal");
        float y = UnityEngine.Input.GetAxis("Vertical");
        playerInput.leftJoystick = new float2(x, y);

        m_Players.Input[i] = playerInput;
    }
}