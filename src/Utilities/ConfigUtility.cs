using System;
using System.IO;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
namespace DataSentinel.Utilities{
    public static class ConfigUtility {
        public static string GetSectionAsString(this IConfiguration config, string sectionName)
        {
            var section = config.GetSection(sectionName);
            if(section == null || section.Value == null)
                throw new Exception($"The value of the section {sectionName} is null in the config file.");
            else
                return section.Value;
        }
    }
}