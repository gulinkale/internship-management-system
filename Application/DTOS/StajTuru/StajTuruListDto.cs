namespace Application.DTOs
{
    public class StajTuruListDto
    {
        public int ID { get; set; }
        public string Ad { get; set; } = string.Empty;
        public int StajSayisi { get; set; }          // Nav: Stajlar.Count
    }
}
