using System;
using System.Collections.Generic;
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

		// typeIndex
		private int[] _HpTypeIndex;
		private int archetypeIndex;

		public void Set(ECSManager ecs)
		{
			// 쿼리 생성
			Query_Filter_1 = ecs.Query()
				.WithAny<HPComponent>()
				.WithNone<NeedInit>()
				.Build();
			// 쿼리에 아키타입 업데이트
			Query_Filter_1.UpdateArchetypes(ecs.entityManager);

			// 찾는 컴포넌트의 청크내부 인덱스 저장

			_HpTypeIndex = new int[Query_Filter_1.archetypes.Count];
			archetypeIndex = 0;
			foreach(var archetype in Query_Filter_1.archetypes)
			{
				if (archetype.TypeIndexMap.TryGetValue(ComponentTypeRegister.GetID(typeof(HPComponent)), out int index))
					_HpTypeIndex[archetypeIndex++] = index;
				else
				{
					_HpTypeIndex[archetypeIndex++] = -1;
					throw new InvalidDataException(" didn't find Type in Archetype");
				}
			}
		
		}
		public void OnUpdate(ECSManager ecs)
		{
			int plus = 2;
			archetypeIndex = 0;
			// 쿼리 아키타입 갱신
			Query_Filter_1.UpdateArchetypes(ecs.entityManager);
			// 아키타입 순환
			foreach(var archetype in Query_Filter_1.archetypes)
			{
				// 아키타입내부 컴포넌트 타입에 맞는 청크 순환
				foreach(var chunk in archetype.Chunks)
				{
					Console.WriteLine("archetype" + archetypeIndex);
					var HP = chunk.GetSpan<HPComponent>(_HpTypeIndex[archetypeIndex++]);
					for(int i = 0; i < HP.Length ;i++)
					{
						HP[i].point += plus+=2;
						Console.WriteLine(HP[i].point);
					}
				}
			}
		}
	}
}
