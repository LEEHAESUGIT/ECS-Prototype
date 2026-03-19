using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECSCore
{ 
	internal class EntityQuery
	{
		internal readonly int[] All;
		internal readonly int[] None;
		internal readonly int[] Any;

		internal bool IsClean = true; 

		internal readonly List<Archetype> archetypes = new List<Archetype>();

		internal EntityQuery(int[] _all , int[] _none , int[] _any)
		{
			this.All = _all;
			this.None = _none;
			this.Any = _any;

			Array.Sort(this.All);
			Array.Sort(this.None);
			Array.Sort(this.Any);

			IsClean = true;
		}
		public void UpdateArchetypes(EntityManager em)
		{
			if (!IsClean) return;

			archetypes.Clear();

			foreach(var archetype in em.GetAllArchetypes())
			{
				if(archetype.IncludeNeedType(this))
				{
					this.archetypes.Add(archetype);
				}
			}

			IsClean = true;
		}
		internal void NeedClean() => IsClean = false;

	}
}
