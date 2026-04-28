# ECS(Entity-Component-System)
+ ECS 아키텍처를 직접 구현하고, OOP 대비 성능 차이를 분석한 프로젝트
+ Unity,Console 환경에서 사용 할 수 있는 경량 라이브러리 구현
+ OOP 대비 성능 개선을 목표로 설계한 ECS 아키텍처 

## 상태 : 기초완성(진행중)

# 개발 환경
+ IDE : Visual Studio 2022
+ .Net8.0
+ Unity 6.3 LTS (6000.3.9f1)
+ Language : C#
+ Platform : x64
+ Dependencies : None (No external NuGet packages)

# 주요 기능
+ 클래스 기반 객체 구조 대신 컴포넌트 단위로 데이터를 분리하여 관리
+ 동일한 컴포넌트를 가진 엔티티를 아키타입으로 그룹화 하여 효율적으로 처리
+ 아키타입 내부에 컴포넌트를 청크 배열로 저장해, 연속된 메모리 구조를 통해 데이터 접근 성능 향상
+ 조건에 맞는 엔티티를 조회하여 일괄 처리
+ 컴포넌트 조합 기반 Query를 아키타입 단위로 필터링 하여 청크 단위 순회를 통해 효율적으로 처리
+ 객체 간 참조 대신 데이터 배열 기반으로 직접 접근하여 메모리 할당 및 GC 부담 감소

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
### Entity 관리(Has/Remove)
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
### System


Query를 통해 EXComponent를 가진 엔티티를 조회하고, Archetype과 Chunk 단위로 순회하여 데이터를 직접 수정한다.
```
public static void EXSystem(ECSManager ecs)
{
    var EXID = ComponentTypeRegister.GetID<EXComponent>();

    var query = ecs.Query()
                   .withAll<EXComponent>();

    foreach (var archetype in query.GetArchetype(ecs.entityManager))
    {
        var EX_IDx = archetype.GetTypeIndex(EXID);

        foreach (var chunk in archetype.Chunks)
        {
            var span = chunk.GetSpan<EXComponent>(EX_IDx);

            for (int i = 0; i < chunk.ChunkCount; i++)
            {
                span[i].point += 1f;
            }
        }
    }
}
```
# Architecture
+ Entity : ID 기반 식별자
+ Component : 순수 데이터 구조
+ System : 특정 컴포넌트 조합을 가진 엔티티를 대상으로 수행하는 처리 단위
+ Archetype : 동일한 컴포넌트 조합을 가진 엔티티 그룹
+ Chunk : 연속된 메모리 블록
+ EntityRecord : 존재하는 모든 엔티티의 상태와 위치를 추적하는 레코드

# 구성
```
ECSProject/
├──── ECSCore/
│ ├──── Archetype/
│ │	└──── Archetype.cs
│ ├──── Chunk/
│ │	└──── Chunk.cs
│ ├──── Component/
│ │	└──── ComponentGroup.cs
│ ├──── ECSManager/
│ │	└──── ECSManager.cs
│ ├──── Entity/
│ │ ├──── Entity.cs
│ │ ├──── EntityManager.cs
│ │ └──── EntityRecord.cs
│ ├──── GlobalUsing/
│ │	└──── GlobalUsing.cs
│ ├──── Interface/
│ │ ├──── IComponentData.cs
│ │ ├──── IEntity.cs
│ │ └──── ISystem.cs
│ ├──── Query/
│ │ ├──── EntityQuery.cs
│ │ ├──── QueryBuilder.cs
│ │ └──── QueryManager.cs
│ ├──── Register/
│ │ ├──── BitMaskRegister.cs
│ │ └──── ComponentTypeRegister.cs
│ ├──── System/
│ ├──── Tool/
│ │ └──── Tool.cs
└─────── README.md
```
# 성능차이(by OOP)
=====================ECSTest_Init_AVG ===================================
 Sequential     : 61.54ms | Allocated_Memory : 19430599.00 | TimeToEntityCreate : 0.00  | MemoryToEntityCreate : 194.00 | GC0 : 0.9 , GC1 : 0.9 , GC2 : 0.9
 Conditonal     : 48.93ms | Allocated_Memory : 19431413.00 | TimeToEntityCreate : 0.00  | MemoryToEntityCreate : 194.00 | GC0 : 0.9 , GC1 : 0.9 , GC2 : 0.9
 Random         : 60.71ms | Allocated_Memory : 19480834.00 | TimeToEntityCreate : 0.00  | MemoryToEntityCreate : 194.00 | GC0 : 0.9 , GC1 : 0.9 , GC2 : 0.9
 MultiComponent : 50.99ms | Allocated_Memory : 19430695.00 | TimeToEntityCreate : 0.00  | MemoryToEntityCreate : 194.00 | GC0 : 0.9 , GC1 : 0.9 , GC2 : 0.9
 Caculation     : 54.81ms | Allocated_Memory : 19430248.00 | TimeToEntityCreate : 0.00  | MemoryToEntityCreate : 194.00 | GC0 : 0.9 , GC1 : 0.9 , GC2 : 0.9
