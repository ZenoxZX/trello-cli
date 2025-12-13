namespace TrelloCli.Commands;

public interface ICommand
{
    Task ExecuteAsync(string[] args);
}
