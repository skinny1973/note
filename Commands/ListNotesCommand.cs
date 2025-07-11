using NoteManager.Interfaces;
using NoteManager.Services;

namespace NoteManager.Commands;

/// <summary>
/// Command for listing all notes
/// </summary>
public class ListNotesCommand : ICommand
{
    private readonly NoteService _noteService;
    
    public ListNotesCommand(NoteService noteService)
    {
        _noteService = noteService;
    }
    
    public string Name => "list";
    public string Description => "List all notes";
    
    public void Execute(string[]? args = null)
    {
        _noteService.ListNotes();
    }
}
