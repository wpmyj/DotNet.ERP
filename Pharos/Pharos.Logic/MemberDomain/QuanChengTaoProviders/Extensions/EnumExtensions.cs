﻿using Pharos.Logic.MemberDomain.QuanChengTaoProviders.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Pharos.Logic.MemberDomain.QuanChengTaoProviders.Extensions
{
    public static class EnumExtensions
    {
        public static IDictionary<int, string> GetIntegralProviderTypeTitleAndValue()
        {
            var dict = new Dictionary<int, string>();
            var enumType = typeof(IntegralProviderType);
            var values = Enum.GetValues(enumType);
            foreach (var value in values)
            {
                string name = Enum.GetName(enumType, value);
                if (name != null)
                {
                    // 获取枚举字段。
                    FieldInfo fieldInfo = enumType.GetField(name);
                    if (fieldInfo != null)
                    {
                        // 获取描述的属性。
                        DescriptionAttribute attr = Attribute.GetCustomAttribute(fieldInfo,
                            typeof(DescriptionAttribute), false) as DescriptionAttribute;
                        if (attr != null)
                        {
                            dict.Add((int)value, attr.Description);
                        }
                    }
                }
            }
            return dict;
        }
        public static IDictionary<int, string> GetMeteringModeDescriptionAttributeTitleAndValue(IntegralProviderType integralProviderType)
        {
            var dict = new Dictionary<int, string>();
            var enumType = typeof(MeteringMode);
            var values = Enum.GetValues(enumType);
            foreach (var value in values)
            {
                string name = Enum.GetName(enumType, value);
                if (name != null)
                {
                    // 获取枚举字段。
                    FieldInfo fieldInfo = enumType.GetField(name);
                    if (fieldInfo != null)
                    {
                        // 获取描述的属性。
                        Attribute[] attrs = Attribute.GetCustomAttributes(fieldInfo,
                            typeof(MeteringModeDescriptionAttribute), false);
                        var attr = attrs.FirstOrDefault(o => (((MeteringModeDescriptionAttribute)o).IntegralProviderType & integralProviderType) == integralProviderType);
                        if (attr != null)
                        {
                            dict.Add((int)value, ((MeteringModeDescriptionAttribute)attr).Description);
                        }
                    }
                }
            }
            return dict;
        }
        public static IDictionary<int, string> GetEnumDescription<T>()
        {
            var dict = new Dictionary<int, string>();
            var enumType = typeof(T);
            var vals = Enum.GetValues(enumType);
            foreach (var item in vals)
            {
                string name = Enum.GetName(enumType, item);
                if (!string.IsNullOrEmpty(name))
                {
                    // 获取枚举字段。
                    FieldInfo fieldInfo = enumType.GetField(name);
                    if (fieldInfo != null)
                    {
                        // 获取描述的属性。
                        DescriptionAttribute attr = Attribute.GetCustomAttribute(fieldInfo,
                            typeof(DescriptionAttribute), false) as DescriptionAttribute;
                        if (attr != null)
                        {
                            dict.Add((int)item, (attr).Description);
                        }
                    }
                }
            }
            return dict;
        }

        /// <summary>
        /// 取单个枚举的描述
        /// </summary>
        /// <param name="enumValue"></param>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static string GetEnumDescription(int enumValue, Type enumType)
        {
            string name = Enum.GetName(enumType, enumValue);
            if (name != null)
            {
                // 获取枚举字段。
                FieldInfo fieldInfo = enumType.GetField(name);
                if (fieldInfo != null)
                {
                    // 获取描述的属性。
                    DescriptionAttribute attr = Attribute.GetCustomAttribute(fieldInfo,
                        typeof(DescriptionAttribute), false) as DescriptionAttribute;
                    if (attr != null)
                    {
                        return attr.Description;
                    }
                }
            }
            return string.Empty;
        }

    }
}