using System;

public class NetworkHostMessages
{
	private static readonly char seperator = ':';
	private static readonly string ExpectedNumberOfPlayers = "ExpectedNumberOfPlayers";
	
	
	public static string GenerateHostComment(int expectedNumOfPlayers) {
		return ExpectedNumberOfPlayers + seperator + expectedNumOfPlayers;
	}
}