namespace API.Errors
{
    public class APIExceptions
    {
   
        public APIExceptions(int statusCode, string details,string message) 
        {
                this.StatusCode = statusCode;
                this.Details = details;
                this.Message=message;
   
        }
        public int StatusCode { get; set; }
        public string Message { get; set; }

        public string Details { get; set; }

    }
}