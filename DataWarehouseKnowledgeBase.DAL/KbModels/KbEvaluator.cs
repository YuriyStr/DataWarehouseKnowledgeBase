using System.Collections.Generic;
using System.Linq;

namespace DataWarehouseKnowledgeBase.DAL.KbModels
{
    public class KbEvaluator : IKbEvaluator
    {
        private readonly KnowledgeBase _kb;

        public KbEvaluator(IKbSerializer<KnowledgeBase> serializer)
        {
            _kb = serializer.Deserialize();
        }

        public string GetAttribute(string attributeName, object parameter)
        {
            return parameter?.GetType().GetProperty(attributeName)?.GetValue(parameter)?.ToString() ??
                _kb?.Rules?.Where(r => r.ThenAttributeName == attributeName)
                    .FirstOrDefault(r => VerifyRule(r, parameter))
                    ?.ThenAttributeValue;
        }

        private bool VerifyRule(Rule rule, object parameter)
        {
            var attributeDictionary = rule.RequiredAttributesSplitted
                .Select(a => new KeyValuePair<string, string>(a, GetAttribute(a, parameter)))
                .ToDictionary(pair => pair.Key, pair => pair.Value);
            return rule.Condition?.Evaluate(attributeDictionary, parameter) ?? false;
        }
    }
}
