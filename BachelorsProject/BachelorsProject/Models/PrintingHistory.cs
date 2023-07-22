namespace BachelorsProject.Models
{
    public class PrintingHistory
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string FileName { get; set; }
        public string PrinterName { get; set; }
        public double PlasticWeight { get; set; }
        public DateTime StartedPrintingAt { get; set; }
        public DateTime FinishedPrintingAt { get; set; }

        public PrintingHistory()
        {
            FileName = string.Empty;
            PrinterName = string.Empty;
            StartedPrintingAt = DateTime.UtcNow;
            FinishedPrintingAt = DateTime.UtcNow;
            UserName = string.Empty;
            PlasticWeight = 0;
        }
    }
}
