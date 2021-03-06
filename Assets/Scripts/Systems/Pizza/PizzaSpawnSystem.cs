﻿using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Transforms2D;
using UnityEngine;

public class PizzaSpawnSystem : ComponentSystem
{
    public struct Data
    {
        public int Length;
        public EntityArray Entities;
        [ReadOnly] public SharedComponentDataArray<PizzaSpawnData> SpawnData;
    }

    [Inject] private Data data;

    protected override void OnUpdate()
    {
        for (int i = 0; i < data.Length; ++i)
        {
            Entity spawnedEntity = data.Entities[i];
            PizzaSpawnData spawnData = data.SpawnData[i];

            PostUpdateCommands.RemoveComponent<PizzaSpawnData>(spawnedEntity);

            PostUpdateCommands.AddSharedComponent(spawnedEntity, spawnData.PizzaGroup);
            PostUpdateCommands.AddComponent(spawnedEntity, spawnData.Position );
            IngredientList ingredientList = new IngredientList { Value = generatePizzaIngredients() };
            PostUpdateCommands.AddSharedComponent(spawnedEntity, ingredientList);

            PostUpdateCommands.AddComponent(spawnedEntity, new Pizza {});
            PostUpdateCommands.AddComponent(spawnedEntity, new Heading2D { Value = new float2(0, -1) });
            PostUpdateCommands.AddComponent(spawnedEntity, default(TransformMatrix) );
            PostUpdateCommands.AddSharedComponent(spawnedEntity, BootStrap.PizzaLook);
        
            PostUpdateCommands.AddComponent(spawnedEntity, getPizzaCost(ingredientList));

            BootStrap.SetPizzaOrderUIIngredients(ingredientList.Value, spawnData.PizzaGroup.PizzaId);
        }
    }

    // A really, really inefficient random list generator.
    private List<int> generatePizzaIngredients  () {
        List<int> pizzaIngredients = new List<int>();
        int numIngredients = Random.Range(0, 5);
        while (pizzaIngredients.Count < numIngredients)
        {
            int randomIngredient = Random.Range(0, BootStrap.IngredientsData.Length);
            if (!pizzaIngredients.Contains(randomIngredient))
            {
                pizzaIngredients.Add(randomIngredient);
            }
        }
        return pizzaIngredients;
    }

    private PizzaCost getPizzaCost(IngredientList ingredientList) {
        return new PizzaCost { OrderCost = ingredientList.Value.Count * 10 };
    }
}