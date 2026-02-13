using ECSCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using ECSTest;

namespace ECSTest
{
	internal class ComponentTypeRegisterTest
	{
		[Test]
		public void SetTest()
		{

			for (int i = 0; i < ComponentTypeRegister.typeToID.Count; i++)
			{
				Type type = ComponentTypeRegister.GetType(i);
				int typeID = ComponentTypeRegister.Set(typeof(DummyATKComponent));

				Assert.That(typeID, Is.EqualTo(ComponentTypeRegister.GetID(type)));
			}


		}
	}
}
