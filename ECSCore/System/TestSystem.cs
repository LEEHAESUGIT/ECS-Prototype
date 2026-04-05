using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace ECSCore
{
	internal class TestSystem : ISystem
	{
		// 
		EntityQuery Query_Filter_1;
		
		EntityManager entityManager;
		// typeIndex
		private int hpID = ComponentTypeRegister.GetID(typeof(HPComponent));


		public void Set(ECSManager ecs)
		{
			entityManager = ecs.entityManager;

			

			// 쿼리 생성
			Query_Filter_1 = ecs.Query()
				.WithAny<HPComponent>()
				.WithNone<NeedInit>()
				.Build();
		

			//foreach(var archetype in Query_Filter_1.GetArchetype(ecs.entityManager))
			//{
			//	typeindex_HP = archetype.GetTypeIndex(hpID);




			//	if (archetype.TypeIndexMap.TryGetValue(ComponentTypeRegister.GetID(typeof(HPComponent)), out int index))
			//		_HpTypeIndex[archetypeIndex++] = index;
			//	else
			//	{
			//		_HpTypeIndex[archetypeIndex++] = -1;
			//		throw new InvalidDataException(" didn't find Type in Archetype");
			//	}
			//}
		
		}
		public void OnUpdate(ECSManager ecs)
		{
			int plus = 2;
			foreach(var archetype in Query_Filter_1.GetArchetype(entityManager))
			{
				int hpIndex = archetype.TypeIndexArray[hpID];
				if (hpIndex == -1)
					continue;
				foreach(var chunk in archetype.Chunks)
				{
					var hp = chunk.GetSpan<HPComponent>(hpIndex);
					for (int i = 0; i < hp.Length; i++)
					{
						hp[i].point += plus;
					}
					for (int i = 0; i < hp.Length; i++)
					{
						Console.WriteLine($"{hp[i].point}");
					}
				}
			}



			//archetypeIndex = 0;
			// 쿼리 아키타입 갱신
			//Query_Filter_1.UpdateArchetypes(ecs.entityManager);
			// 아키타입 순환
			//foreach(var archetype in Query_Filter_1.archetypes)
			//{
			//	 아키타입내부 컴포넌트 타입에 맞는 청크 순환
			//	foreach(var chunk in archetype.Chunks)
			//	{
			//		Console.WriteLine("archetype" + archetypeIndex);
			//		var HP = chunk.GetSpan<HPComponent>(_HpTypeIndex[archetypeIndex++]);
			//		for(int i = 0; i < HP.Length ;i++)
			//		{
			//			HP[i].point += plus+=2;
			//			Console.WriteLine(HP[i].point);
			//		}
			//	}
			//}
		}
	}
}
