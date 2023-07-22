namespace BachelorsProject.Models
{
    public class Files
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string UserName { get; set; }
        public double? Volume { get; set; }
        public double? PrintingTime { get; set; }
        public string? FilePath { get; set; }


        public Files()
        {
            FileName = string.Empty;
            UserName = string.Empty;
        }
    }
}
