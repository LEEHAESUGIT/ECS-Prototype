using ECSCore;
using System.Diagnostics;

internal class Program
{
	internal static void Main()
	{
		Entity[] entity = new Entity[1500];

		ECSManager world = new ECSManager();


		for (int i = 0; i < 50; i++)
		{
			entity[i] = world.CreateEntity(typeof(ATKComponent),
								typeof(DEFComponent),
								typeof(HPComponent));
		}

		for (int j = 0; j < 25; j++)
		{
			world.Init(entity[j]);
		}
		world.Get<ATKComponent>(entity[0]).point = 100f;
		world.Get<ATKComponent>(entity[26]).point = 1000f;
		System.Console.WriteLine("ATKComponent =" + world.Get<ATKComponent>(entity[0]).point);
		System.Console.WriteLine("ATKComponent =" + world.Get<ATKComponent>(entity[26]).point);


	}


	internal static void View(ECSManager world)
	{
		var record = world.entityManager._entityRecord;
		System.Console.WriteLine("길이 : {0} \n", record.Count);
	}



}