namespace DataSentinel.Infrastructure{
    public class AppConfig{
        public string ConnectionString{get;set;}
        public string DatabaseName{get;set;}
        public byte[] SecretKey {get;set;}
    }
}