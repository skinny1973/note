using NoteManager.Models;

namespace NoteManager.Models;

/// <summary>
/// Data structure for auto-save functionality
/// </summary>
public class SaveData
{
    /// <summary>
    /// Collection of saved notes
    /// </summary>
    public List<Note> Notes { get; set; } = new();
    
    /// <summary>
    /// Next available ID for new notes
    /// </summary>
    public int NextId { get; set; } = 1;
    
    /// <summary>
    /// Timestamp of last save operation
    /// </summary>
    public DateTime LastSaved { get; set; }
}
