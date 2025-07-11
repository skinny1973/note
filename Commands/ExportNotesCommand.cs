using NoteManager.Interfaces;
using NoteManager.Services;

namespace NoteManager.Commands;

/// <summary>
/// Command for exporting notes to file
/// </summary>
public class ExportNotesCommand : ICommand
{
    private readonly NoteService _noteService;
    
    public ExportNotesCommand(NoteService noteService)
    {
        _noteService = noteService;
    }
    
    public string Name => "export";
    public string Description => "Export notes to JSON file (usage: export \"filename.json\" OR just export for interactive mode)";
    
    public void Execute(string[]? args = null)
    {
        string filePath;
        
        if (args != null && args.Length > 0)
        {
            filePath = args[0].Trim('"');
        }
        else
        {
            Console.Write("Enter export file path (e.g., notes.json): ");
            filePath = Console.ReadLine() ?? "";
        }
        
        if (!string.IsNullOrWhiteSpace(filePath))
        {
            if (!filePath.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            {
                filePath += ".json";
            }
            _noteService.ExportNotes(filePath);
        }
        else
        {
            Console.WriteLine("File path cannot be empty.");
        }
    }
}
