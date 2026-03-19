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

		public EntityQuery CreateQuery(int[]? _all = null , int[]? _none = null , int[]? _any = null)
		{
			
			EntityQuery resultQuery = new EntityQuery(	_all ?? Array.Empty<int>(), 
														_none ?? Array.Empty<int>() , 
														_any ?? Array.Empty<int>());
			entityQuery.Add(resultQuery);
			return resultQuery;
		}


	}
}
