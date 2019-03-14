using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json.Linq;
namespace DataSentinel.Infrastructure.InputFormaters
{
    public class JsonStringBodyInputFormatter : InputFormatter
    {
        public JsonStringBodyInputFormatter()
        {
            this.SupportedMediaTypes.Add("application/json");
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            var request = context.HttpContext.Request;
            return await InputFormatterResult.SuccessAsync(request.Body);

        }

        protected override bool CanReadType(Type type)
        {
            return type == typeof(Stream);
        }
    }
}