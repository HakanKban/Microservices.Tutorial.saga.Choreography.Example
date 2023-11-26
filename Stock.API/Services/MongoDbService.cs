using MongoDB.Driver;

namespace Stock.API.Services
{
    public class MongoDbService
    {
        readonly IMongoDatabase database;
        public MongoDbService(IConfiguration configuration)
        {
            MongoClient mongoClient = new(configuration.GetConnectionString("MongoDB"));
                
        }
    }
}
