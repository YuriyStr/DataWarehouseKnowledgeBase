using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.CodeAnalysis.CSharp.Scripting;

namespace DataWarehouseKnowledgeBase.DAL.KbModels
{
    [Serializable]
    [XmlRoot(ElementName = "KB")]
    public class KnowledgeBase
    {
        public List<Rule> Rules { get; set; }
    }

    [Serializable]
    public class Rule
    {
        [XmlAttribute]
        public string ThenAttributeName { get; set; }

        [XmlAttribute]
        public string ThenAttributeValue { get; set; }

        [XmlAttribute]
        public string RequiredAttributes { get; set; }

        [XmlIgnore] public List<string> RequiredAttributesSplitted => RequiredAttributes?.Split(";".ToCharArray()).ToList();

        [XmlElement(Type = typeof(ConditionGroup), ElementName = "ConditionGroup")]
        [XmlElement(Type = typeof(ConditionNode), ElementName = "Condition")]
        public Condition Condition { get; set; }
    }

    [Serializable]
    public abstract class Condition
    {
        [XmlAttribute]
        public bool InvertResult { get; set; } = false;

        public bool Evaluate(Dictionary<string, string> parameters, object mainParameter)
        {
            return InvertResult != EvaluateCondition(parameters, mainParameter);
        }

        protected abstract bool EvaluateCondition(Dictionary<string, string> parameters, object mainParameter);
    }

    [Serializable]
    public class ConditionGroup : Condition
    {
        [XmlAttribute]
        public string GroupType { get; set; } = "AND";

        [XmlArrayItem(Type = typeof(ConditionGroup), ElementName = "ConditionGroup")]
        [XmlArrayItem(Type = typeof(ConditionNode), ElementName = "Condition")]
        public List<Condition> Conditions { get; set; }

        protected override bool EvaluateCondition(Dictionary<string, string> parameters, object mainParameter)
        {
            switch (GroupType.ToUpper())
            {
                case "AND":
                    return Conditions.All(c => c.Evaluate(parameters, mainParameter));
                case "OR":
                    return Conditions.Any(c => c.Evaluate(parameters, mainParameter));
                default:
                    return false;
            }
        }
    }

    [Serializable]
    public class ConditionNode : Condition
    {
        [XmlAttribute]
        public string Condition { get; set; }

        protected override bool EvaluateCondition(Dictionary<string, string> parameters, object mainParameter)
        {
            if (string.IsNullOrEmpty(Condition))
                return true;
            var refactoredCondition = RefactorCondition(parameters);
            return CSharpScript.EvaluateAsync<bool>(refactoredCondition).Result;
        }

        private string RefactorCondition(Dictionary<string, string> parameters)
        {
            var builder = new StringBuilder(Condition);
            foreach (var item in parameters)
            {
                builder.Replace("{{" + item.Key + "}}", item.Value?.ToLower());
            }
            return builder.ToString();
        }
    }
}
