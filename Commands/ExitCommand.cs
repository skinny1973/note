using NoteManager.Interfaces;
using NoteManager.Services;

namespace NoteManager.Commands;

/// <summary>
/// Command for exiting the application
/// </summary>
public class ExitCommand : ICommand
{
    private readonly NoteService _noteService;
    
    public ExitCommand(NoteService noteService)
    {
        _noteService = noteService;
    }
    
    public string Name => "exit";
    public string Description => "Exit the application";
    
    public void Execute(string[]? args = null)
    {
        Console.WriteLine("Saving session data...");
        _noteService.SaveAutoSaveFile();
        Console.WriteLine("Goodbye!");
        Environment.Exit(0);
    }
}
