using System;
using System.Collections.Generic;

// Archetype
// 틀 이라는 어원에 맞게 컴포넌트모임을 틀을 잡아 준다. 
// 리스트를 사용해 Chunck배열을 관리한다.
// 간단히 Archetype는 엔티티를 구성하기 위한 몰드의 역활이 되겠다.
// 리스트 배열 Chunks내부에 요소는 각 16kb의 용량제한을 한다.
// 실제 저장상태또한 타입별로 저장하는 동시에 용량제한을 넘길 시 리스트 요소를 증가시켜 Chunck를 증가시켜 저장한다.
namespace LHS.ECS.Core
{
	internal class Archetype
	{
		// Chunk
		internal readonly List<Chunk> Chunks = new();
		// TypeIndex
		internal readonly int[] ComponentTypeOrder;
		internal readonly int[] TypeIndexArray;
		
		// ChunkCount
		internal readonly int ChunkMaxSize;
		// BitMask
		internal readonly ulong typeMask = 0;


		// 아키타입에 포함된 엔티티들
		// 엔티티들이 아키타입에 생성된 순서대로 대입된다.
		// 삭제시 맨뒤의 엔티티가 삭제된 엔티티 위치로 이동.
		// 즉 청크배열의 순서대로 포함된 엔티티들이 정렬된다.
		// 삭제,변경시 항상 리스트의 마지막 요소가 삭제된다. 
		//internal List<Entity> EntityAt = new();

		// 청크인덱스의 범주 0 ~ 청크맥스 사이즈

		// Archetype
		//	ㄴ Chunks []     데이터컨테이너 인덱스
		//		ㄴ Chunk
		//			ㄴ ComponentArray[] 데이터타입 인덱스
		//				ㄴ ComponentArray[][] 컴포넌트 인덱스

		// capacitySize = 16384 == 16kb
		
		internal Archetype(ulong typeMask,int memorySize)
		{
			this.typeMask = typeMask;

			int typeCount = BitMaskRegister.CountTypeInBitMask(typeMask);
			int componentCount = ComponentTypeRegister.TypeCount;

			ComponentTypeOrder = new int[typeCount];
			TypeIndexArray = new int[componentCount];

			Array.Fill(TypeIndexArray , -1);

			int count = 0;
			
			for(int id = 0; id < componentCount; id++)
			{
				if (BitMaskRegister.IsIncludeTypeInBitMask(typeMask, id))
				{
					ComponentTypeOrder[count] = id;
					TypeIndexArray[id] = count;
					count++;
				}
			}

			ChunkMaxSize = Tool.CaculatorCapacityForSize(memorySize, typeMask);
			Chunks.Add(new Chunk(this));
		}

		private Chunk createChunk()
		{
			Chunk resultChunk = new Chunk(this);
			Chunks.Add(resultChunk);
			return resultChunk;
		}

		internal Chunk RecycleOrCreateChunk()
		{
			foreach (var chunk in Chunks)
			{
				if (!chunk.IsMax(ChunkMaxSize))
				{
					return chunk;
				}
			}
			return createChunk();
		}

		internal bool IsNeedInit()
		{
			int needInitID = ComponentTypeRegister.GetID<NeedInit>();
			return BitMaskRegister.IsIncludeTypeInBitMask(typeMask,needInitID);
		}

		internal bool IncludeNeedType(EntityQuery query)
		{
			// 항상 갖공 있는
			if ((typeMask & query.AllMask) != query.AllMask)
				return false;
			// 하나라도 있는 
			if ((query.AnyMask != 0) && ( typeMask & query.AnyMask) == 0)
				return false;
			// 가지고 있으면 안되는
			if ((typeMask & query.NoneMask) !=  0)
				return false;

			return true;
		}


		// For System
		internal int GetTypeIndex(int typeID)
		{
			if (!BitMaskRegister.IsIncludeTypeInBitMask(typeMask, typeID))
				throw new InvalidDataException("didn't have type in Archetype");
			return TypeIndexArray[typeID];
		}
	}


	//// Swap and Delete
	//// 엔티티를 삭제한 후 그 빈자리는 배열의 마지막인덱스 요소를 채워넣는다. 
	//// 마지막 인덱스 요소는 초기화 하지 않아도 상관없다.
	//// 애초에 메모리할당이 되어있는 상태이고, 더 이상 그 요소를 가리키는것이 사라지기 때문이다.
}