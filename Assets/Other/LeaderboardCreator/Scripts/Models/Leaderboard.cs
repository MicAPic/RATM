namespace Dan.Models
{
    [System.Serializable]
    public class Leaderboard
    {
        public string Name { get; set; }
        public string PublicKey { get; set; }
        public string SecretKey { get; set; }
        public bool IsProfanityEnabled { get; set; }
        public bool IsSuperLeaderboard { get; set; }
    }
}