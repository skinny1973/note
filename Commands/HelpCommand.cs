using NoteManager.Interfaces;
using NoteManager.Services;

namespace NoteManager.Commands;

/// <summary>
/// Command for displaying help information
/// </summary>
public class HelpCommand : ICommand
{
    private readonly Dictionary<string, ICommand> _commands;
    
    public HelpCommand(Dictionary<string, ICommand> commands)
    {
        _commands = commands;
    }
    
    public string Name => "help";
    public string Description => "Show available commands";
    
    public void Execute(string[]? args = null)
    {
        Console.WriteLine("Available commands:");
        foreach (var cmd in _commands.Values)
        {
            Console.WriteLine($"  {cmd.Name} - {cmd.Description}");
        }
    }
}
