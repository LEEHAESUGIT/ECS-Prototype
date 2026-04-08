using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LHS.ECS.Core.Query
{ 
	internal class EntityQuery
	{
		internal readonly ulong AllMask;
		internal readonly ulong NoneMask;
		internal readonly ulong AnyMask;

		internal bool IsClean = true; 

		internal readonly List<Archetype> archetypes = new List<Archetype>();

		internal EntityQuery(ulong all_Mask , ulong none_Mask , ulong any_Mask)
		{
			this.AllMask = all_Mask;
			this.NoneMask = none_Mask;
			this.AnyMask = any_Mask;

			IsClean = true;
		}

		internal IReadOnlyList<Archetype> GetArchetype(EntityManager em)
		{
			UpdateArchetypes(em);
			return archetypes;
		}


		private void UpdateArchetypes(EntityManager em)
		{
			if (!IsClean) return;

			archetypes.Clear();

			foreach(var archetype in em.GetAllArchetypes())
			{
				Console.WriteLine($"Archetype Mask: {archetype.typeMask}");
				if (archetype.IncludeNeedType(this))
				{
					this.archetypes.Add(archetype);
				}
			}

			IsClean = true;
		}
		internal void NeedClean() => IsClean = false;

	}
}
