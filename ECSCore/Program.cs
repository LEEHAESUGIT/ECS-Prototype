using ECSCore;
using System.Diagnostics;
using System.IO.Compression;

internal class Program
{
	internal static void Main()
	{
		Entity[] entity = new Entity[1500];

		ECSManager world = new ECSManager();
		EntityQuery Query_Filter_2;
		EntityQuery Query_Filter_3;

		for (int i = 0; i < 100; i+=2)
		{
			entity[i] = world.CreateEntity(typeof(ATKComponent),
								typeof(DEFComponent),
								typeof(HPComponent));
			entity[i+1] = world.CreateEntity(typeof(HPComponent),
								typeof(DEFComponent));
		}

		for (int j = 0; j < 100; j++)
		{
			world.Init(entity[j]);
		}

		Console.WriteLine($"HP ID: {ComponentTypeRegister.GetID<HPComponent>()}");
		Console.WriteLine($"ATK ID: {ComponentTypeRegister.GetID<ATKComponent>()}");
		Console.WriteLine($"DEF ID: {ComponentTypeRegister.GetID<DEFComponent>()}");


		Query_Filter_2 = world.Query()
				.WithAll<HPComponent>()
				.WithAll<ATKComponent>()
				.WithAll<DEFComponent>()
				.WithNone<NeedInit>()
				.Build();

		Console.WriteLine($"Query2 AllMask: {Query_Filter_2.AllMask}");
		Console.WriteLine($"Query2 AnyMask: {Query_Filter_2.AnyMask}");
		Console.WriteLine($"Query2 NoneMask: {Query_Filter_2.NoneMask}");


		foreach (var archetype in Query_Filter_2.GetArchetype(world.entityManager))
		{
			var atkidx = archetype.GetTypeIndex(ComponentTypeRegister.GetID<ATKComponent>());
			var hpidx = archetype.GetTypeIndex(ComponentTypeRegister.GetID<HPComponent>());
			var defidx = archetype.GetTypeIndex(ComponentTypeRegister.GetID<DEFComponent>());
			foreach(var chunk in archetype.Chunks)
			{
				var atkspan = chunk.GetSpan<ATKComponent>(atkidx);
				var hpspan = chunk.GetSpan<HPComponent>(hpidx);
				var defspan = chunk.GetSpan<DEFComponent>(defidx);

				for(int i = 0; i < chunk.ChunkCount; i++)
				{
					atkspan[i].point = 1;
					hpspan[i].point = 2;
					defspan[i].point = 3;
				}
				for (int i = 0; i < chunk.ChunkCount; i++)
				{
					Console.WriteLine($" {atkspan[i].point}, {hpspan[i].point} , {defspan[i].point}");
				}
			}
		}



		Query_Filter_3 = world.Query()
				.WithAll<HPComponent>()
				.WithAll<DEFComponent>()
				.WithNone<ATKComponent>()
				.WithNone<NeedInit>()
				.Build();

		Console.WriteLine($"Query3 AllMask: {Query_Filter_3.AllMask}");
		Console.WriteLine($"Query3 AnyMask: {Query_Filter_3.AnyMask}");
		Console.WriteLine($"Query3 NoneMask: {Query_Filter_3.NoneMask}");

		foreach (var archetype in Query_Filter_3.GetArchetype(world.entityManager))
		{
			var hpidx = archetype.GetTypeIndex(ComponentTypeRegister.GetID<HPComponent>());
			var defidx = archetype.GetTypeIndex(ComponentTypeRegister.GetID<DEFComponent>());
			foreach (var chunk in archetype.Chunks)
			{
				var hpspan = chunk.GetSpan<HPComponent>(hpidx);
				var defspan = chunk.GetSpan<DEFComponent>(defidx);

				for (int i = 0; i < chunk.ChunkCount; i++)
				{
					hpspan[i].point = 2;
					defspan[i].point = 3;
				}
				for (int i = 0; i < chunk.ChunkCount; i++)
				{
					Console.WriteLine($"{hpspan[i].point} , {defspan[i].point}");
				}
			}
		}

		TestSystem test = new TestSystem();
		test.Set(world);
		test.OnUpdate(world);

	}


	internal static void View(ECSManager world)
	{
		var record = world.entityManager._entityRecord;
		System.Console.WriteLine("길이 : {0} \n", record.Count);
	}



}