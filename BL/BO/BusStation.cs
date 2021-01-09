namespace BO
{
    public class BusStation
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public override string ToString() => $"Code:{Code}, Name:{Name}";

    }
}