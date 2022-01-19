namespace Theatre.DataProcessor.ExportDto
{
    public class TheatreExportDto
    {
        public string Name { get; set; }
        public int Halls { get; set; }
        public float TotalIncome { get; set; }
        public TicketExportDto[] Tickets { get; set; }
    }
}
