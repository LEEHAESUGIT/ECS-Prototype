using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECSCore;
using NUnit.Framework.Legacy;

namespace ECSTest
{
	public class ECSManagerTest
	{
		ECSManager DummyECSmanager;

		[SetUp]
		public void Setup()
		{
			ComponentTypeRegister.Set(typeof(DummyHPComponent));
			ComponentTypeRegister.Set(typeof(DummyATKComponent));
			ComponentTypeRegister.Set(typeof(DummyDEFComponent));
			DummyECSmanager = new ECSManager();
		}

		[Test]
		public void CreateEntityTest()
		{
			// 아키타입생성을 위한 타입들을 단 하나라도 집어넣지 않으면 예외처리된다.
			Assert.Throws<ArgumentException>(() => DummyECSmanager.CreateEntity());
			// 엔티티 초기생성용
			var DummyFirstEntity = Dummy.TestEntityCreate(DummyECSmanager);
			// ID
			Assert.That(DummyFirstEntity.ID, Is.EqualTo(0) );
			// Generation
			Assert.That(DummyFirstEntity.Generation, Is.EqualTo(0));

			// 엔티티 재활용
			DummyECSmanager.Remove(DummyFirstEntity);
			var DummyRecycleEntity = Dummy.TestEntityCreate(DummyECSmanager);
			// ID
			Assert.That(DummyRecycleEntity.ID, Is.EqualTo(0));
			// Generation
			Assert.That(DummyRecycleEntity.Generation, Is.EqualTo(1));
		}

		[Test]
		public void entityIdIssuanceTest()
		{
			Stack<int> DummyStack = new Stack<int>();
			// 비어있을때
			int dummyID = DummyECSmanager.entityIdIssuance(DummyStack);
			Assert.That(dummyID , Is.EqualTo(DummyECSmanager.nextID -1));
			// 스택에 요소가 있을때
			int tempStackID = 2;
			DummyStack.Push(tempStackID);
			dummyID = DummyECSmanager.entityIdIssuance(DummyStack);
			Assert.That(dummyID, Is.EqualTo(tempStackID));
			Assert.That(0, Is.EqualTo(DummyStack.Count));
		}

		[Test]
		public void insertNeedInitTest()
		{
			Type[] DummyTypes = {	typeof(DummyATKComponent),
						typeof(DummyHPComponent),
						typeof(DummyDEFComponent)};	
			Type[] insertNeedInitType =  DummyECSmanager.insertNeedInit(DummyTypes);

			//Array.Exists(insertNeedInitType, t => t.Equals(typeof(NeedInit)));

			ClassicAssert.IsTrue(Array.Exists(insertNeedInitType, t => t.Equals(typeof(NeedInit))));
		}
		
		[Test]
		public void InitTest()
		{

			// throw InvalidOperationException
			// 유효하지 않은 엔티티
			Assert.Throws<InvalidOperationException>(() => DummyECSmanager.Init(new Entity(10,10)));
			// throw InvalidOperationException
			// NeedInit을 포함하지 않은 엔티티를 수정하려할때
			Type[] DummyTypes = {   typeof(DummyATKComponent),
						typeof(DummyHPComponent),
						typeof(DummyDEFComponent)};
			Entity includeNeedInitEntity = DummyECSmanager.CreateEntity(DummyTypes);
			DummyECSmanager.Init(includeNeedInitEntity);
			Assert.Throws<InvalidOperationException>(() => DummyECSmanager.Init(includeNeedInitEntity));
		

			//  유효값 테스트
			Type[] vaildTypes = { typeof(DummyHPComponent),
									typeof(DummyATKComponent)};
			Entity vaildEntity = DummyECSmanager.CreateEntity(vaildTypes);
			EntityRecord DummyRecordforTest = DummyECSmanager.entityManager._entityRecord[vaildEntity.ID];
			DummyECSmanager.Init(vaildEntity);

			//Assert.That(vaildTypes ,Is.EqualTo(DummyECSmanager.entityManager._entityRecord[vaildEntity.ID].CapturedArchetype.Types));
			Assert.That(DummyRecordforTest, !Is.EqualTo(DummyECSmanager.entityManager._entityRecord[vaildEntity.ID]));
		
			
		}
		[Test]
		public void removeNeedInitTest()
		{
			Type[] DummyTypes = {	typeof(DummyHPComponent),
									typeof(DummyATKComponent),
									typeof(DummyDEFComponent),
									typeof(NeedInit)};
			Type[] deleteNeedInitType = DummyECSmanager.removeNeedInit(DummyTypes);

			ClassicAssert.IsTrue(!Array.Exists(deleteNeedInitType , t => t.Equals(typeof(NeedInit))));
		}
		[Test]
		public void GetTest()
		{
			Type[] DummyTypes = {  typeof(DummyHPComponent),
									typeof(DummyATKComponent),
									typeof(DummyDEFComponent)};
			Entity vaildEntity =  DummyECSmanager.CreateEntity(DummyTypes);
			vaildEntity = DummyECSmanager.Init(vaildEntity);

			ref DummyHPComponent getComponent = ref DummyECSmanager.Get<DummyHPComponent>(vaildEntity);
			getComponent.value = 200f;
			Assert.That(getComponent.value, Is.EqualTo(200f));
		}
		[Test]
		public void RemoveTest()
		{

		}
		[Test]
		public void HasTest()
		{

		}
	}


}
