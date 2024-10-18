namespace SAPB1SLayerWebAPI.Models
{
    public class GW_API_Auth_Response
    {
        public int? Code { get; set; }
        public MessageProp Message { get; set; } = new();
        public string? Version { get; set; }
        public int? SessionTimeout { get; set; }
    }

    public class MessageProp
    {
        public string? Lang { get; set; } 
        public string? Value { get; set; } 
    }
}
