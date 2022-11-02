using System.Text.Json.Serialization;

namespace Secrets.API.Models;

public class SecretRecord
{
	[JsonInclude] public string Passphrase;
	[JsonInclude] public string Secret;
	[JsonInclude] public DateTime BackupDate;
	[JsonInclude] public char FavouriteLetter;
	[JsonInclude] public short FavouriteNum;
	
	public SecretRecord(string passphrase, string secret, DateTime backupDate, char favouriteLetter, short favouriteNum)
	{
		Passphrase = passphrase;
		Secret = secret;
		BackupDate = backupDate;
		FavouriteLetter = favouriteLetter;
		FavouriteNum = favouriteNum;
	}
}