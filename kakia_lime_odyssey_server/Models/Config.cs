namespace kakia_lime_odyssey_server.Models;

public class Config
{
	public string ServerIP { get; set; } = default!;
	public int Port { get; set; }
	public bool Crypto { get; set; }
}
