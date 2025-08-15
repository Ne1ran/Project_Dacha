using System;
using System.Collections.Generic;
using Game.Utils;
using Newtonsoft.Json.Linq;

namespace Core.Parameters
{
    public readonly struct Parameters
    {
        public Dictionary<string, object> Dictionary { get; }

        public T? Get<T>(string name)
        {
            object? param = Dictionary.Get(name);

            if (param == null) {
                return default;
            }

            T result = Cast<T>(param);
            return result;
        }

        public Parameters AddParam(string paramName, object value)
        {
            Dictionary.Add(paramName, value);
            return new Parameters(Dictionary);
        }

        public Parameters AddParams(Parameters itemModelParameters)
        {
            foreach ((string key, object value) in itemModelParameters.Dictionary) {
                Dictionary.Add(key, value);
            }
            
            return new Parameters(Dictionary);
        }

        public T GetOrDefault<T>(string name, T defaultValue)
        {
            object? param = Dictionary.Get(name);
            if (param == null) {
                return defaultValue;
            }

            T result = Cast<T>(param);
            return result;
        }

        public T Require<T>(string name)
        {
            object param = Dictionary.Require(name);
            T result = Cast<T>(param);
            return result;
        }

        public IReadOnlyList<T> RequireCollection<T>(string name)
        {
            List<T> result = new List<T>();
            object param = Dictionary.Require(name);
            if (param is not JArray jArray) {
                throw new InvalidCastException();
            }

            foreach (JToken jToken in jArray) {
                if (jToken is not JObject jObject) {
                    continue;
                }

                result.Add(jObject.ToObject<T>()!);
            }

            return result;
        }

        public Parameters RequireParameters(string name)
        {
            object param = Dictionary.Require(name);
            Dictionary<string, object> dictionary = Cast<Dictionary<string, object>>(param);
            return new Parameters(dictionary);
        }

        private T Cast<T>(object obj)
        {
            Type type = typeof(T);

            if (type.IsEnum) {
                if (type == obj.GetType()) {
                    return (T) obj;
                }

                return (T) Game.Utils.Utils.ParseEnum((string) obj, type);
            }

            if (!(obj is JObject jParam)) {
                return (T) Convert.ChangeType(obj, type);
            }

            T? result = jParam.ToObject<T>();

            if (result == null) {
                throw new InvalidCastException();
            }

            return result;
        }

        public Parameters(Dictionary<string, object> parameters)
        {
            Dictionary = parameters;
        }

        public Parameters(ValueTuple<string, object> param)
        {
            Dictionary = new Dictionary<string, object> {
                    [param.Item1] = param.Item2
            };
        }

        public Parameters(ValueTuple<string, object> param1, ValueTuple<string, object> param2)
        {
            Dictionary = new Dictionary<string, object> {
                    [param1.Item1] = param1.Item2,
                    [param2.Item1] = param2.Item2
            };
        }

        public Parameters(ValueTuple<string, object> param1, ValueTuple<string, object> param2, ValueTuple<string, object> param3)
        {
            Dictionary = new Dictionary<string, object> {
                    [param1.Item1] = param1.Item2,
                    [param2.Item1] = param2.Item2,
                    [param3.Item1] = param3.Item2
            };
        }

        public Parameters(ValueTuple<string, object> param1,
                          ValueTuple<string, object> param2,
                          ValueTuple<string, object> param3,
                          ValueTuple<string, object> param4)
        {
            Dictionary = new Dictionary<string, object> {
                    [param1.Item1] = param1.Item2,
                    [param2.Item1] = param2.Item2,
                    [param3.Item1] = param3.Item2,
                    [param4.Item1] = param4.Item2,
            };
        }

        public Parameters(string paramName, object value)
        {
            Dictionary = new Dictionary<string, object>() {
                    { paramName, value }
            };
        }
    }
}