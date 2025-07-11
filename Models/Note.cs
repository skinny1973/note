namespace NoteManager.Models;

/// <summary>
/// Represents a note entity
/// </summary>
public class Note
{
    /// <summary>
    /// Unique identifier for the note
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Title of the note
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Content/body of the note
    /// </summary>
    public string Content { get; set; } = string.Empty;
    
    /// <summary>
    /// Timestamp when the note was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
