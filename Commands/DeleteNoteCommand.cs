using NoteManager.Interfaces;
using NoteManager.Services;

namespace NoteManager.Commands;

/// <summary>
/// Command for deleting notes
/// </summary>
public class DeleteNoteCommand : ICommand
{
    private readonly NoteService _noteService;
    
    public DeleteNoteCommand(NoteService noteService)
    {
        _noteService = noteService;
    }
    
    public string Name => "delete";
    public string Description => "Delete a note by ID (usage: delete)";
    
    public void Execute(string[]? args = null)
    {
        // If there's a parameter, use it; otherwise ask interactively
        int id;
        if (args != null && args.Length > 0 && int.TryParse(args[0], out id))
        {
            _noteService.DeleteNote(id);
        }
        else
        {
            Console.Write("Enter note ID to delete: ");
            var input = Console.ReadLine();
            
            if (int.TryParse(input, out id))
            {
                _noteService.DeleteNote(id);
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }
        }
    }
}
