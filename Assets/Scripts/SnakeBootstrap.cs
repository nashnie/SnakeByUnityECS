﻿using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine.SceneManagement;

public sealed class SnakeBootstrap
{
    public static EntityArchetype PlayerArchetype;
    public static EntityArchetype FoodArchetype;

    public static MeshInstanceRenderer PlayerLook;
    public static MeshInstanceRenderer FoodLook;

    public static SnakeSettings Settings;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
        // This method creates archetypes for entities we will spawn frequently in this game.
        // Archetypes are optional but can speed up entity spawning substantially.

        var entityManager = World.Active.GetOrCreateManager<EntityManager>();

        // Create player archetype
        PlayerArchetype = entityManager.CreateArchetype(
            typeof(Position), typeof(Heading), typeof(Input),
            typeof(Health), typeof(TransformMatrix));

        // Create Food archetype
        FoodArchetype = entityManager.CreateArchetype(
            typeof(Position), typeof(Heading), typeof(Food), 
            typeof(Health), typeof(TransformMatrix));
    }

    // Begin a new game.
    public static void NewGame()
    {
        // Access the ECS entity manager
        var entityManager = World.Active.GetOrCreateManager<EntityManager>();

        // Create an entity based on the player archetype. It will get default-constructed
        // defaults for all the component types we listed.
        Entity player = entityManager.CreateEntity(PlayerArchetype);

        // We can tweak a few components to make more sense like this.
        entityManager.SetComponentData(player, new Position { Value = new float3(0.0f, 0.0f, 0.0f) });
        entityManager.SetComponentData(player, new Heading { Value = new float3(0.0f, 1.0f, 0.0f) });
        entityManager.SetComponentData(player, new Health { Value = Settings.playerInitialHealth });

        // Finally we add a shared component which dictates the rendered look
        entityManager.AddSharedComponentData(player, PlayerLook);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void InitializeAfterSceneLoad()
    {
        var settingsGO = GameObject.Find("Settings");
        if (settingsGO == null)
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            return;
        }

        InitializeWithScene();
    }

    public static void InitializeWithScene()
    {
        var settingsGO = GameObject.Find("Settings");
        if (settingsGO == null)
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            return;
        }
        Settings = settingsGO?.GetComponent<SnakeSettings>();
        if (!Settings)
            return;

        PlayerLook = GetLookFromPrototype("Snake");
        FoodLook = GetLookFromPrototype("Food");

        FoodSpawnSystem.SetupComponentData(World.Active.GetOrCreateManager<EntityManager>());
        World.Active.GetOrCreateManager<UpdatePlayerHUD>().SetupGameObjects();
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        InitializeWithScene();
    }

    private static MeshInstanceRenderer GetLookFromPrototype(string protoName)
    {
        var proto = GameObject.Find(protoName);
        var result = proto.GetComponent<MeshInstanceRendererComponent>().Value;
        Object.Destroy(proto);
        return result;
    }
}