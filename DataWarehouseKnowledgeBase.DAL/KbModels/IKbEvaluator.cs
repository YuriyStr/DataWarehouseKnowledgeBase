namespace DataWarehouseKnowledgeBase.DAL.KbModels
{
    public interface IKbEvaluator
    {
        string GetAttribute(string attributeName, object parameter);
    }
}
