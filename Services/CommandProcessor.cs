using System.Text;
using NoteManager.Commands;
using NoteManager.Interfaces;
using NoteManager.Services;

namespace NoteManager.Services;

/// <summary>
/// Command processor that handles command registration and execution
/// </summary>
public class CommandProcessor
{
    private readonly Dictionary<string, ICommand> _commands;
    private readonly NoteService _noteService;
    
    public CommandProcessor()
    {
        _noteService = new NoteService();
        _commands = new Dictionary<string, ICommand>();
        
        // Register all available commands
        RegisterCommand(new AddNoteCommand(_noteService));
        RegisterCommand(new ListNotesCommand(_noteService));
        RegisterCommand(new DeleteNoteCommand(_noteService));
        RegisterCommand(new ExitCommand(_noteService));
        RegisterCommand(new HelpCommand(_commands));
        RegisterCommand(new SearchNoteCommand(_noteService));
        RegisterCommand(new UpdateNoteCommand(_noteService));
        RegisterCommand(new ExportNotesCommand(_noteService));
        RegisterCommand(new ImportNotesCommand(_noteService));
    }
    
    /// <summary>
    /// Register a new command
    /// </summary>
    /// <param name="command">Command to register</param>
    public void RegisterCommand(ICommand command)
    {
        _commands[command.Name] = command;
    }
    
    /// <summary>
    /// Save session data and exit application
    /// </summary>
    public void SaveAndExit()
    {
        _noteService.SaveAutoSaveFile();
        Console.WriteLine("Goodbye!");
        Environment.Exit(0);
    }
    
    /// <summary>
    /// Process a command input string
    /// </summary>
    /// <param name="input">Command input string</param>
    public void ProcessCommand(string input)
    {
        var parts = ParseCommandLine(input);
        if (parts.Length == 0) return;
        
        var commandName = parts[0].ToLower();
        var args = parts.Length > 1 ? parts[1..] : null;
        
        if (_commands.TryGetValue(commandName, out var command))
        {
            command.Execute(args);
        }
        else
        {
            Console.WriteLine($"Unknown command: {commandName}. Type 'help' for available commands.");
        }
    }
    
    /// <summary>
    /// Parse command line input into parts, respecting quoted strings
    /// </summary>
    /// <param name="input">Input string to parse</param>
    /// <returns>Array of command parts</returns>
    private static string[] ParseCommandLine(string input)
    {
        var parts = new List<string>();
        var current = new StringBuilder();
        bool inQuotes = false;
        
        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            
            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ' ' && !inQuotes)
            {
                if (current.Length > 0)
                {
                    parts.Add(current.ToString());
                    current.Clear();
                }
            }
            else
            {
                current.Append(c);
            }
        }
        
        if (current.Length > 0)
        {
            parts.Add(current.ToString());
        }
        
        return parts.ToArray();
    }
}
