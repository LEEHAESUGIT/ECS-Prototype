using System.Runtime.CompilerServices;

namespace ECSCore
{
	internal class ComponentTypeID
	{
		internal readonly int ID;
		internal ComponentTypeID(int id) => ID = id;
	}

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




	}
}