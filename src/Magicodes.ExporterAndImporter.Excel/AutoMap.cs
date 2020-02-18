﻿using CsvHelper.Configuration;
using Magicodes.ExporterAndImporter.Core;
using Magicodes.ExporterAndImporter.Core.Extension;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Magicodes.ExporterAndImporter.Excel
{
    /// <summary>
    ///     动态构建映射
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AutoMap<T> : ClassMap<T>
    {
        /// <summary>
        ///     构造方法
        /// </summary>
        public AutoMap()
        {
            var properties = typeof(T).GetProperties();
            var nameProperty = properties.FirstOrDefault(p => p.Name == "Name");
            if (nameProperty != null)
                MapProperty(nameProperty).Item1.Index(0);
            foreach (var prop in properties.Where(p => p != nameProperty))
            {
                var result = MapProperty(prop);
                var tcOption = result.Item1.TypeConverterOption;
                tcOption.NumberStyles(NumberStyles.Any);
                tcOption.DateTimeStyles(DateTimeStyles.None);
                var format = tcOption.Format();
                if (result.Item2!=null)
                {
                    if (!string.IsNullOrEmpty(result.Item2?.Format))
                    {
                        tcOption.Format(result.Item2.Format);
                    }
                    if (result.Item2.IsIgnore)
                    {
                        format.Ignore();
                    }
                }
                else if(result.Item3 != null)
                {
                    if (result.Item3.IsIgnore)
                    {
                        format.Ignore();
                    }
                }

                if (result.Item1.GetType().IsEnum)
                {
                   
                }
               // result.Item1.TypeConverter<CsvHelperEnumConverter<>>()
                //Map(m => m.Gender).TypeConverter<CalendarExceptionEnumConverter<Genders>>().Name("性别");
                //result.Item1.Configuration.TypeConverterCache.AddConverter<TestEnum>(new Converters.EnumConverter());


            }
        }

        private (MemberMap, ExporterHeaderAttribute, ImporterHeaderAttribute) MapProperty(PropertyInfo property)
        {
            var map = Map(typeof(T), property);
            string name = property.Name;
            var headerAttribute = property.GetCustomAttribute<ExporterHeaderAttribute>();
            if (headerAttribute != null)
            {
                name = headerAttribute.DisplayName ?? property.GetDisplayName() ?? property.Name;
            }
            var importAttribute = property.GetCustomAttribute<ImporterHeaderAttribute>();
            if (importAttribute != null)
            {
                name = importAttribute.Name ?? property.GetDisplayName() ?? property.Name;
            }
            map.Name(name);
            return (map, headerAttribute,importAttribute);

        }
    }
}