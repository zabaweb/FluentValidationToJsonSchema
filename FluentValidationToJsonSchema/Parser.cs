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
        private bool _verbose = false;
        public JObject Parse(IValidator validator, bool verbose = false)
        {
            _verbose = verbose;
            var schema = GetEmptyObject();

            if (_verbose)
            {
                Console.WriteLine("Starting schema parsing...");
            }

            if (validator == null)
            {
                if (_verbose)
                {
                    Console.WriteLine("Validator is null. Returning an empty schema.");
                }
                return schema;
            }

            var descriptor = validator.CreateDescriptor();

            if (_verbose)
            {
                Console.WriteLine("Validator descriptor created.");
            }

            if (descriptor == null || !descriptor.Rules.Any())
            {
                if (_verbose)
                {
                    Console.WriteLine("Validator descriptor is null or contains no rules. Returning an empty schema.");
                }
                return schema;
            }

            var propetriesRules = new Dictionary<string, JObject>();

            foreach (var rule in descriptor.Rules)
            {
                if (_verbose)
                {
                    Console.WriteLine($"Processing rule for property: {rule.PropertyName}");
                }
                AddRuleToSchema(rule, propetriesRules);
            }

            var properties = new JObject();
            foreach (var (propertyName, propertySchema) in propetriesRules.Select(x => (x.Key, x.Value)))
            {
                if (_verbose)
                {
                    Console.WriteLine($"Adding property '{propertyName}' to schema.");
                }
                properties.Add(propertyName, propertySchema);
            }

            schema.Add("properties", properties);

            if (_verbose)
            {
                Console.WriteLine("Schema successfully generated.");
            }

            return schema;
        }

        private void AddRuleToSchema(IValidationRule rule, Dictionary<string, JObject> propetriesRules)
        {
            JObject propertyObject;
            if (propetriesRules.ContainsKey(rule.PropertyName))
            {
                propertyObject = propetriesRules[rule.PropertyName];
                if (_verbose)
                {
                    Console.WriteLine($"Property '{rule.PropertyName}' already exists in schema. Using existing object.");
                }
            }
            else
            {
                propertyObject = new JObject();
                propetriesRules.Add(rule.PropertyName, propertyObject);
                if (_verbose)
                {
                    Console.WriteLine($"Property '{rule.PropertyName}' added to schema.");
                }
            }

            foreach (var component in rule.Components)
            {
                if (_verbose)
                {
                    Console.WriteLine($"Processing component '{component.Validator.Name}' for property '{rule.PropertyName}'.");
                }
                AddComponentToPropertyObject(component, rule.Member, propertyObject);
            }
        }

        private void AddComponentToPropertyObject(IRuleComponent component, MemberInfo member, JObject propertyObject)
        {
            if (_verbose)
            {
                Console.WriteLine($"Adding component '{component.Validator.Name}' to property object.");
            }

            switch (component.Validator.Name)
            {
                case "NotNullValidator":
                    ProcessNotNullValidator(component, member, propertyObject);
                    break;
                case "RegularExpressionValidator":
                    ProcessRegularExpressionValidator(component, member, propertyObject);
                    break;
                case "NotEmptyValidator":
                    ProcessNotEmptyValidator(component, member, propertyObject);
                    break;
                default:
                    if (_verbose)
                    {
                        Console.WriteLine($"Unknown validator '{component.Validator.Name}' encountered.");
                    }
                    break;
            }
        }

        private void ProcessRegularExpressionValidator(IRuleComponent component, MemberInfo member, JObject propertyObject)
        {
            if (_verbose)
            {
                Console.WriteLine("Processing RegularExpressionValidator.");
            }

            var regexValidator = component.Validator as FluentValidation.Validators.IRegularExpressionValidator;

            if (regexValidator == null)
            {
                throw new InvalidOperationException("Expected a RegularExpressionValidator.");
            }

            propertyObject["type"] = "string";
            propertyObject["pattern"] = regexValidator.Expression;
        }

        private void ProcessNotNullValidator(IRuleComponent component, MemberInfo member, JObject propertyObject)
        {
            if (_verbose)
            {
                Console.WriteLine("Processing NotNullValidator.");
            }

            propertyObject.Remove("type");
            var propertyTypeName = MemberInfoToTypeName(member);
            propertyObject.Add("type", new JArray { propertyTypeName });
        }

        private void ProcessNotEmptyValidator(IRuleComponent component, MemberInfo member, JObject propertyObject)
        {
            if (_verbose)
            {
                Console.WriteLine("Processing NotEmptyValidator.");
            }

            var propertyTypeName = MemberInfoToTypeName(member);

            switch (propertyTypeName)
            {
                case "string":
                    propertyObject["type"] = "string";
                    propertyObject["minLength"] = 1;
                    break;

                case "array":
                    propertyObject["type"] = "array";
                    propertyObject["minItems"] = 1;
                    break;

                default:
                    propertyObject["type"] = propertyTypeName;
                    break;
            }
        }

        private string MemberInfoToTypeName(MemberInfo member)
        {
            if (_verbose)
            {
                Console.WriteLine($"Determining type name for member: {member.Name}");
            }

            switch (member.MemberType)
            {
                case MemberTypes.Property:
                    var propertyInfo = member as PropertyInfo;
                    return TypeToTypeName(propertyInfo.PropertyType);
                default:
                    return "string";
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0066:Convert switch statement to expression", Justification = "<Pending>")]
        private string TypeToTypeName(Type type)
        {
            if (_verbose)
            {
                Console.WriteLine($"Mapping CLR type '{type.Name}' to JSON schema type.");
            }

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
                case "List`1":
                    return "array";
                default:
                    return "any";
            }
        }

        private JObject GetEmptyObject()
        {
            if (_verbose)
            {
                Console.WriteLine("Creating an empty JSON schema object.");
            }

            var schema = new JObject();

            schema.Add("$schema", new JValue("https://json-schema.org/draft/2020-12/schema"));
            schema.Add("type", new JValue("object"));
            return schema;
        }
    }
}
