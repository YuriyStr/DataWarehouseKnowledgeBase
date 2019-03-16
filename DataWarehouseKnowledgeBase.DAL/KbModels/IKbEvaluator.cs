namespace DataWarehouseKnowledgeBase.DAL.KbModels
{
    public interface IKbEvaluator<TParam>
    {
        T GetAttribute<T>(string attributeName);
    }
}
