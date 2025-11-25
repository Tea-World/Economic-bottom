using Microsoft.Analytics.Interfaces;
using Microsoft.Analytics.Interfaces.Streaming;
using Microsoft.Analytics.Types.Sql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Library_function_calculator
{
    public class RealEstateProject
    {
        // Основные входные параметры
        public int Floors { get; set; }                    // F - Количество этажей
        public int UnitsPerFloor { get; set; }            // U - Квартир на этаж
        public double AverageArea { get; set; }           // S - Средняя площадь квартиры (м²)
        public double PricePerSqm { get; set; }           // A - Цена продажи за м²
        public double ConstructionCostPerSqm { get; set; } // B - Стоимость строительства за м²
        public double LandCost { get; set; }              // C_land - Стоимость земли
        public double UtilitiesCost { get; set; }         // C_utilities - Затраты на коммуникации
        public double DesignCost { get; set; }            // C_design - Затраты на проектирование
        public double MarketingCost { get; set; }         // C_marketing - Маркетинговые затраты
        public double OtherCosts { get; set; }            // C_other - Прочие затраты
        public double LandTax { get; set; }               // C_tax_land - Налог на землю
        public double ConstructionTax { get; set; }       // C_tax_construction - Налоги на строительство
        public double ProfitTaxRate { get; set; }         // T_profit - Ставка налога на прибыль (например, 0.20 для 20%)
        public double ProjectDurationYears { get; set; }  // T - Срок реализации (лет)
    }

    public class ProjectCalculationResult
    {
        public int TotalApartments { get; set; }          // N - Общее количество квартир
        public double TotalRevenue { get; set; }          // P - Общая выручка
        public double TotalCostBeforeTax { get; set; }    // C_before_tax - Затраты до налога на прибыль
        public double ConstructionCost { get; set; }      // C_construction - Стоимость строительства
        public double ProfitBeforeTax { get; set; }       // Прибыль до налогообложения
        public double NetProfit { get; set; }             // Profit - Чистая прибыль
        public double ROI { get; set; }                   // ROI - Доходность инвестиций (%)
        public double AnnualizedROI { get; set; }         // Годовая доходность (%)
    }

    public static class RealEstateCalculator
    {
        /// <summary>
        /// Расчет доходности инвестиционного проекта в недвижимость
        /// </summary>
        /// <param name="project">Параметры проекта</param>
        /// <returns>Результаты расчета</returns>
        public static ProjectCalculationResult CalculateROI(RealEstateProject project)
        {
            var result = new ProjectCalculationResult();

            // Шаг 1: Расчет выручки (P)
            result.TotalApartments = project.Floors * project.UnitsPerFloor; // N = F * U
            result.TotalRevenue = result.TotalApartments * project.AverageArea * project.PricePerSqm; // P = N * S * A

            // Шаг 2: Расчет затрат до налога на прибыль (C_before_tax)
            result.ConstructionCost = result.TotalApartments * project.AverageArea * project.ConstructionCostPerSqm; // C_construction = N * S * B

            result.TotalCostBeforeTax = project.LandCost
                                      + result.ConstructionCost
                                      + project.UtilitiesCost
                                      + project.DesignCost
                                      + project.MarketingCost
                                      + project.OtherCosts
                                      + project.LandTax
                                      + project.ConstructionTax;

            // Шаг 3: Расчет чистой прибыли (Profit)
            result.ProfitBeforeTax = result.TotalRevenue - result.TotalCostBeforeTax;
            result.NetProfit = result.ProfitBeforeTax * (1 - project.ProfitTaxRate);

            // Шаг 4: Расчет доходности (ROI)
            result.ROI = (result.NetProfit / result.TotalCostBeforeTax) * 100;

            // Шаг 5: Расчет годовой доходности
            result.AnnualizedROI = (Math.Pow(1 + result.ROI / 100, 1 / project.ProjectDurationYears) - 1) * 100;

            return result;
        }

        /// <summary>
        /// Пример использования с валидацией входных данных
        /// </summary>
        public static ProjectCalculationResult CalculateROIWithValidation(RealEstateProject project)
        {
            // Валидация входных параметров
            if (project == null)
                throw new ArgumentNullException(nameof(project));

            if (project.Floors <= 0)
                throw new ArgumentException("Количество этажей должно быть положительным числом");

            if (project.UnitsPerFloor <= 0)
                throw new ArgumentException("Количество квартир на этаже должно быть положительным числом");

            if (project.AverageArea <= 0)
                throw new ArgumentException("Средняя площадь квартиры должна быть положительным числом");

            if (project.PricePerSqm <= 0)
                throw new ArgumentException("Цена за м² должна быть положительным числом");

            if (project.ConstructionCostPerSqm <= 0)
                throw new ArgumentException("Стоимость строительства за м² должна быть положительным числом");

            if (project.ProjectDurationYears <= 0)
                throw new ArgumentException("Срок реализации должен быть положительным числом");

            return CalculateROI(project);
        }
    }

    // Пример использования
    class Program
    {
        static void Main()
        {
            // Создание тестового проекта
            var project = new RealEstateProject
            {
                Floors = 10,
                UnitsPerFloor = 4,
                AverageArea = 75.5,
                PricePerSqm = 120000,
                ConstructionCostPerSqm = 70000,
                LandCost = 50000000,
                UtilitiesCost = 15000000,
                DesignCost = 5000000,
                MarketingCost = 8000000,
                OtherCosts = 3000000,
                LandTax = 2000000,
                ConstructionTax = 10000000,
                ProfitTaxRate = 0.20, // 20%
                ProjectDurationYears = 2.5
            };

            try
            {
                // Расчет показателей
                var result = RealEstateCalculator.CalculateROIWithValidation(project);

                // Вывод результатов
                Console.WriteLine($"РЕЗУЛЬТАТЫ РАСЧЕТА:");
                Console.WriteLine($"==================");
                Console.WriteLine($"Количество квартир: {result.TotalApartments} шт.");
                Console.WriteLine($"Общая выручка: {result.TotalRevenue:N0} руб.");
                Console.WriteLine($"Затраты до налога: {result.TotalCostBeforeTax:N0} руб.");
                Console.WriteLine($"Стоимость строительства: {result.ConstructionCost:N0} руб.");
                Console.WriteLine($"Прибыль до налога: {result.ProfitBeforeTax:N0} руб.");
                Console.WriteLine($"Чистая прибыль: {result.NetProfit:N0} руб.");
                Console.WriteLine($"ROI: {result.ROI:F2}%");
                Console.WriteLine($"Годовая ROI: {result.AnnualizedROI:F2}%");

                // Анализ эффективности
                Console.WriteLine($"\nАНАЛИЗ ЭФФЕКТИВНОСТИ:");
                Console.WriteLine($"===================");
                if (result.ROI > 25)
                    Console.WriteLine("ВЫСОКАЯ эффективность");
                else if (result.ROI > 15)
                    Console.WriteLine("СРЕДНЯЯ эффективность");
                else if (result.ROI > 0)
                    Console.WriteLine("НИЗКАЯ эффективность");
                else
                    Console.WriteLine("ПРОЕКТ УБЫТОЧЕН");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при расчете: {ex.Message}");
            }
        }
    }
}