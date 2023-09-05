using Microsoft.Extensions.Options;
using MongoDB.Driver;
using UserApi.Models;

namespace UserApi.Services;

public class UserService
{
    private readonly IMongoCollection<User> _userCollection;

    public UserService(IOptions<MongoDBSettings> mongoDBSettings)
    {
        MongoClient mongoClient = new MongoClient(mongoDBSettings.Value.ConnectionString);
        IMongoDatabase mongoDatabase = mongoClient.GetDatabase(mongoDBSettings.Value.DatabaseName);
        _userCollection = mongoDatabase.GetCollection<User>(mongoDBSettings.Value.CollectionName);
    }

    public async Task<User?> GetAsync(string email, string password) => await _userCollection.Find(user => user.EmailConfirmed == email && user.PasswordHash == password).FirstOrDefaultAsync();

    public async Task<User?> GetAsync(string id) => await _userCollection.Find(user => user.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(UserRegister newUser)
    {
        User _newUser = new()
        {
            Username = newUser.Username,
            FirstName = newUser.FirstName,
            LastName = newUser.LastName,
            EmailConfirmed = newUser.Email,
            PasswordHash = newUser.Password
        };

        await _userCollection.InsertOneAsync(_newUser);
    }

    public async Task UpdateAsync(string id, User updatedUser) => await _userCollection.ReplaceOneAsync(user => user.Id == id, updatedUser);

    public async Task RemoveAsync(string id) => await _userCollection.DeleteOneAsync(user => user.Id == id);
}