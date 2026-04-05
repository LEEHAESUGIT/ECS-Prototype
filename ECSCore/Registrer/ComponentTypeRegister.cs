using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ECSCore
{
	internal static class ComponentTypeRegister
	{
		// Key : Type , Value : TypeID
		internal static readonly Dictionary<Type, int> TypeToID = new();
		// Index : TypeID , value : Info
		internal static readonly List<ComponentTypeInfo> TypeInfos = new();
		private static readonly List<Type> types = new();

		private static int nextID = 0;
		internal static int TypeCount = 0;

		internal static bool IsFrozen = false;


		//internal static readonly Dictionary<ulong, int> ArchetypeNeedMenmory = new();
		//internal static readonly List<int> TypeSizes = new();



		//// 필수이고 자주사용되는 NeedInit 캐싱
		//internal static readonly int NeedInitID = ComponentTypeRegister.GetID(typeof(NeedInit));
		//internal static readonly ulong NeedInitMask = 1LU << NeedInitID;

	 
		internal  static int Set<T>() where T :  unmanaged,  IComponentData
		{
			int size = Marshal.SizeOf<T>();
			return Set(typeof(T),size);
		}

		private  static int Set(Type type , int tpyeSize)
		{
			if (IsFrozen)
				throw new Exception("you can't import Component. Now Running");
			if (type == null)
				throw new ArgumentNullException("TYPE To NULL");
			if (TypeToID.TryGetValue(type, out var ID))
			{
				return ID;
			}
			int id = nextID;
			int size = tpyeSize;
			ulong bitMask = BitMaskRegister.ToMask(id);

			var typeInfo = new ComponentTypeInfo(id, size, bitMask);
			TypeToID.Add(type, id);
			TypeInfos.Add(typeInfo);
			types.Add(type);

			nextID++;
			TypeCount++;


			return id;
		}


		internal static int GetID(Type type)
		{
			if (TypeToID.TryGetValue(type, out var ID))
			{
				return ID;
			}
			throw new InvalidOperationException();
		}
		internal static int GetID<T>() where T : struct, IComponentData => GetID(typeof(T));
		internal static int GetSize(int typeID) => TypeInfos[typeID].Size;
		internal static int GetSize<T>() where T : struct, IComponentData
		{ 
			int id = GetID(typeof(T));
			return TypeInfos[id].Size;
		}
		internal static ulong Getbitmask(int typeID) => TypeInfos[typeID].BitMask;
		internal static ulong Getbitmask<T>() where T : struct, IComponentData
		{
			int id = GetID(typeof(T));
			return TypeInfos[id].BitMask;
		}
		internal static Type GetType(int typeID) => types[typeID];


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
	
	internal struct ComponentTypeInfo
	{
		internal int ID;
		internal int Size;
		internal ulong BitMask;

		internal ComponentTypeInfo(int id, int size, ulong bitmask)
		{
			this.ID = id;
			this.Size = size;
			this.BitMask = bitmask;
		}
	}
}