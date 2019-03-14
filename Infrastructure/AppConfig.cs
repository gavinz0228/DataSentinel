namespace DataSentinel.Infrastructure{
    public class AppConfig{
        public string ConnectionString{get;set;}
        public string DatabaseName{get;set;}
        public byte[] SecretKey {get;set;}
        public string LoginUserNameKey{get; set;}
        public string LoginPasswordKey{get; set;}
        public string TokenSecretKey{get; set;}
    }
}