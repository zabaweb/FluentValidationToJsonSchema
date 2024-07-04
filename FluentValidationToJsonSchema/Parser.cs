using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentValidation;
using FluentValidation.Internal;
using Newtonsoft.Json.Linq;

namespace FluentValidatorToJsonSchema
{
    public class Parser : IParser
    {
        public JObject Parse(IValidator validator)
        {
            var schema = GetEmptyObject();

            if (validator == null)
            {
                return schema;
            }

            var descriptor = validator.CreateDescriptor();

            if (descriptor == null || !descriptor.Rules.Any())
            {
                return schema;
            }

            var propetriesRules = new Dictionary<string, JObject>();

            foreach (var rule in descriptor.Rules)
            {
                AddRuleToSchema(rule, propetriesRules);
            }

            var properties = new JObject();
            foreach (var (propertyName, propertySchema) in propetriesRules.Select(x => (x.Key, x.Value)))
            {
                properties.Add(propertyName, propertySchema);
            }

            schema.Add("properties", properties);

            return schema;
        }

        private void AddRuleToSchema(IValidationRule rule, Dictionary<string, JObject> propetriesRules)
        {
            JObject propertyObject;
            if (propetriesRules.ContainsKey(rule.PropertyName))
            {
                propertyObject = propetriesRules[rule.PropertyName];
            }
            else
            {
                propertyObject = new JObject();
                propetriesRules.Add(rule.PropertyName, propertyObject);
            }

            foreach (var component in rule.Components)
            {
                AddComponentToPropertyObject(component, rule.Member, propertyObject);
            }

        }

        private void AddComponentToPropertyObject(IRuleComponent component, MemberInfo member, JObject propertyObject)
        {
            switch (component.Validator.Name)
            {
                case "NotNullValidator":
                    ProcessNotNullValidator(component, member, propertyObject);
                    break;
                default:
                    break;
            }
        }

        private void ProcessNotNullValidator(IRuleComponent component, MemberInfo member, JObject propertyObject)
        {
            propertyObject.Remove("type");
            var propertyTypeName = MemberInfoToTypeName(member);
            propertyObject.Add("type", new JArray { propertyTypeName });
        }

        private string MemberInfoToTypeName(MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Property:
                    var propertyInfo = member as PropertyInfo;
                    return TypeToTypeName(propertyInfo.PropertyType);
                default:
                    return "string";
            }
        }

        private string TypeToTypeName(Type type)
        {
            switch (type.Name)
            {
                case "Nullable`1":
                    return TypeToTypeName(type.GenericTypeArguments[0]);
                case "String":
                    return "string";
                case "Int32":
                    return "number";
                case "Object":
                    return "object";
                default:
                    return "any";
            }
        }

        private static JObject GetEmptyObject()
        {
            var schema = new JObject();

            schema.Add("$schema", new JValue("https://json-schema.org/draft/2020-12/schema"));
            schema.Add("type", new JValue("object"));
            return schema;
        }
    }
}
