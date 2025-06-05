using System.Drawing; // Для PointF

namespace GraphColoringApp // <<< ЗАМІНІТЬ НА ПРОСТІР ІМЕН ВАШОГО ПРОЕКТУ
{
    public class State
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Color { get; set; } = -1; // -1 означає нерозфарбований

        // Для графічної візуалізації
        public PointF Position { get; set; } // Позиція для малювання на формі

        public State(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}