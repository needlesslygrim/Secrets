using System.Data;
using System.Text.Json;
using Secrets.API.Models;
using MySqlConnector;

WebApplicationBuilder _builder = WebApplication.CreateBuilder(args);
_builder.Services.AddCors(policyBuilder => policyBuilder.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

WebApplication _app = _builder.Build();

string _connectionString = File.ReadAllText(@"/home/erickth/Documents/Code/CSharp/connectionstring.txt").Trim();

_app.MapGet("/secrets/passphrase/{passphrase}", async (string passphrase) =>
{
	MySqlConnection _connection = new MySqlConnection(_connectionString);
	await _connection.OpenAsync();

	MySqlCommand _command = new MySqlCommand($"SELECT `Passphrase`, `Secret`, `BackupDate`, `FavouriteLetter`, `FavouriteNum`  FROM Secrets.prod WHERE Passphrase = '{passphrase}'", _connection);
	return await ReadDb(_command);
});

_app.MapGet("/secrets/backup-data/{date}/{favouriteDigit}/{favouriteLetter}", async (string date, int favouriteDigit, char favouriteLetter) =>
{
	MySqlConnection _connection = new MySqlConnection(_connectionString);
	await _connection.OpenAsync();
	
	MySqlCommand _command = new MySqlCommand($"SELECT `Passphrase`, `Secret`, `BackupDate`, `FavouriteLetter`, `FavouriteNum`  FROM Secrets.prod WHERE BackupDate = '{date}' AND FavouriteLetter = '{favouriteLetter}' AND FavouriteNum = '{favouriteDigit}'", _connection);
	return await ReadDb(_command);
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

async Task<IResult> ReadDb(MySqlCommand command)
{
	MySqlDataReader _reader = await command.ExecuteReaderAsync();
	bool _success = await _reader.ReadAsync();
	if (!_success) { return Results.NotFound(); }

	SecretRecord _record = new SecretRecord(_reader.GetString(0), _reader.GetString(1), _reader.GetDateTime(2), _reader.GetChar(3), _reader.GetInt16(4));

	return Results.Ok(_record);
}

_app.UseCors();
_app.Run();