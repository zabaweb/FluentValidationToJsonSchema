using FluentValidation;
using Newtonsoft.Json.Linq;

namespace FluentValidatorToJsonSchema
{
    public interface IParser
    {
        JObject Parse(IValidator validator, bool verbose = false);
    }
}