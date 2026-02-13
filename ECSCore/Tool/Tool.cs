using System.Runtime.InteropServices;
namespace ECSCore
{
	internal class Tool
	{
		//internal static readonly int TypeID;

		#region Archetype
		internal static ulong CaculatorHash(Type[] types)
		{
			ulong hash = 14695981039346656037UL;

			foreach (var type in types.OrderBy(t => t.Name))
			{
				{
					ulong typehash = (ulong)type.GetHashCode();
					hash ^= typehash;
					hash *= 1099511628211UL;
				}
			}
			return hash;
		}

		internal static int CaculatorCapacityForSize(int memoryCapacity, Type[] types)
		{
			int expectCapacity = 0;
			foreach (var type in types)
			{
				if (!type.IsValueType)
				{
					throw new InvalidOperationException($" Not value type");
				}
				else
				{
					expectCapacity += Marshal.SizeOf(type);
				}
			}
			int size = memoryCapacity / expectCapacity;

			return Math.Max(size, 1);
		}

		internal static int CaculatorChunkIndex(int entitycount, int chunkMaxSize)
		{
			// 청크인덱스 계산
			return entitycount / chunkMaxSize;
		}
		internal static int CaculatorComponentIndex(int entitycount, int chunkMaxSize)
		{
			// 컴포넌트 인덱스 계산
			return entitycount % chunkMaxSize;
		}
		#endregion
	}
}