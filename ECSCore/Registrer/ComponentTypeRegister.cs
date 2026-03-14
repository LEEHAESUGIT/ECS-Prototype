using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ECSCore
{
	internal static class ComponentTypeRegister
	{
		internal static readonly Dictionary<Type, int> typeToID = new();
		private static readonly List<Type> types = new();
		private static int nextID = 0;

		internal static int Set(Type type)
		{
			if (typeToID.TryGetValue(type, out var ID))
			{
				return ID;
			}
			typeToID[type] = nextID++;
			types.Add(type);	
			return nextID;
		}
		
		internal static int GetID(Type type)
		{
			if (typeToID.TryGetValue(type, out var ID))
			{
				return ID;
			}
			throw new InvalidOperationException();
		}
		internal static int GetID<T>() where T : struct, IComponentData => GetID(typeof(T));
		internal static Type GetType(int typeID) => types[typeID];

		internal static Type[] ReturnTypesfor(int[] typesID)
		{
			Type[] tempTypes = new Type[typesID.Length];
			for (int i = 0; i < typesID.Length; i++)
			{
				tempTypes[i] = GetType(typesID[i]);
			}
			return tempTypes;
		}
		internal static int[] ReturnTypesIDfor(Type[] types)
		{
			int[] tempTypesID = new int[types.Length];
			for (int i = 0; i < tempTypesID.Length; i++)
			{
				tempTypesID[i] = GetID(types[i]);
			}
			return tempTypesID;
		}

	}
}