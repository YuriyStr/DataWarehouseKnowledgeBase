using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace DataWarehouseKnowledgeBase.DAL.KbModels
{
    [Serializable]
    [XmlRoot(ElementName = "KB")]
    public class KnowledgeBase
    {
        [XmlAttribute]
        public string Parameter { get; set; }

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

        [XmlArrayItem(Type = typeof(ConditionGroup), ElementName = "ConditionGroup")]
        [XmlArrayItem(Type = typeof(ConditionNode), ElementName = "Condition")]
        public List<Condition> Conditions { get; set; }
    }

    [Serializable]
    public abstract class Condition
    {
        [XmlAttribute]
        public bool InvertResult { get; set; } = false;
    }

    [Serializable]
    public class ConditionGroup : Condition
    {
        [XmlAttribute]
        public string GroupType { get; set; } = "AND";

        [XmlArrayItem(Type = typeof(ConditionGroup), ElementName = "ConditionGroup")]
        [XmlArrayItem(Type = typeof(ConditionNode), ElementName = "Condition")]
        public List<Condition> Conditions { get; set; }
    }

    [Serializable]
    public class ConditionNode : Condition
    {
        [XmlAttribute]
        public string Condition { get; set; }
    }
}
