using Microsoft.CodeAnalysis.CSharp.Scripting;

namespace DataWarehouseKnowledgeBase.DAL.KbModels
{
    public class KbEvaluator<TParam> : IKbEvaluator<TParam>
    {
        private readonly KnowledgeBase _kb;
        private readonly TParam _parameter;

        public KbEvaluator(KbSerializer<KnowledgeBase> serializer, TParam parameter)
        {

        }

        public T GetAttribute<T>(string attributeName)
        {
            throw new System.NotImplementedException();
        }
    }
}
