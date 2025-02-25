#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Flurl.Util;
using Jint.Native;
using Jint.Native.Object;
using Jint.Runtime.Interop;

namespace Jint.Tests.Runtime.Converters;

public class EnumDictionaryTypeConverter : DefaultTypeConverter
{
    private readonly Engine _engine;

    public EnumDictionaryTypeConverter(Engine engine) : base(engine)
    {
        _engine = engine;
    }

    public override bool TryConvert(
        object? value,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors |
                                    DynamicallyAccessedMemberTypes.PublicFields)]
        Type type,
        IFormatProvider formatProvider,
        [NotNullWhen(true)] out object? converted)
    {
        converted = null;

        if (value == null)
        {
            return base.TryConvert(value, type, formatProvider, out converted);
        }

        // If the type is a dictionary with enum keys, handle it specially
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
        {
            var keyType = type.GetGenericArguments()[0];
            var valueType = type.GetGenericArguments()[1];
            // create new instance of type
            var dictionary = (System.Collections.IDictionary)Activator.CreateInstance(type)!;

            // value.ToKeyValuePairs()
            var innerSucceeded = true;
            foreach (var keyValuePair in value.ToKeyValuePairs())
            {
                
                Console.WriteLine($"key: {keyValuePair.Key}, value: {keyValuePair.Value}");
                object? convertedKey;
                object? convertedValue;
                if (!this.TryConvert(keyValuePair.Key, keyType, formatProvider, out convertedKey))
                {
                    innerSucceeded = false;
                    break;
                }

                if (!this.TryConvert(keyValuePair.Value, valueType, formatProvider, out convertedValue))
                {
                    innerSucceeded = false;
                    break;
                }
                dictionary.Add(convertedKey,convertedValue);

            }

            if (innerSucceeded)
            {
                converted = dictionary;
                return true;
            }
        }

        return base.TryConvert(value, type, formatProvider, out converted);
    }
} 
