
using LHS.ECS.Core;
using System;
using System.Collections.Generic;


namespace LHS.ECS.Core
{

	internal class ArchetypeAccessCache<Tcache>
	{
		private readonly Dictionary<Archetype, Tcache> cacheMap = new();

		private readonly Func<Archetype, Tcache> creator;

		public ArchetypeAccessCache(Func<Archetype, Tcache> creator)
		{
			this.creator = creator;
		}

		public Tcache Get(Archetype archetype)
		{
			if (!cacheMap.TryGetValue(archetype, out var cache))
			{
				cache = creator(archetype);
				cacheMap.Add(archetype, cache);
			}

			return cache;
		}

	}
}