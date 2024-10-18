namespace SAPB1SLayerWebAPI.Utils
{
    public class UserIDs
    {
        public static FinalPosting FINAL_POSTING { get; set; } = new();
        public static int MANAGER { get; set; } = 1;
    }

    public class FinalPosting
    {
        public List<int> SO { get; set; } = [86];
    }
}
