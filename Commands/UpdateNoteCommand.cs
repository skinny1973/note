using NoteManager.Interfaces;
using NoteManager.Services;

namespace NoteManager.Commands;

/// <summary>
/// Command for updating existing notes
/// </summary>
public class UpdateNoteCommand : ICommand
{
    private readonly NoteService _noteService;
    
    public UpdateNoteCommand(NoteService noteService)
    {
        _noteService = noteService;
    }
    
    public string Name => "update";
    public string Description => "Update a note (usage: update id \"new title\" \"new content\" OR just update for interactive mode)";
    
    public void Execute(string[]? args = null)
    {
        int id;
        string? newTitle = null, newContent = null;
        
        if (args != null && args.Length >= 1 && int.TryParse(args[0], out id))
        {
            if (args.Length >= 2) newTitle = args[1].Trim('"');
            if (args.Length >= 3) newContent = args[2].Trim('"');
        }
        else
        {
            Console.Write("Enter note ID to update: ");
            var input = Console.ReadLine();
            
            if (!int.TryParse(input, out id))
            {
                Console.WriteLine("Invalid ID format.");
                return;
            }
            
            Console.Write("Enter new title (leave empty to keep current): ");
            newTitle = Console.ReadLine();
            
            Console.Write("Enter new content (leave empty to keep current): ");
            newContent = Console.ReadLine();
        }
        
        _noteService.UpdateNote(id, newTitle, newContent);
    }
}
