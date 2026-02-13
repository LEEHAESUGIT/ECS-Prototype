using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECSCore;

namespace ECSTest
{
	internal static class Dummy
	{
		// 엔티티더미
		internal static Entity TestEntityCreate(ECSManager TestECSManagerDummy)
		{
			ComponentTypeRegister.Set(typeof(DummyHPComponent));
			ComponentTypeRegister.Set(typeof(DummyATKComponent));
			ComponentTypeRegister.Set(typeof(DummyDEFComponent));
			return TestECSManagerDummy.CreateEntity(typeof(DummyHPComponent),
												typeof(DummyATKComponent),
												typeof(DummyDEFComponent));
		}
		internal static Archetype TestArchetypeCreate(ECSManager TestECSManagerDummy , Entity TestEntityDummy)
		{
			return TestECSManagerDummy.entityManager._entityRecord[TestEntityDummy.ID].CapturedArchetype;
		}
		internal static Chunk TestChunkCreate(ECSManager TestECSManagerDummy , Entity TestEntityDummy)
		{
			return TestECSManagerDummy.entityManager._entityRecord[TestEntityDummy.ID].CapturedChunk;
		}
		
	}

	internal struct DummyHPComponent() : IComponentData
	{
		internal float value;
	}
	internal struct DummyATKComponent() : IComponentData
	{
		internal float value;
	}
	internal struct DummyDEFComponent() : IComponentData
	{
		internal float value;
	}
}
