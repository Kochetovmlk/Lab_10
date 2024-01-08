namespace Lab_10.Structures;

public class Prices
{
    public int Id { get; set; }
    public int TickerId { get; set; }
    public double PriceBefore { get; set; }
    public double PriceAfter { get; set; }
    public DateTimeOffset DateBefore { get; set; }
    public DateTimeOffset DateAfter { get; set; }
}