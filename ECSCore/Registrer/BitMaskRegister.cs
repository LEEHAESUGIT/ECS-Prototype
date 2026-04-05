using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ECSCore
{
	internal static class BitMaskRegister
	{
		internal static ulong ToMask(int idx)
		{
			ulong typeMask = 0;

			typeMask |= (1LU << idx);
			return typeMask;
		}
		internal static ulong BuildMask(params int[] idx)
		{
			ulong typeMask = 0;

			for(int i = 0 ; i < idx.Length ; i++)
				typeMask |= (1LU << idx[i]);
			return typeMask;
		}
		// 비트마스크 내부 상태 갯수
		internal static int CountTypeInBitMask(ulong typeMask) => BitOperations.PopCount(typeMask);

		internal static bool IsIncludeTypeInBitMask(ulong typeMask,int typeID ) => (typeMask & (1LU << typeID)) != 0;

		internal static ulong RemoveType(ulong typeMask, int typeID) => (typeMask & ~(1LU << typeID));

	}
}