=====================ECSTest_Run_AVG===================================
 Sequential     : 5.43ms | Allocated_Memory : 8000.00 | AVG_Process_Time : 0.00  | SecForProcess_Time : 1914059288.03 | GC0 : 0.0 , GC1 : 0.0 , GC2 : 0.0
 Conditonal     : 12.59ms | Allocated_Memory : 8000.00 | AVG_Process_Time : 0.00  | SecForProcess_Time : 794950947.05 | GC0 : 0.0 , GC1 : 0.0 , GC2 : 0.0
 Random         : 19.38ms | Allocated_Memory : 8000.00 | AVG_Process_Time : 0.00  | SecForProcess_Time : 523983629.24 | GC0 : 0.0 , GC1 : 0.0 , GC2 : 0.0
 MultiComponent : 9.24ms | Allocated_Memory : 8000.00 | AVG_Process_Time : 0.00  | SecForProcess_Time : 1086286696.41 | GC0 : 0.0 , GC1 : 0.0 , GC2 : 0.0
 Caculation     : 16.80ms | Allocated_Memory : 8000.00 | AVG_Process_Time : 0.00  | SecForProcess_Time : 602311615.03 | GC0 : 0.0 , GC1 : 0.0 , GC2 : 0.0

 =====================OOPTest_Init_AVG ===================================
 Sequential     : 3.62ms | Allocated_Memory : 5600024.00 | TimeToEntityCreate : 0.00  | MemoryToEntityCreate : 56.00 | GC0 : 0.0 , GC1 : 0.0 , GC2 : 0.0
 Conditonal     : 3.39ms | Allocated_Memory : 5600024.00 | TimeToEntityCreate : 0.00  | MemoryToEntityCreate : 56.00 | GC0 : 0.0 , GC1 : 0.0 , GC2 : 0.0
 Random         : 4.80ms | Allocated_Memory : 6000048.00 | TimeToEntityCreate : 0.00  | MemoryToEntityCreate : 60.00 | GC0 : 0.0 , GC1 : 0.0 , GC2 : 0.0
 MultiComponent : 4.11ms | Allocated_Memory : 5600024.00 | TimeToEntityCreate : 0.00  | MemoryToEntityCreate : 56.00 | GC0 : 0.0 , GC1 : 0.0 , GC2 : 0.0
 Caculation     : 3.94ms | Allocated_Memory : 5600024.00 | TimeToEntityCreate : 0.00  | MemoryToEntityCreate : 56.00 | GC0 : 0.0 , GC1 : 0.0 , GC2 : 0.0
