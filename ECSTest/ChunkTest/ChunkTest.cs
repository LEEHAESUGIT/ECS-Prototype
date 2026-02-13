using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECSCore;
namespace ECSTest
{
	internal class ChunkTest
	{
		//private Archetype TestArchetype = Dummy.CreateArchetypeDummy_HP_ATK_DEF();
		[Test]
		public void IndexIssuanceTest()
		{
			ECSManager DummyECSmanager = new ECSManager();

			Entity DummyEntity = Dummy.TestEntityCreate(DummyECSmanager);
			Archetype DummyArchetype = Dummy.TestArchetypeCreate(DummyECSmanager, DummyEntity);
			Chunk DummyChunk = Dummy.TestChunkCreate(DummyECSmanager, DummyEntity);
			var resultInt = DummyChunk.IndexIssuance(DummyArchetype.ChunkMaxSize);

			Assert.That(resultInt, Is.InRange(0, DummyArchetype.ChunkMaxSize - 1));
		}
		[Test]
		public void CopyFromLastIndexTest()
		{

		}



	}
}