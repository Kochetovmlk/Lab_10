namespace Lab_10.Structures
{
    // Класс Tickers представляет информацию о тикере (акции).
    public class Tickers
    {
        // Уникальный идентификатор тикера.
        public int Id { get; set; }

        // Символьное обозначение тикера (например, "AAPL" для акции Apple).
        public string TickerSymbol { get; set; }
    }
}