using NoteManager.Interfaces;
using NoteManager.Services;

namespace NoteManager.Commands;

/// <summary>
/// Command for importing notes from file
/// </summary>
public class ImportNotesCommand : ICommand
{
    private readonly NoteService _noteService;
    
    public ImportNotesCommand(NoteService noteService)
    {
        _noteService = noteService;
    }
    
    public string Name => "import";
    public string Description => "Import notes from JSON file (usage: import \"filename.json\" OR just import for interactive mode)";
    
    public void Execute(string[]? args = null)
    {
        string filePath;
        
        if (args != null && args.Length > 0)
        {
            filePath = args[0].Trim('"');
        }
        else
        {
            Console.Write("Enter import file path: ");
            filePath = Console.ReadLine() ?? "";
        }
        
        if (!string.IsNullOrWhiteSpace(filePath))
        {
            _noteService.ImportNotes(filePath);
        }
        else
        {
            Console.WriteLine("File path cannot be empty.");
        }
    }
}
