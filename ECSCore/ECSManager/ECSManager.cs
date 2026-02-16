// ************ Internal 선언 변수,함수는 절대 함부로 호출,변경 하면 안된다.**********
// 외부에서는 무조건 public으로 접근할 수 있는 ECSManager의 
// CreateEntity
// Get
// Remove
// Has
// 뿐이다.

// World의 역활 
// EntityManager 관리.
// 아키타입생성에 필요한 타입들을 대입.

// 생성
// 객체가 생성될때 외부에서 UI,prefab 등 무언가 생성되고 그것에 대한 데이터가 필요할때 
// 월드 내부에서는 타입에 맞는 아키타입, 그리고 그것에 맞는 청크를 생성하고 청크배열을 확보한다.
// 모든 확보가 끝난후 엔티티를 리턴해 ID를 배출한다.

// 초기화
// 생성시에는 NeedInit라는 컴포넌트를 추가하여 생성해 초기화가 필요로하는 아키타입생성
// 엔티티리스트로 엔티티들을 보유하여 전체엔티티 관련 상호작용, 특정한 엔티티를 고를수 있다.




// Entity.ID -> EntityRecord[Index]   (엔티티레코드 리스트의 인덱스) 

// EntityRecord.ArchetypeEntityIndex -> 엔티티메니저 안에 있는 아키타입의 컴포넌트들의 인덱스
//	아키타입은 아키타입을 구성하는 타입들로 계산된 해쉬값비교
// EntityRecord.Archetype -> (참조)ref Archetype


using System.Xml;

namespace ECSCore
{
	public class ECSManager
	{
		internal int nextID = 0;
		// 엔티티 관리를 위한 저장
		private Stack<int> freeID = new Stack<int>();
		public EntityManager entityManager = new EntityManager();

		public ECSManager()
		{
			// 컴포넌트 그룹 타입저장
			ConponentSetting.SetComponent();
		}

		#region CreateEntity
		// 처음 생성되는 객체는 기본값으로 NeedInit컴포넌트가 플래그로 들어간다.
		// 초기화와 함께 NeedInit컴포넌트가 없이 초기화 가 된다.
		public Entity CreateEntity(params Type[] componentTypes)
		{
			// ToDo
			// 기능나누기
			//		매개변수에 아무것도 들어오지 않았을때 예외처리
			//		Stack인 freeID에서 재사용할 ID가 있는지 확인후 재발급 or 신규발급
			//		NeedInit컴포넌트 삽입.
			//		발급받은 엔티티ID, NeedInit이 포함된 컴포넌트 타입을 기준으로 엔티티 생성
			// fin
			if(IsTypeDuplication(out int[] sortTypeIDS , componentTypes))
				throw new ArgumentException("ComponentType Dublication ");

			if (componentTypes.Length == 0)
				throw new ArgumentException("Nothing Types");

			int resultID = entityIdIssuance(freeID);
			Type[] resultCombineTypes = insertNeedInit(componentTypes);
			
			return entityManager.SpawnEntityRecord(resultID, resultCombineTypes);
		}

		// Check the stack for recycleID and Issuance
		internal int entityIdIssuance(Stack<int> stackID)
		{
			return stackID.Count > 0 ? stackID.Pop() : nextID++; 
		}

		// expend TypeArray and input NeedInit Component , return combineTypes 
		internal Type[] insertNeedInit(Type[] componentTypes)
		{
			Type[] combineTypes = new Type[componentTypes.Length + 1];
			Array.Copy(componentTypes, combineTypes, componentTypes.Length);
			combineTypes[^1] = typeof(NeedInit);

			return combineTypes;
		}
		#endregion

		#region Init

		// NeedInit 컴포넌트를 포함하는 아키타입에 한해서 값을 대입 후
		// NeedInit 컴포넌트 를 제외한 나머지 컴포넌트를 새로운 아키타입으로 재정의

