using ECSCore;
using System.Diagnostics;

internal class Program
{
	internal static void Main()
	{
		Entity[] entity = new Entity[1500];

		ECSManager world = new ECSManager();


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