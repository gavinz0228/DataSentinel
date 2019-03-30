using System;
using System.IO;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
namespace DataSentinel.Utilities{
    public static class SystemUtility {
        public static string GetEnvironmentVariableAsString(string key)
        {
            string value = Environment.GetEnvironmentVariable(key);
            if(value == null)
                throw new Exception($"The value is null for the environment variable {key}.");
            else
                return value;
        }
    }
}