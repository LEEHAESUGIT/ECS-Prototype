// Entity.ID -> EntityRecord[Index]   (엔티티레코드 리스트의 인덱스) 
namespace ECSCore
{

	public class Entity : IEntity
	{
		internal readonly int ID;
		internal readonly int Generation;
		internal Entity(int id, int generation)
		{
			this.ID = id;
			this.Generation = generation;
		}

	}

}
