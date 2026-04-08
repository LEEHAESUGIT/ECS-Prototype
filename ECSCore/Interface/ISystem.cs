namespace LHS.ECS.Interface
{
	public interface ISystem
	{

		public void Set(ECSManager ecs);
		public void OnUpdate(ECSManager ecs);
	}
}
