# ECS(Entity-Component-System)
+ Unity,Console 환경에서 사용 할 수 있는 경량 라이브러리 구현 프로젝트
+ OOP 대비 성능 개선을 목표로 설계한 ECS 아키텍처 

# 개발 환경
+ IDE : Visual Studio 2022
+ .Net8.0
+ Unity 6
+ Language : C#
+ Platform : x64
+ Dependencies : None (No external NuGet packages)

# 주요 기능
+ 클래스 기반 객체 구조 대신 컴포넌트 단위로 데이터를 분리하여 관리
+ 동일한 컴포넌트를 가진 엔티티를 아키타입으로 그룹화 하여 효율적으로 처리
+ 아키타입 내부에 컴포넌트를 청크 배열로 저장해, 연속된 메모리 구조를 통해 데이터 접근 성능 향상
+ 조건에 맞는 엔티티를 조회하여 일괄 처리
+ 객체 간 참조 대신 데이터 배열 기반으로 직접 접근하여 메모리 할당 및 GC 부담 감

# 사용방법
## 시작전
1. ECSCore 폴더를 프로젝트 폴더에 삽입한다.
2. C#10 이하 의 경우 GlobalUsing파일을 주석처리한다.
3. 외부에서 ECSCore에 접근하기 위해서 메소드는 다음과 같다



   ECSManager.cs
   + public Entity CreateEntity(ulong typeBitMask)
   + public Entity Init(Entity entity)
   + public ref T Get<T>(Entity entity)
   + public void Remove(Entity entity)
   + public bool Has(Entity entity)
   + internal QueryBuilder Query()
  
   
   ComponentTypeRegister.cs
   + public static int GetID(Type type)
   + public static int GetID<T>()
  
     
   BitMaskRegister.cs
   + public static ulong ToMask(int idx)
   + public static ulong BuildMask(params int[] idx)
  
   
   Archetype.cs
   + public int GetTypeIndex(int typeID)  
## 빠른사용
1. 컴포넌트 등록
2. ECSManager 생성
3. Entity 생성 및 초기화
4. 데이터 접근 및 처리


### Set
1. Component 정의

   
Component폴더의 ComponentGroup.cs 파일 내부에 구조체형식으로 필요한 컴포넌트를 선언한다.
```
internal struct EXComponent : IComponentData
{
	internal float point;
}
```
2. Component 등록

  
ComponentSetting() 내부에 사용할 ComponentTypeRegister.Set<~>();사용해 컴포넌트들을 삽입 한다. NeedInit은 최상단에 고정한다.
```
internal static void SetComponent()
{
    // flag
    ComponentTypeRegister.Set<NeedInit>(); // 무조건 항상 0번째
    ComponentTypeRegister.Set<EXComponent>(); 
    ComponentTypeRegister.IsFrozen = false;
}
```
3. ECSManager 생성
```
ECSManager ecsMG = new ECSManager();
```
### EntityCreate
1. Entity 생성

엔티티생성에 필요한 컴포넌트 타입들을 ComponentGroup.cs에서 저장했던 ID값을 토대로 비트마스크로 변환한다.
```
int EXID = ComponentTypeRegister.GetID<EXComponent>();
ulong EXBitMask = BitMaskRegister.BuildMask(EXID);
``` 
2. 변환한 비트마스크를 사용하여 엔티티를 생성한다.
```
Entity entity = ecsMG.CreateEntity(EXBitMask);
```
3. 생성한 엔티티를 초기화 한다
```
ecsMG.Init(entity);
```
### 데이터 접근(Get)
1. ref를 사용하는 Get<T> 메소드를 사용해 참조할수 있다.
```
ref var EXComp = ref ecsMG.Get<EXComponent>(entity);
EXComp.point = 1f;
```
#### Query 기반 처리
1. Query를 사용해 조건에 맞는 아키타입을 찾을수 있다. 
  +  withAll  (대상 모든 컴포넌트를 포함하는 아키타입)
  +  withAny  (대상 컴포넌트를 하나라도 포함하는 아키타입)
  +  withNone  (대상 컴포넌트를 포함하지 않는 아키타입)
```
EXQuery = ecsMG.Query()
          .withAll<EXComponent>()
```
```
EXQuery = ecsMG.Query()
          .withAny<EXComponent>()
```
```
EXQuery = ecsMG.Query()
          .withNone<EXComponent>()
```
2. Query조건으로 찾은 아키타입의 활용
```
foreach (var archetype in Query_FilterForCaculation.GetArchetype(ecsMG.entityManager))
{
	var EX_IDx = archetype.GetTypeIndex(EXID);
	foreach (var chunk in archetype.Chunks)
	{
		var EX_Span = chunk.GetSpan<EXComponent>(EX_IDx);
		for (int i = 0; i < chunk.ChunkCount; i++)
		{
			EX_Span[i].point = 2f;
		}
	}
}
```

## Entity 관리(Has/Remove)
### Has
1. 엔티티가 현재 존재하는 엔티티인지 확인하는 방법
```
if(ecsMG.Has(entity)){}
```
### Remove
1. 엔티티를 삭제. 삭제시 청크배열 내부에서 엔티티의 위치에는 청크배열 마지막요소가 덮어 씌워진다 , 또한 엔티티의 제네레이션이 증가하며 기존의 엔티티와 구별하게 된다. 
```
ecsMG.Remove(entity);
```