=====================OOPTest_Run_AVG===================================
 Sequential     : 28.46ms | Allocated_Memory : 0.00 | AVG_Process_Time : 0.00  | SecForProcess_Time : 356812942.66 | GC0 : 0.0 , GC1 : 0.0 , GC2 : 0.0
 Conditonal     : 29.44ms | Allocated_Memory : 0.00 | AVG_Process_Time : 0.00  | SecForProcess_Time : 342840182.65 | GC0 : 0.0 , GC1 : 0.0 , GC2 : 0.0
 Random         : 151.65ms | Allocated_Memory : 0.00 | AVG_Process_Time : 0.02  | SecForProcess_Time : 65952917.79 | GC0 : 0.0 , GC1 : 0.0 , GC2 : 0.0
 MultiComponent : 79.81ms | Allocated_Memory : 0.00 | AVG_Process_Time : 0.01  | SecForProcess_Time : 125502383.18 | GC0 : 0.0 , GC1 : 0.0 , GC2 : 0.0
 Caculation     : 97.69ms | Allocated_Memory : 0.00 | AVG_Process_Time : 0.01  | SecForProcess_Time : 102407043.39 | GC0 : 0.0 , GC1 : 0.0 , GC2 : 0.0

# Why ECS?
기존 OOP 구조에서 대량의 객체를 생성, 관리 하는 과정에서 발생하는 성능문제를 주제로 친구와 논의하다 흥미를 갖게 되었다. 
프로그래밍을 공부하기 시작했을때부터 생각했던 class, struct 를 어떻게 나눠야 잘 구분되게 나뉘어 질까? 고민하다. 현실에 존재하는 모든것의 최소단위부터 시작하는게 어떨까 라는 생각을 했었다. 
하지만 현실적으로 객체를 설계 할때는 그 객체에 필요한 부분만 있어야 했고, 최소단위부터 시작하기에는 너무 과한 설계 방식이었다. 그러던 중 친구와 대량의 객체를 관리하는 방법에 논의하게 되었고,
그러다 DDD, ECS등의 개념을 알게되었다. 둘을 동일선상에 놓는건 잘못되긴 했다. DDD는 도메인 설계 방식이고, ECS는 데이터 처리 구조 이기 때문이다. 하지만 둘 다 "구조를 명확하게 나눈다"는 점에서 흥미로운 접근이었다.
더군다나 ECS는 성능까지 더 좋아질 수 있고, 데이터 관리하는데 있어서 일관적이고 효율적이다? 매력적이었다.
그래서 ECS를 좀 더 알아보고 싶었고, OOP랑 비교해서 성능차이를 직접 눈으로 보고싶었다.

# 그래서?
일단 구현해보면서 가장 어려웠던 부분은 데이터 관리 방식이 기존 OOP와 너무 다른것이었다. 이해를 하고 나서는 "이렇게 까지 해야하나?" 였다.
ECS는 분명 데이터 처리 성능 측면에서 장점이 있다. 하지만 단순한 구조를 구현할때도 코드량이 증가하고 , 설계 복잡도가 올라간다는 단점이 존재한다.

ECS를 사용하기 위한 최소 조건은 다음과 같다.
1. 많은 데이터를 처리해야 하는 경우
   ECS는 청크 기반으로 메모리를 미리 할당하고 데이터를 연속적으로 관리하기 때문에 일정 규모 이상의 데이터가 있어야 효율적이다.
2. 반복적인 데이터 처리 비중이 높은 경우
   엔티티 생성 초기비용이 OOP보다 높을수 있지만 , 이후 데이터 처리 단계에서는 캐시 친화적인 구조로 인해 더 효율적이다.
3. 성능이 중요한 프로젝트인 경우
   객체 참조 기반 접근을 하는 OOP보다 데이터 배열순회를 하는 ECS가 CPU 캐시 활용 측면에서 효율적이다.
4. 개발비용(시간,돈)을 감당할 수 있는 경우
   ECS구조 이해 , 설계 , 초기개발 의 비용이 OOP보다 높다


# 결론
ECS가 OOP를 완전히 대체할 수 있는구조는 아니다. 하지만 특정 조건만 맞춰진다면 매우 효율적이고 합당한 선택지가 될수 있다.
따라서 ECS 사용 여부는 프로젝트의 요구사항과 상황에 따라 결정하는것이 적절하다.



