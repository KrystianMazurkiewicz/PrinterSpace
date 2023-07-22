namespace BachelorsProject.Models
{
    public class Printer
    {
        public int Id { get; set; }
        public string PrinterName { get; set; }
        public string IP { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
        public Printer ()
        {
            PrinterName = string.Empty;
            IP = string.Empty;
            ApiKey = string.Empty;
            ApiSecret = string.Empty;
        }
    }
}
