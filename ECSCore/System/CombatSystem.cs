using ECSCore;

public class CombatSystem : ISystem
{
	public void OnUpdate(ECSManager world)
	{
		//	foreach(var entity in world.Query<HPComponent , ComponentGroup>())
		//	{
		//		world.GetHP(entity, out var hp);
		//		world.GetATK(entity, out var dmg);

		//		hp.point -= dmg.point;

		//		world.Remove<HPComponent>(entity);
		//	}
	}
}