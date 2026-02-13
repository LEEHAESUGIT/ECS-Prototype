// Archetype
// 틀 이라는 어원에 맞게 컴포넌트모임을 틀을 잡아 준다. 
// 리스트를 사용해 Chunck배열을 관리한다.
// 간단히 Archetype는 엔티티를 구성하기 위한 몰드의 역활이 되겠다.
// 리스트 배열 Chunks내부에 요소는 각 16kb의 용량제한을 한다.
// 실제 저장상태또한 타입별로 저장하는 동시에 용량제한을 넘길 시 리스트 요소를 증가시켜 Chunck를 증가시켜 저장한다.
namespace ECSCore
{
	internal class Archetype
	{

		internal readonly List<Chunk> Chunks = new();

		// 불변
		internal readonly Dictionary<ComponentTypeID, int> TypeIndexMap = new();
		internal readonly Type[] Types;
		internal readonly int ChunkMaxSize;

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
		internal Archetype(Type[] types, int memorySize)
		{
			Types = types.OrderBy(t => t.FullName).ToArray();
			ChunkMaxSize = Tool.CaculatorCapacityForSize(memorySize, types);
			Chunks.Add(new Chunk(this));

			for (int i = 0; i < Types.Length; i++)
			{
				TypeIndexMap.Add(new ComponentTypeID(ComponentTypeRegister.GetID(Types[i])), i);
			}
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
			foreach(var a in TypeIndexMap)
			{
				if(a.Key.ID == ComponentTypeRegister.GetID(typeof(NeedInit)))
				{
					return true;
				}
			}
			return false;
		}

		// 엔티티를 집어넣어 아키타입에서 현재 포함된 엔티티를 알수 있고,
		// 리턴으로 컴포넌트 인덱스를 엔티티 레코드에 반환한다.

		// 청크s 의 내부를 돌아 빈공간에 청크를 확인한다.

	}


	//// Swap and Delete
	//// 엔티티를 삭제한 후 그 빈자리는 배열의 마지막인덱스 요소를 채워넣는다. 
	//// 마지막 인덱스 요소는 초기화 하지 않아도 상관없다.
	//// 애초에 메모리할당이 되어있는 상태이고, 더 이상 그 요소를 가리키는것이 사라지기 때문이다.
}