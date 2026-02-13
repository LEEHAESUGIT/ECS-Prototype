namespace ECSCore
{

	internal class EntityRecord
	{
		// ID 는 엔티티레코드 배열의 인덱스 값으로 대체
		// 대신 제네레이션을 추가해 인덱스레코드의 무결성? 검증
		// 엔티티 인덱스 아이디 는 엔티티 아이디가 아닌 아키타입,청크 배열의 인덱스 값
		//internal int ID { get; private set; }
		internal int Generation { get; private set; } = 0;
		internal int IndexInChunk;
		internal Archetype CapturedArchetype { get; private set; }
		internal Chunk CapturedChunk { get; private set; }
		internal EntityRecord(int archetypeEntityIndex, Archetype archetype, Chunk chunk)
		{
			this.IndexInChunk = archetypeEntityIndex;
			this.CapturedArchetype = archetype;
			this.CapturedChunk = chunk;
		}
		internal bool IsAlive(int generation)
		{
			return this.Generation == generation ? true : false;
		}

		internal void Reset(int index, Archetype archetype, Chunk chunk)
		{
			this.IndexInChunk = index;
			this.CapturedArchetype = archetype;
			this.CapturedChunk = chunk;
		}

		internal void NextGeneration()
		{
			Generation++;
		}
	}

}
