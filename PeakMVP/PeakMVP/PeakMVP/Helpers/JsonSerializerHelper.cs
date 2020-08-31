using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using PeakMVP.Models.Rests.Requests.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace PeakMVP.Helpers {
    internal static class JsonSerializerHelper {

        public static byte[] SerializeToByteArray<TRequest>(object requestDataModel) where TRequest : IRequest {
            string serializedRequestDataModel = JsonConvert.SerializeObject(requestDataModel);
            return Encoding.Unicode.GetBytes(serializedRequestDataModel);
        }

        public static string SerializeObject<TRequest>(object requestDataModel) where TRequest : IRequest =>
            JsonConvert.SerializeObject(requestDataModel);

        public static TResult Deserialize<TResult>(string jsonObj) {
            try {
                TResult obj = default(TResult);
                if (!string.IsNullOrEmpty(jsonObj)) {
                    obj = JsonConvert.DeserializeObject<TResult>(jsonObj);
                }
                return obj;
            } catch (Exception ex) {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Method", "JsonSerializerHelper.Deserialize" } });

                Debugger.Break();
                Debug.WriteLine($"ERROR:{ex.Message}");
                throw;
            }
        }
    }
}
