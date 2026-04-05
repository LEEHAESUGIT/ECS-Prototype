using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECSCore
{
	internal class QueryBuilder
	{
		private readonly QueryManager QM;
		//internal readonly List<int> _All = new();
		//internal readonly List<int> _None = new();
		//internal readonly List<int> _Any = new();

		internal ulong All_Mask = 0UL;
		internal ulong None_Mask = 0UL ;
		internal ulong Any_Mask = 0UL;



		internal QueryBuilder(QueryManager qm) 
		{
			this.QM = qm;
		}

		internal QueryBuilder WithAll<T>() where T : struct ,IComponentData
		{
			All_Mask |= BitMaskRegister.ToMask(ComponentTypeRegister.GetID<T>());

			return this;
		}
			//_All.Add(ComponentTypeRegister.GetID(typeof(T)));

		internal QueryBuilder WithNone<T>() where T : struct , IComponentData
		{
			None_Mask |= BitMaskRegister.ToMask(ComponentTypeRegister.GetID<T>());
			//_None.Add(ComponentTypeRegister.GetID(typeof(T)));
			return this;
		}

		internal QueryBuilder WithAny<T>() where T : struct,IComponentData
		{
			Any_Mask |= BitMaskRegister.ToMask(ComponentTypeRegister.GetID<T>());
			//_Any.Add(ComponentTypeRegister.GetID(typeof(T)));
			return this;
		}

		internal EntityQuery Build()
		{
			var resultQuery = QM.CreateQuery(All_Mask, None_Mask, Any_Mask);
			QM.AddQuery(resultQuery);
			Reset();
			return resultQuery;
		}	

		public void Reset()
		{
			this.All_Mask = 0UL;
			this.None_Mask = 0UL;
			this.Any_Mask = 0UL;

			//this._All.Clear();
			//this._None.Clear();
			//this._Any.Clear();
		}
	}
}
