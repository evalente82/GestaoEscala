namespace GestaoEscalaPermutas.Server.Settings
{
    public class RecaptchaSettings
    {
        public string SiteKey { get; set; }
        public string GoogleCloudProjectId { get; set; }
        public double MinScore { get; set; }
        public string ExpectedAction { get; set; }
    }
}
