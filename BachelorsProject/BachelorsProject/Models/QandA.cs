namespace BachelorsProject.Models
{
    public class QandA
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public string Answers { get; set; }

        public QandA()
        {
            Question = string.Empty;
            Answers = string.Empty;
        }
    }
}
