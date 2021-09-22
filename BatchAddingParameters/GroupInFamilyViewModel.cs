using Autodesk.Revit.DB;
using System.Collections.Generic;


namespace BatchAddingParameters
{
    public class GroupInFamilyViewModel
    {
        public string GroupName { get; set; }

        public List<string> GetGroups()
        {
            List<string> groups = new List<string>();
            groups.Add("Прочее");
            groups.Add("Размеры");
            groups.Add("Графика");
            groups.Add("Материалы и отделка");
            groups.Add("Видимость");
            groups.Add("Данные");
            groups.Add("Общие");
            groups.Add("Текст");
            groups.Add("Идентификация");
            groups.Add("Свойства модели");
            groups.Add("Параметры IFC");
            groups.Add("Стадии");
            groups.Add("Зависимости");
            groups.Add("Слои");
            groups.Add("Строительство");
            groups.Add("Несущие конструкции");
            groups.Add("Редактирование формы перекрытия");
            groups.Add("Набор арматурных стержней");
            groups.Add("Расчет несущих конструкций");
            groups.Add("Моменты");
            groups.Add("Силы");
            groups.Add("Геометрия разделения");
            groups.Add("Аналитическая модель");
            groups.Add("Механизмы");
            groups.Add("Механизмы - Расход");
            groups.Add("Механизмы - Нагрузки");
            groups.Add("Сантехника");
            groups.Add("Сегменты и соединительные детали");
            groups.Add("Система пожаротушения");
            groups.Add("Электросети");
            groups.Add("Электросети (А)");
            groups.Add("Электросети - Нагрузки");
            groups.Add("Электросети - Освещение");
            groups.Add("Электросети - Создание цепей");
            groups.Add("Рачет энергопотребления");
            groups.Add("Результаты анализа");
            groups.Add("Фотометрические");
            groups.Add("Свойства экологически чистого здания");
            groups.Add("Шрифт заголовков");
            groups.Add("Общая легенда");

            return groups;
        }
        public BuiltInParameterGroup Group()
        {
            if (GroupName == "Моменты") return BuiltInParameterGroup.PG_MOMENTS;
            else if (GroupName == "Силы") return BuiltInParameterGroup.PG_FORCES;
            else if (GroupName == "Геометрия разделения") return BuiltInParameterGroup.PG_DIVISION_GEOMETRY;
            else if (GroupName == "Сегменты и соединительные детали") return BuiltInParameterGroup.PG_SEGMENTS_FITTINGS;
            else if (GroupName == "Общая легенда") return BuiltInParameterGroup.PG_OVERALL_LEGEND;
            else if (GroupName == "Видимость") return BuiltInParameterGroup.PG_VISIBILITY;
            else if (GroupName == "Данные") return BuiltInParameterGroup.PG_DATA;
            else if (GroupName == "Электросети - Создание цепей") return BuiltInParameterGroup.PG_ELECTRICAL_CIRCUITING;
            else if (GroupName == "Общие") return BuiltInParameterGroup.PG_GENERAL;
            else if (GroupName == "Свойства модели") return BuiltInParameterGroup.PG_ADSK_MODEL_PROPERTIES;
            else if (GroupName == "Результаты анализа") return BuiltInParameterGroup.PG_ANALYSIS_RESULTS;
            else if (GroupName == "Редактирование формы перекрытия") return BuiltInParameterGroup.PG_SLAB_SHAPE_EDIT;
            else if (GroupName == "Фотометрические") return BuiltInParameterGroup.PG_LIGHT_PHOTOMETRICS;
            else if (GroupName == "Свойства экологически чистого здания") return BuiltInParameterGroup.PG_GREEN_BUILDING;
            else if (GroupName == "Шрифт заголовков") return BuiltInParameterGroup.PG_TITLE;
            else if (GroupName == "Система пожаротушения") return BuiltInParameterGroup.PG_FIRE_PROTECTION;
            else if (GroupName == "Аналитическая модель") return BuiltInParameterGroup.PG_ANALYTICAL_MODEL;
            else if (GroupName == "Набор арматурных стержней") return BuiltInParameterGroup.PG_REBAR_ARRAY;
            else if (GroupName == "Слои") return BuiltInParameterGroup.PG_REBAR_SYSTEM_LAYERS;
            else if (GroupName == "Параметры IFC") return BuiltInParameterGroup.PG_IFC;
            else if (GroupName == "Электросети (А)") return BuiltInParameterGroup.PG_AELECTRICAL;
            else if (GroupName == "Рачет энергопотребления") return BuiltInParameterGroup.PG_ENERGY_ANALYSIS;
            else if (GroupName == "Расчет несущих конструкций") return BuiltInParameterGroup.PG_STRUCTURAL_ANALYSIS;
            else if (GroupName == "Механизмы - Расход") return BuiltInParameterGroup.PG_MECHANICAL_AIRFLOW;
            else if (GroupName == "Механизмы - Нагрузки") return BuiltInParameterGroup.PG_MECHANICAL_LOADS;
            else if (GroupName == "Электросети - Нагрузки") return BuiltInParameterGroup.PG_ELECTRICAL_LOADS;
            else if (GroupName == "Электросети - Освещение") return BuiltInParameterGroup.PG_ELECTRICAL_LIGHTING;
            else if (GroupName == "Текст") return BuiltInParameterGroup.PG_TEXT;
            else if (GroupName == "Зависимости") return BuiltInParameterGroup.PG_CONSTRAINTS;
            else if (GroupName == "Стадии") return BuiltInParameterGroup.PG_PHASING;
            else if (GroupName == "Механизмы") return BuiltInParameterGroup.PG_MECHANICAL;
            else if (GroupName == "Несущие конструкции") return BuiltInParameterGroup.PG_STRUCTURAL;
            else if (GroupName == "Сантехника") return BuiltInParameterGroup.PG_PLUMBING;
            else if (GroupName == "Электросети") return BuiltInParameterGroup.PG_ELECTRICAL;
            else if (GroupName == "Материалы и отделка") return BuiltInParameterGroup.PG_MATERIALS;
            else if (GroupName == "Графика") return BuiltInParameterGroup.PG_GRAPHICS;
            else if (GroupName == "Строительство") return BuiltInParameterGroup.PG_CONSTRUCTION;
            else if (GroupName == "Размеры") return BuiltInParameterGroup.PG_GEOMETRY;
            else if (GroupName == "Идентификация") return BuiltInParameterGroup.PG_IDENTITY_DATA;
            else if (GroupName == "Прочее") return BuiltInParameterGroup.INVALID;
            else if (string.IsNullOrEmpty(GroupName)) return BuiltInParameterGroup.INVALID;
            else return BuiltInParameterGroup.INVALID;
        }
    }
}
