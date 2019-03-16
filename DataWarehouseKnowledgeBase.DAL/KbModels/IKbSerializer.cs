namespace DataWarehouseKnowledgeBase.DAL.KbModels
{
    public interface IKbSerializer<T>
    {
        void Serialize(T obj);

        T Deserialize();
    }
}
