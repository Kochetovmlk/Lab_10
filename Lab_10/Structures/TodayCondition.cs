namespace Lab_10.Structures
{
    // Класс TodayCondition представляет состояние акции на текущий день.
    public class TodayCondition
    {
        // Уникальный идентификатор состояния акции.
        public int Id { get; set; }

        // Идентификатор тикера (акции), связанный с этим состоянием.
        public int TickerId { get; set; }

        // Состояние акции на текущий день (например, "увеличилась" или "уменьшилась").
        public string State { get; set; }
    }
}