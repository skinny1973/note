using NoteManager.Interfaces;
using NoteManager.Services;

namespace NoteManager.Commands;

/// <summary>
/// Command for adding new notes
/// </summary>
public class AddNoteCommand : ICommand
{
    private readonly NoteService _noteService;
    
    public AddNoteCommand(NoteService noteService)
    {
        _noteService = noteService;
    }
    
    public string Name => "add";
    public string Description => "Add a new note (usage: add \"title\" \"content\" OR just add for interactive mode)";
    
    public void Execute(string[]? args = null)
    {
        string title, content;
        
        // Strict parameter checking
        if (args != null && args.Length > 0)
        {
            if (args.Length == 2)
            {
                // Exactly 2 parameters: use parameter mode
                title = args[0].Trim('"');
                content = args[1].Trim('"');
            }
            else if (args.Length == 1)
            {
                // Only 1 parameter: error, need both
                Console.WriteLine("Error: Missing content parameter.");
                Console.WriteLine("Usage: add \"title\" \"content\" OR just 'add' for interactive mode");
                return;
            }
            else
            {
                // More than 2 parameters: error
                Console.WriteLine($"Error: Too many parameters ({args.Length}). Expected exactly 2.");
                Console.WriteLine("Usage: add \"title\" \"content\" OR just 'add' for interactive mode");
                return;
            }
        }
        else
        {
            // No parameters: interactive mode
            Console.Write("Enter note title: ");
            title = Console.ReadLine() ?? "";
            
            Console.Write("Enter note content: ");
            content = Console.ReadLine() ?? "";
        }
        
        if (!string.IsNullOrWhiteSpace(title))
        {
            _noteService.AddNote(title, content);
        }
        else
        {
            Console.WriteLine("Title cannot be empty.");
        }
    }
}
