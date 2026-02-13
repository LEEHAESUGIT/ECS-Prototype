// EntityManager  
// 월드에서 존재하며, 실제 생성되고 관리되는 아키타입형식을 관리한다.
// 관리방식은 딕셔너리를 사용해 해시값을 기준으로 아키타입들을 대입. 
// 생성 , 삭제 , 아키타입관리는 이곳에서 한다.
namespace ECSCore
{
	public class EntityManager
	{
		private static int MemoryCapacity = 16384;

		private Dictionary<ulong, Archetype> Archetypes = new();
		internal Dictionary<int, EntityRecord> _entityRecord = new();

		// 엔티티 생성할시 추가되는것.
		// 엔티티
		// 아키타입
		// 엔티티 레코드
		// 엔티티를 내부를 구성한다. 

		private void recycleOrCreateRecord(int entityID, int entityIndex, Archetype resultArchetype, Chunk resultChunk)
		{

			//엔티티아이디를 받아 엔티티레코드 딕셔너리에서 키값으로 검색하여 존재하는지 확인.
			if (_entityRecord.ContainsKey(entityID))
			{
				// 존재한다면 엔티티를 새로운엔티티로 리셋시킨다.
				_entityRecord[entityID].Reset(entityIndex, resultArchetype, resultChunk);
			}
			else
			{
				// 존재하지 않는다면 새로운 엔티티 레코드를 할당한다.
				EntityRecord resultRecord = new EntityRecord(entityIndex, resultArchetype, resultChunk);
				_entityRecord.Add(entityID, resultRecord);
			}
		}
		

		private Archetype getOrCreateArchetype(Type[] componentTypes, int capacity)
		{
			ulong tempHash = Tool.CaculatorHash(componentTypes);

			// 해쉬값 비교 같은 키값이 없을경우 아키타입종류 추가
			// 해쉬값이 같은 경우 아키타입 반환
			if (!Archetypes.TryGetValue(tempHash, out var archetype))
			{
				archetype = new Archetype(componentTypes, capacity);
				Archetypes.Add(tempHash, archetype);
			}
			return archetype;
		}
		// 엔티티 전체 설계
		internal Entity SpawnEntityRecord(int entityID, params Type[] componentTypes)
		{
			// 아키타입 생성
			Archetype resultArchetype = getOrCreateArchetype(componentTypes, MemoryCapacity);
			// 아키타입 내부 엔티티를 할당할 청크 생성 or 호출
			Chunk resultChunk = resultArchetype.RecycleOrCreateChunk();
			// 엔티티레코드 - 아키타입엔티티 인덱스(청크내 컴포넌트 위치)
			int entityIndex = resultChunk.IndexIssuance(MemoryCapacity);
			// 엔티티레코드 생성
			recycleOrCreateRecord(entityID, entityIndex, resultArchetype, resultChunk);
			// 엔티티 생성
			Entity resultEntity = new Entity(entityID, _entityRecord[entityID].Generation);
			// 청크 식별용 int배열이 존재하는데 여기에 엔티티아이디를 추가한다. 순서는 컴포넌트 어레이와 같다
			resultChunk.InEntity(resultEntity.ID);

			return resultEntity;
		}
		internal Entity InitEntityRecord(int entityID, params Type[] componentTypes)
		{
			// 기존의 엔티티레코드 스왑백
			RelocationEntity(entityID);
			// 기존의 미초기화 아키타입에서 NeedInit를 제외한 모든 아키타입을 다시 생성.
			Archetype initArchetype = getOrCreateArchetype(componentTypes, MemoryCapacity);
			// 새로운 아키타입에서 청크에 할당.
			Chunk initChunk = initArchetype.RecycleOrCreateChunk();
			// 새로운 청크에 할당된 인덱스 위치.
			int index = initChunk.IndexIssuance(MemoryCapacity);
			// 기존의 엔티티레코드 리셋
			_entityRecord[entityID].Reset(index, initArchetype, initChunk);
			// 엔티티 생성
			Entity initEntity = new Entity(entityID, _entityRecord[entityID].Generation);
			// 청크 식별용 int배열이 존재하는데 여기에 엔티티아이디를 추가한다. 순서는 컴포넌트 어레이와 같다
			initChunk.InEntity(initEntity.ID);
			return initEntity;
		}


		/// <summary>
		/// 엔티티 삭제시 
		/// 청크의 카피
		/// 엔티티레코드의 제네레이션 증가
		/// </summary>
		/// <param name="entityID"></param>
		internal void RelocationEntity(int entityID)
		{
			EntityRecord record = _entityRecord[entityID];

			int movedID = record.CapturedChunk.CopyFromLastIndex(record.IndexInChunk);
			if (entityID != movedID)
			{
				_entityRecord[movedID].IndexInChunk = record.IndexInChunk;
			}
			// 상위 World 에 있는 Remove에서 제네레이션을 올려준다.
			//record.NextGeneration();
		}

	}
}