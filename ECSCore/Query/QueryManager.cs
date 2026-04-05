using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECSCore
{
	internal class QueryManager
	{
		internal readonly EntityManager _EM;
		internal readonly List<EntityQuery> entityQuery = new List<EntityQuery>();

		public QueryManager(EntityManager em)
		{
			this._EM = em;
		}

		public EntityQuery CreateQuery(ulong all = 0UL , ulong none = 0UL, ulong any = 0UL)
		{
			EntityQuery resultQuery = new EntityQuery(all , none , any );
			return resultQuery;
		}
		public void AddQuery(EntityQuery EQ)
		{
			entityQuery.Add(EQ);
		}

	}
}
