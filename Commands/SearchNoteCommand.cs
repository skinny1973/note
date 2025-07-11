using NoteManager.Interfaces;
using NoteManager.Services;

namespace NoteManager.Commands;

/// <summary>
/// Command for searching notes
/// </summary>
public class SearchNoteCommand : ICommand
{
    private readonly NoteService _noteService;
    
    public SearchNoteCommand(NoteService noteService)
    {
        _noteService = noteService;
    }
    
    public string Name => "search";
    public string Description => "Search notes by title or content (usage: search \"term\" OR just search for interactive mode)";
    
    public void Execute(string[]? args = null)
    {
        string searchTerm;
        
        if (args != null && args.Length > 0)
        {
            searchTerm = args[0].Trim('"');
        }
        else
        {
            Console.Write("Enter search term: ");
            searchTerm = Console.ReadLine() ?? "";
        }
        
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var results = _noteService.SearchNotes(searchTerm);
            
            if (results.Any())
            {
                Console.WriteLine($"Found {results.Count} note(s) matching '{searchTerm}':");
                foreach (var note in results.OrderByDescending(n => n.CreatedAt))
                {
                    Console.WriteLine($"[{note.Id}] {note.Title} - {note.CreatedAt:yyyy-MM-dd HH:mm}");
                    Console.WriteLine($"    {note.Content}");
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine($"No notes found matching '{searchTerm}'.");
            }
        }
        else
        {
            Console.WriteLine("Search term cannot be empty.");
        }
    }
}
