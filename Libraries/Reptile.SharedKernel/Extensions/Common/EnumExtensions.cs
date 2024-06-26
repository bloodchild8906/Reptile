﻿using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;

namespace Reptile.SharedKernel.Extensions.Common;

public static class EnumExtensions
{
    public static T ParseAsEnumByDescriptionAttribute<T>(this string description) // where T : enum
    {
        if (string.IsNullOrEmpty(description))
            throw new ArgumentNullException(description, @"Cannot parse an empty description");

        var enumType = typeof(T);
        if (!enumType.IsEnum) throw new InvalidOperationException($"Invalid Enum type{typeof(T)}");

        foreach (T item in Enum.GetValues(typeof(T)))
        {
            var attributes =
                (DescriptionAttribute[])
                item.GetType()
                    .GetField(item.ToString())
                    .GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Length > 0 && attributes[0].Description.ToUpper() == description.ToUpper()) return item;
        }

        throw new InvalidOperationException(
            $"Couldn't find enum of type {typeof(T)} with attribute of '{description}'");
    }


    public static string GetXmlEnumAttribute(this Enum enumerationValue)
    {
        var attributes =
            (XmlEnumAttribute[])
            enumerationValue.GetType()
                .GetField(enumerationValue.ToString())
                .GetCustomAttributes(typeof(XmlEnumAttribute), false);
        return attributes.Length > 0 ? attributes[0].Name : enumerationValue.ToString();
    }

	public static T GetEnumAttribute<T>(this Enum enumerationValue) where T : Attribute => ((T[])
				enumerationValue.GetType()
					.GetField(enumerationValue.ToString())
					.GetCustomAttributes(typeof(T), false))
			.FirstOrDefault();

	public static IEnumerable<T> GetAllItems<T>(this Enum value) => from object item in Enum.GetValues(typeof(T)) select (T)item;

	public static IEnumerable<T> GetAllItems<T>() where T : struct => Enum.GetValues(typeof(T)).Cast<T>();

	public static IEnumerable<T> GetAllSelectedItems<T>(this Enum value)
    {
        var valueAsInt = Convert.ToInt32(value, CultureInfo.InvariantCulture);

        return from object item in Enum.GetValues(typeof(T))
            let itemAsInt = Convert.ToInt32(item, CultureInfo.InvariantCulture)
            where itemAsInt == (valueAsInt & itemAsInt)
            select (T)item;
    }

    public static bool Contains<T>(this Enum value, T request)
    {
        var valueAsInt = Convert.ToInt32(value, CultureInfo.InvariantCulture);
        var requestAsInt = Convert.ToInt32(request, CultureInfo.InvariantCulture);

        if (requestAsInt == (valueAsInt & requestAsInt)) return true;

        return false;
    }

    public static T? TryParseEnum<T>(this string enumerationString) where T : struct
    {
        if (GetAllItems<T>().Any(e => e.ToString() == enumerationString)) return enumerationString.ToEnum<T>();

        return null;
    }

    public static T ToEnum<T>(this string enumerationString)
    {
        if (string.IsNullOrWhiteSpace(enumerationString))
            return default;

        return (T)Enum.Parse(typeof(T), enumerationString);
    }

    public static string GetDescription(this Enum? enumerationValue)
    {
        if (enumerationValue == null)
            return null;
        var attributes =
            (DescriptionAttribute[])
            enumerationValue.GetType()
                .GetField(enumerationValue.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false);
        return attributes.Length > 0 ? attributes[0].Description : enumerationValue.ToString();
    }
}