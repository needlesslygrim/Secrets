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

_app.MapPost("/secrets", async (SecretRecord record) =>
{
	MySqlConnection _connection = new MySqlConnection(_connectionString);
	await _connection.OpenAsync();

	await using MySqlCommand _command = _connection.CreateCommand();
	_command.CommandText = @"INSERT INTO Secrets.prod (`Passphrase`, `Secret`, `BackupDate`, `FavouriteLetter`, `FavouriteNum`) VALUES (@passphrase, @secret, @backupdate, @favouriteletter, @favouritenum);";


	_command.Parameters.Add(new MySqlParameter
	{
		ParameterName = "@passphrase",
		DbType = DbType.String,
		Value = record.Passphrase,
	});

	_command.Parameters.Add(new MySqlParameter
	{
		ParameterName = "@secret",
		DbType = DbType.String,
		Value = record.Secret,
	});

	_command.Parameters.Add(new MySqlParameter
	{
		ParameterName = "@backupdate",
		DbType = DbType.Date,
		Value = record.BackupDate,
	});

	_command.Parameters.Add(new MySqlParameter
	{
		ParameterName = "@favouriteletter",
		DbType = DbType.StringFixedLength,
		Value = record.FavouriteLetter,
	});

	_command.Parameters.Add(new MySqlParameter
	{
		ParameterName = "@favouritenum",
		DbType = DbType.Int16,
		Value = record.FavouriteNum,
	});

	await _command.ExecuteNonQueryAsync();

	return Results.Created($"/secrets/make/{record.Passphrase}", record);
});

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