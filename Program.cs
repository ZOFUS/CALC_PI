using System;
using System.IO;
using System.Linq;

class Program
{
    // Координаты центра окружности и радиус
    static double x0 = 1.0;
    static double y0 = 2.0;
    static double r0 = 5.0;

    // Пределы интегрирования для функции f(x)
    static double a = 0;
    static double b = 2;

    // Числа экспериментов
    static int[] expNums = { 10000, 100000, 1000000, 10000000, 100000000 };

    // Генерация случайных чисел и расчет π
    static Random random = new Random();
    static double[][] CalculatePi(int[] expNums, int numSeries, double x0, double y0, double r0)
    {
        double xmin = x0 - r0;
        double xmax = x0 + r0;
        double ymin = y0 - r0;
        double ymax = y0 + r0;
        double[][] results = new double[numSeries][];

        for (int i = 0; i < numSeries; i++)
        {
            results[i] = new double[expNums.Length];
            for (int j = 0; j < expNums.Length; j++)
            {
                int m = 0;
                for (int k = 0; k < expNums[j]; k++)
                {
                    double p = random.NextDouble();
                    double x = (xmax - xmin) * p + xmin;

                    p = random.NextDouble();
                    double y = (ymax - ymin) * p + ymin;

                    if (Math.Pow((x - x0), 2) + Math.Pow((y - y0), 2) < Math.Pow(r0, 2))
                    {
                        m++;
                    }
                }
                results[i][j] = (double)m / expNums[j] * 4;
            }
        }
        return results;
    }

    // Генерация случайных чисел и расчет интеграла
    static double[][] CalculateIntegral(int[] expNums, double a, double b, int numSeries)
    {
        double xmin = a;
        double xmax = b;
        double ymin = 0;
        double ymax = Math.Pow(b, 3) + 1;
        double[][] results = new double[numSeries][];
        Random random = new Random();

        for (int i = 0; i < numSeries; i++)
        {
            results[i] = new double[expNums.Length];
            for (int j = 0; j < expNums.Length; j++)
            {
                int m = 0;
                for (int k = 0; k < expNums[j]; k++)
                {
                    double p = random.NextDouble();
                    double x = (xmax - xmin) * p + xmin;

                    p = random.NextDouble();
                    double y = (ymax - ymin) * p + ymin;

                    if (Math.Pow(x, 3) + 1 > y)
                    {
                        m++;
                    }
                }
                results[i][j] = (double)m / expNums[j] * (b - a) * (Math.Pow(b, 3) + 1);
            }
        }
        return results;
    }

    // Расчет средних значений и отклонений
    static double[] CalculateAverage(double[][] series)
    {
        double[] averages = new double[series[0].Length];
        for (int i = 0; i < series[0].Length; i++)
        {
            double sum = 0;
            for (int j = 0; j < series.Length; j++)
            {
                sum += series[j][i];
            }
            averages[i] = sum / series.Length;
        }
        return averages;
    }

    // Расчет стандартного отклонения по каждой серии данных
    static double[] CalculateStandardDeviation(double[][] series, double[] averages)
    {
        double[] deviations = new double[series[0].Length];
        for (int i = 0; i < series[0].Length; i++)
        {
            double sum = 0;
            for (int j = 0; j < series.Length; j++)
            {
                sum += Math.Pow(series[j][i] - averages[i], 2);
            }
            deviations[i] = Math.Sqrt(sum / series.Length);
        }
        return deviations;
    }

    // Расчет отклонений от точного значения
    static double[] CalculateDeviation(double[][] results, double referenceValue)
    {
        double[] deviations = new double[results[0].Length];
        for (int i = 0; i < results[0].Length; i++)
        {
            double sum = 0;
            for (int j = 0; j < results.Length; j++)
            {
                sum += results[j][i];
            }
            double average = sum / results.Length;
            deviations[i] = Math.Abs((average - referenceValue) / referenceValue);
        }
        return deviations;
    }

    // Вывод таблицы с результатами
    static void PrintTable(double[][] data, string title)
    {
        using (var writer = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = false })
        {
            writer.WriteLine(title);
            writer.WriteLine(new string('-', 60));
            writer.WriteLine($"{"Эксперименты",-15} | {"Серия 1",-15} | {"Серия 2",-15} | {"Серия 3",-15} | {"Серия 4",-15} | {"Серия 5",-15}");
            writer.WriteLine(new string('-', 60));

            for (int i = 0; i < data[0].Length; i++)
            {
                writer.Write($"{expNums[i],-15}");
                for (int j = 0; j < data.Length; j++)
                {
                    writer.Write($" {data[j][i],-15:F6} ");
                }
                writer.WriteLine();
            }
            writer.WriteLine(new string('-', 60));
            writer.Flush();
        }
    }

    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        // Задание 1-3: расчет π
        Console.WriteLine("Расчет значения числа Pi:");
        double[][] piResults = CalculatePi(expNums, 5, x0, y0, r0);
        PrintTable(piResults, "Результаты расчета числа Pi");

        double[] piAverages = CalculateAverage(piResults);
        double[] piDeviations = CalculateStandardDeviation(piResults, piAverages);
        double[] piDeviationPercentages = CalculateDeviation(piResults, Math.PI);

        Console.WriteLine("");
        Console.WriteLine("Отклонение для числа Pi:");
        Console.WriteLine(new string('-', 100));
        Console.WriteLine($"{"Эксперименты",-15} | {"Среднее значение",-15} | {"Отклонение",-15}");
        Console.WriteLine(new string('-', 100));

        for (int i = 0; i < expNums.Length; i++)
        {
            Console.WriteLine($"{expNums[i],-15} | {piAverages[i],-15:F6} | {piDeviations[i],-15:F6}");
        }
        Console.WriteLine(new string('-', 100));
        Console.WriteLine("Процентное отклонение от Pi:");
        for (int i = 0; i < expNums.Length; i++)
        {
            Console.WriteLine($"Для {expNums[i]} экспериментов: {piDeviationPercentages[i]:F6}");
        }

        // Задание 4: расчет интеграла
        Console.WriteLine("\nРасчет значения интеграла:");
        double[][] integralResults = CalculateIntegral(expNums.Take(expNums.Length - 1).ToArray(), a, b, 5);
        PrintTable(integralResults, "Результаты расчета интеграла");

        double[] integralAverages = CalculateAverage(integralResults);
        double[] integralDeviations = CalculateStandardDeviation(integralResults, integralAverages);
        double[] integralDeviationPercentages = CalculateDeviation(integralResults, 6);

        Console.WriteLine("Отклонение для интеграла:");
        Console.WriteLine(new string('-', 100));
        Console.WriteLine($"{"Эксперименты",-15} | {"Среднее значение",-15} | {"Отклонение",-15}");
        Console.WriteLine(new string('-', 100));

        for (int i = 0; i < expNums.Length - 1; i++)
        {
            Console.WriteLine($"{expNums[i],-15} | {integralAverages[i],-15:F6} | {integralDeviations[i],-15:F6}");
        }
        Console.WriteLine(new string('-', 100));
        Console.WriteLine("Процентное отклонение от точного значения интеграла:");
        for (int i = 0; i < expNums.Length - 1; i++)
        {
            Console.WriteLine($"Для {expNums[i]} экспериментов: {integralDeviationPercentages[i]:F6}");
        }
        Console.ReadKey();
    }
}
