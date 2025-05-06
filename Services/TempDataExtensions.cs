using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace MediaPlus.Services
{
    public static class TempDataExtensions
    {
        public static void SetObject<T>(this ITempDataDictionary tempData, string key, T value)
        {
            tempData[key] = JsonSerializer.Serialize(value);
        }

        public static void UpdateObject<T>(this ITempDataDictionary tempData,string key, T value)
        {
            tempData.SetObject(key, value);
        }

        public static T? GetObject<T>(this ITempDataDictionary tempData, string key)
        {
            var data = tempData[key];
            return data == null ? default : JsonSerializer.Deserialize<T>((string)data);
        }

        public static void ClearObject(this ITempDataDictionary tempData, string key){
            tempData.Remove(key);
        }
    }
}