namespace LoadDWVentas.Data.Results
{
    public class LoadWHVentas1
    {
        public int TotalRecordsProcessed { get; set; }
        public int SuccessfulRecords { get; set; }
        public int FailedRecords { get; set; }
        public string Message { get; set; }

        public LoadWHVentas1(int totalRecords, int successful, int failed, string message)
        {
            TotalRecordsProcessed = totalRecords;
            SuccessfulRecords = successful;
            FailedRecords = failed;
            Message = message;
        }
    }
}
