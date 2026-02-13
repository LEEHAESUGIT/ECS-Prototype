using ECSCore;

internal class Program
{
	internal static void Main()
	{
		Entity[] entity = new Entity[1500];

		ECSManager world = new ECSManager();

		for (int i = 0; i < 1500; i+=3)
		{
			entity[i] = world.CreateEntity(typeof(ATKComponent),
								typeof(DEFComponent),
								typeof(HPComponent));
			entity[i+1] = world.CreateEntity(typeof(EXPComponent),
								typeof(DEFComponent),
								typeof(HPComponent));
			entity[i+2] = world.CreateEntity(typeof(ATKComponent),
								typeof(ATKComponent),
								typeof(ATKComponent));
		}
		for(int j = 0 ; j < 1500 ; j++)
		{
			world.Init(entity[j]);
		}
	}


	internal static void View(ECSManager world)
	{
		var record = world.entityManager._entityRecord;
		System.Console.WriteLine("길이 : {0} \n", record.Count);
	}



}