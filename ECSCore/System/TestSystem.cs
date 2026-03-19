using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace ECSCore
{
	internal class TestSystem : ISystem
	{
		EntityQuery EQ;
		public void Set(ECSManager ecs)
		{
			EQ = ecs.Query()
				.WithAny<HPComponent>()
				.Build();
		}
		public void OnUpdate(ECSManager ecs)
		{
			EQ.UpdateArchetypes(ecs.entityManager);
			foreach(var archetype in EQ.archetypes)
			{
				foreach(var chunk in archetype.Chunks)
				{
					archetype.TypeIndexMap.TryGetValue(ComponentTypeRegister.GetID(typeof(HPComponent)),out int index);
					var HP = chunk.ComponentArray[index];
					int a = 1;
					for(int i =0; i < chunk.ChunkCount; i++)
					{

						chunk.Get<HPComponent>(index, i).point += a++;
						Console.WriteLine(chunk.Get<HPComponent>(index, i).point);
					}
				}
			}
		}
	}
}
