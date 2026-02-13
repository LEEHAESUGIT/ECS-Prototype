// Chunck 
// 실제 컴포넌트의 모임, 컴포넌트들이 담기는장소. 딕셔너리 사용
// 각 청크는 16kb 의 용량을 기준으로 관리된다. 16384byte
// 
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ECSTest")]
namespace ECSCore
{
	internal class Chunk
	{

		internal Array[] ComponentArray;
		internal int ChunkCount = 0;
		internal int[] EntityIDs;

		internal Chunk(Archetype archetype)
		{
			ComponentArray = new Array[archetype.Types.Length];
			EntityIDs = new int[archetype.ChunkMaxSize];
			for (int i = 0; i < ComponentArray.Length; i++)
			{
				ComponentArray[i] = Array.CreateInstance(archetype.Types[i], archetype.ChunkMaxSize);
			}
		}
		internal ref T Get<T>(int typeIndex, int entityIndexID)
			where T : struct, IComponentData
		{
			return ref ((T[])ComponentArray[typeIndex])[entityIndexID];
		}
		// 인덱스 발급
		// true: 청크가 꽉참, false : 청크가 비어있음
		internal int IndexIssuance(int memorySize)
		{
			return IsMax(memorySize) ? 0 : ChunkCount++;
		}
		internal bool IsMax(int memorySize)
		{
			if (ChunkCount >= memorySize)
			{
				return true;
			}
			return false;
		}

		// from : ChunkIndex , to : Chunk LastIndex
		internal int CopyFromLastIndex(int targetIndex)
		{
			int lastIndex = ChunkCount - 1;
			for (int typeIndex = 0; typeIndex < ComponentArray.Length; typeIndex++)
			{
				Array.Copy(ComponentArray[typeIndex],
							targetIndex,
							ComponentArray[typeIndex],
							lastIndex,
							1);
				EntityIDs[targetIndex] = EntityIDs[lastIndex];
			}
			ChunkCount--;
			return EntityIDs[targetIndex];
		}
		internal void InEntity(int id)
		{
			EntityIDs[ChunkCount - 1] = id;
		}

	}

}