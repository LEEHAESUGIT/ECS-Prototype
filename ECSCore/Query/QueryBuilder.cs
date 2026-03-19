using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECSCore
{
	internal class QueryBuilder
	{
		private readonly QueryManager _QM;
		internal readonly List<int> _All = new();
		internal readonly List<int> _None = new();
		internal readonly List<int> _Any = new();

		internal QueryBuilder(QueryManager qm) 
		{
			this._QM = qm;
		}

		internal QueryBuilder WithAll<T>() where T : IComponentData
		{
			_All.Add(ComponentTypeRegister.GetID(typeof(T)));
			return this;
		}

		internal QueryBuilder WithNone<T>() where T : IComponentData
		{
			_None.Add(ComponentTypeRegister.GetID(typeof(T)));
			return this;
		}

		internal QueryBuilder WithAny<T>() where T : IComponentData
		{
			_Any.Add(ComponentTypeRegister.GetID(typeof(T)));
			return this;
		}

		internal EntityQuery Build()
		{
			var resultQuery = _QM.CreateQuery(_All.ToArray(),_None.ToArray(),_Any.ToArray());
			Reset();
			return resultQuery;
		}	

		public void Reset()
		{
			this._All.Clear();
			this._None.Clear();
			this._Any.Clear();
		}
	}
}
