using System.Globalization;
using System.Net;
using Lab_10.Structures;
using Lab_10.Classes;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

public class Program
{
    public static void Main(string[] args)
    {
        // Чтение тикеров из файла
        string[] tickers = File.ReadAllLines("C:\\Users\\mikha\\source\\repos\\Lab_10\\Lab_10\\ticker.txt");

        using (var context = new TickersContext())
        {
            // Заполнение базы данных тикерами и соответствующими данными акций
            FillDb(context, tickers);
            // Обновление состояния на сегодняшний день на основе данных акций
            UpdateTodayCondition(context);

            // Ввод пользователем тикеров
            Console.WriteLine("Enter a ticker symbol:");
            var tickerSymbol = Console.ReadLine();

            // Обработка ввода пользователем до ввода пустой строки
            while (tickerSymbol != "")
            {
                // Поиск сущности Tickers для введенного символа тикера
                var ticker = context.Tickers.FirstOrDefault(t => t.TickerSymbol == tickerSymbol);
                if (ticker == null)
                {
                    Console.WriteLine("Ticker symbol not found.");
                    return;
                }

                // Поиск сущности TodayCondition для введенного символа тикера
                var todayCondition = context.TodayCondition.FirstOrDefault(c => c.TickerId == ticker.Id);
                if (todayCondition == null)
                {
                    Console.WriteLine("No data available for today.");
                    return;
                }

                // Вывод результата пользователю
                Console.WriteLine($"Price for {tickerSymbol} has {todayCondition.State} today.");

                // Подсказка пользователю для ввода нового тикера или выхода
                Console.WriteLine("Enter new ticker symbol or press Enter for exit:");
                tickerSymbol = Console.ReadLine();
            }
        }
    }

    // Метод для обновления состояния на сегодня на основе данных акций
    public static void UpdateTodayCondition(TickersContext context)
    {
        // Получение всех цен акций из базы данных
        var tickerPrices = context.Prices.ToList();
        string state;
        // Итерация по каждой цене акции
        foreach (var tickerPrice in tickerPrices)
        {
            if (tickerPrice.PriceBefore > tickerPrice.PriceAfter)
            {
                // Определение, увеличилась ли или уменьшилась цена акции
                state = "decreased";
            }
            else
            {
                state = "increased";
            }

            var todayCondition = new TodayCondition
            {
                TickerId = tickerPrice.TickerId,
                State = state
            };

            // Добавление новой сущности TodayCondition в контекст и сохранение изменений
            context.TodayCondition.Add(todayCondition);
            context.SaveChanges();
        }
    }

    // Метод для заполнения базы данных тикерами и данными акций
    private static void FillDb(TickersContext context, string[] tickers)
    {
        // Итерация по каждому тикеру
        for (int i = 0; i < tickers.Length; ++i)
        {
            // Создание новой сущности Tickers с текущим тикером
            var ticker = new Tickers
            {
                TickerSymbol = tickers[i]
            };
            // Добавление новой сущности Tickers в контекст и сохранение изменений
            context.Tickers.Add(ticker);
            context.SaveChanges();

            // Получение данных акций с использованием метода GetStockData
            var arr = GetStockData(tickers[i]);

            // Создание новой сущности Prices с данными акций и соответствующим TickerId
            var temp = new Prices
            {
                TickerId = context.Tickers.FirstOrDefault(t => t.TickerSymbol == ticker.TickerSymbol).Id,
                PriceBefore = arr[0],
                PriceAfter = arr[1],
                DateBefore = DateTimeOffset.Now.Date,
                DateAfter = DateTimeOffset.Now.AddDays(1)
            };
            // Добавление новой сущности Prices в контекст и сохранение изменений
            context.Prices.Add(temp);
            context.SaveChanges();
        }
    }

    // Метод для получения данных акций с сайта Yahoo Finance
    static double[] GetStockData(string ticker)
    {
        // Расчет временных меток за последние 24 часа
        long startTimestamp = DateTimeOffset.UtcNow.AddDays(-1).ToUnixTimeSeconds();
        long endTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        // Составление URL для API Yahoo Finance
        string url = $"https://query1.finance.yahoo.com/v7/finance/download/{ticker}?period1={startTimestamp}&period2={endTimestamp}&interval=1d&events=history&includeAdjustedClose=true";

        // Использование WebClient для загрузки данных по URL
        using (WebClient client = new WebClient())
        {
            try
            {
                // Загрузка данных
                string data = client.DownloadString(url);

                // Разбор загруженных данных и расчет средних цен
                string[] lines = data.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                double[] prices = new double[lines.Length - 1];
                for (int i = 1; i < lines.Length; i++)
                {
                    var columns = lines[i].Split(',');
                    var high = double.Parse(columns[2], CultureInfo.InvariantCulture);
                    var low = double.Parse(columns[3], CultureInfo.InvariantCulture);
                    prices[i - 1] = (high + low) / 2;
                }

                return prices;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Произошла ошибка при получении данных для акции {ticker}: {e.Message}");
                return null;
            }
        }
    }
}