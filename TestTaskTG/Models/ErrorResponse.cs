using Newtonsoft.Json;

namespace TestTaskTG.Models
{
    public class ErrorResponse
    {
        public ErrorResponse(string err) 
        { 
            Error = err; 
        }

        [JsonProperty("error")]
        private string Error { get; set; }
    }
}