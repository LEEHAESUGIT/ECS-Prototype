
namespace LHS.ECS.Components
{
	internal static class ConponentSetting
	{
		internal static void SetComponent()
		{
			// flag
			ComponentTypeRegister.Set<NeedInit>(); // 무조건 항상 0번째
			// Status
			ComponentTypeRegister.Set<HPComponent>();
			ComponentTypeRegister.Set<DEFComponent>();
			ComponentTypeRegister.Set<ATKComponent>();
									
			ComponentTypeRegister.Set<EXPComponent>();
			ComponentTypeRegister.Set<GOLDComponent>();
			ComponentTypeRegister.Set<SPEEDComponent>();

			ComponentTypeRegister.IsFrozen = false;
		}
	}

	// Status Components
	internal struct HPComponent : IComponentData
	{
		internal float point;
	}

	internal struct DEFComponent : IComponentData
	{
		internal float point;
	}

	internal struct ATKComponent : IComponentData
	{
		internal float point;
	}

	internal struct EXPComponent : IComponentData
	{
		internal float point;
	}

	internal struct GOLDComponent : IComponentData
	{
		internal float point;
	}

	internal struct SPEEDComponent : IComponentData
	{
		internal float point;
	}

	// Component Flags
	internal struct NeedInit : IComponentData { }
}