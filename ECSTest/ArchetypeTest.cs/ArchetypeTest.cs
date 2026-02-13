using ECSCore;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECSTest
{
	internal class ArchetypeTest
	{
		ECSManager DummyECSmanager;
		Archetype DummyArchetype;
		[SetUp]
		public void Setup()
		{
			ComponentTypeRegister.Set(typeof(DummyHPComponent));
			ComponentTypeRegister.Set(typeof(DummyATKComponent));
			ComponentTypeRegister.Set(typeof(DummyDEFComponent));
			DummyECSmanager = new ECSManager();
			Type[] types = { typeof(DummyHPComponent),
							 typeof(DummyATKComponent),
							 typeof(DummyDEFComponent) };

			DummyArchetype = new Archetype(types, 16384);
		}

		[Test]
		public void IsNeedInitTest()
		{
			Type[] types1 = { typeof(DummyHPComponent),
							 typeof(DummyATKComponent),
							 typeof(DummyDEFComponent) };
			// NeedInit 없을때
			DummyArchetype = new Archetype(types1, 16384);
			ClassicAssert.IsFalse(DummyArchetype.IsNeedInit());
			Type[] types2 = { typeof(DummyHPComponent),
							 typeof(DummyATKComponent),
							 typeof(DummyDEFComponent),
								typeof(NeedInit)};
			// NeedInit 있을때
			DummyArchetype = new Archetype(types2, 16384);
			ClassicAssert.IsTrue(DummyArchetype.IsNeedInit());
		}
	}


	

}