		// 이 초기화 부분은 실제 값을 대입하는게 아닌 값을 대입하기 위한 상태로 만드는것에 목적이 있다.
		// 실제 값을 초기화 하는것은 Get메소드를 사용하여 직접 대입하게 된다.
		public Entity Init(Entity entity)
		{
			// ToDo
			// 기능나누기
			//		초기화 하려는 엔티티가 현재 존재하는지 확인
			//		NeedInit 컴포넌트가 존재하는지 확인
			//		NeedInit컴포넌트 삭제
			//		후처리된 타입배열로 엔티티 초기화
			if (!Has(entity))
				throw new InvalidOperationException("Not Alive Entity");

			var record = entityManager._entityRecord[entity.ID];

			if (!record.CapturedArchetype.IsNeedInit())
				throw new InvalidOperationException("Entity already initialized");

			Type[] resultTypes = removeNeedInit(record.CapturedArchetype.Types);

			//Type[] types = record.CapturedArchetype.Types;
			//Type[] resultTypes = new Type[types.Length - 1];
			//int typeFlag = ComponentTypeRegister.GetID<NeedInit>();
			//int count = 0;
			//foreach (var type in types)
			//{
			//	if (typeFlag != ComponentTypeRegister.GetID(type))
			//	{
			//		resultTypes[count++] = type;
			//	}
			//}
			return entityManager.InitEntityRecord(entity.ID, resultTypes);
		}
		internal Type[] removeNeedInit(Type[] componentTypes)
		{
			Type[] resultTypes = new Type[componentTypes.Length - 1];
			int typeFlag = ComponentTypeRegister.GetID<NeedInit>();
			int count = 0;
			foreach(var type in componentTypes)
			{
				if (typeFlag != ComponentTypeRegister.GetID(type))
					resultTypes[count++] = type;
			}
			return resultTypes;
		}
		#endregion

		#region Get
		// GetRW
		public ref T Get<T>(Entity entity)
			where T : struct, IComponentData
		{
			if (!Has(entity))
				throw new InvalidOperationException("Not Alive Entity");

			var record = entityManager._entityRecord[entity.ID];

			if (record.CapturedArchetype.IsNeedInit())
				throw new InvalidOperationException("This Entity Is Need Init");


			if(record.CapturedArchetype.TypeIndexMap.TryGetValue(ComponentTypeRegister.GetID(typeof(T)), out int typeIndex))
			{
				return ref record.CapturedChunk.Get<T>(typeIndex,record.IndexInChunk);
			}
			throw new InvalidOperationException("didn't find ComponentType");

			//var typeIndex = record.CapturedArchetype.TypeIndexMap[new ComponentTypeID(ComponentTypeRegister.GetID(typeof(T)))];
			//return ref record.CapturedChunk.Get<T>(typeIndex, record.IndexInChunk);

		}
		#endregion

		#region Remove
		// 엔티티를 받아와 ID로 검색후 엔티티와 관련있는것을 모두 삭제한다.
		// 삭제라는것은 아키타입의 삭제가 아닌 데이터의 삭제. 즉 확보된 메모리는 그대로 두고 청크에 저장된 데이터를 소멸시킨다.
		// 또한 엔티티의 제네레이션을 증가시켜 기존과 구별한다.

		// Remove
		// EntityAt , EntityRecord , Entitiy 등을 정리해야 한다.
		public void Remove(Entity entity)
		{
			if (!Has(entity))
				throw new InvalidOperationException("Not Alive Entity");

			entityManager.RelocationEntity(entity.ID);
			entityManager._entityRecord[entity.ID].NextGeneration();
			this.freeID.Push(entity.ID);

		}

		#endregion

		#region Has
		// Has
		public bool Has(Entity entity)
		{
			if (!entityManager._entityRecord.TryGetValue(entity.ID, out var record))
			{
				return false;
			}
			if (!record.IsAlive(entity.Generation))
			{
				return false;
			}
			return true;
		}
		#endregion
		internal bool IsTypeDuplication(out int[] sortTypes ,params Type[] types)
		{
			//Types = types.OrderBy(t => t.FullName).ToArray();
			HashSet<int> set = new HashSet<int>();

			sortTypes = new int[types.Length];

			int count = 0;
			foreach (Type type in types)
			{
				int id = ComponentTypeRegister.GetID(type);
				sortTypes[count++] = id;
				if(!set.Add(id))
				{
					return true;
				}
			}
			Array.Sort(sortTypes);
 			return false;
		}



	}

}















