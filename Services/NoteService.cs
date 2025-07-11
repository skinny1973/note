using System.Text.Json;
using NoteManager.Models;

namespace NoteManager.Services;

/// <summary>
/// Service for managing notes with auto-save functionality
/// </summary>
public class NoteService
{
    private readonly List<Note> _notes = new();
    private int _nextId = 1;
    private readonly string _autoSaveFilePath;
    
    public NoteService()
    {
        // Get executable name without extension for auto-save file
        var executableName = Path.GetFileNameWithoutExtension(System.Diagnostics.Process.GetCurrentProcess().ProcessName);
        _autoSaveFilePath = $"{executableName}.json";
        
        // Automatically load data from previous session if exists
        LoadAutoSaveFile();
    }
    
    /// <summary>
    /// Load notes from auto-save file
    /// </summary>
    private void LoadAutoSaveFile()
    {
        try
        {
            if (File.Exists(_autoSaveFilePath))
            {
                var json = File.ReadAllText(_autoSaveFilePath);
                var savedData = JsonSerializer.Deserialize<SaveData>(json);
                
                if (savedData?.Notes != null)
                {
                    _notes.AddRange(savedData.Notes);
                    _nextId = savedData.NextId;
                    Console.WriteLine($"✓ Loaded {savedData.Notes.Count} note(s) from previous session ({_autoSaveFilePath})");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Could not load previous session data: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Save current notes to auto-save file
    /// </summary>
    public void SaveAutoSaveFile()
    {
        try
        {
            var saveData = new SaveData
            {
                Notes = _notes,
                NextId = _nextId,
                LastSaved = DateTime.Now
            };
            
            var json = JsonSerializer.Serialize(saveData, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
            File.WriteAllText(_autoSaveFilePath, json);
            Console.WriteLine($"✓ Session data saved to {_autoSaveFilePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving session data: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Add a new note
    /// </summary>
    /// <param name="title">Note title</param>
    /// <param name="content">Note content</param>
    public void AddNote(string title, string content)
    {
        var note = new Note
        {
            Id = _nextId++,
            Title = title,
            Content = content,
            CreatedAt = DateTime.Now
        };
        _notes.Add(note);
        Console.WriteLine($"Note '{title}' added successfully with ID: {note.Id}");
    }
    
    /// <summary>
    /// List all notes
    /// </summary>
    public void ListNotes()
    {
        if (!_notes.Any())
        {
            Console.WriteLine("No notes found.");
            return;
        }
        
        Console.WriteLine("Your notes:");
        foreach (var note in _notes.OrderByDescending(n => n.CreatedAt))
        {
            Console.WriteLine($"[{note.Id}] {note.Title} - {note.CreatedAt:yyyy-MM-dd HH:mm}");
            Console.WriteLine($"    {note.Content}");
            Console.WriteLine();
        }
    }
    
    /// <summary>
    /// Delete a note by ID
    /// </summary>
    /// <param name="id">Note ID to delete</param>
    public void DeleteNote(int id)
    {
        var note = _notes.FirstOrDefault(n => n.Id == id);
        if (note != null)
        {
            _notes.Remove(note);
            Console.WriteLine($"Note '{note.Title}' deleted successfully.");
        }
        else
        {
            Console.WriteLine($"Note with ID {id} not found.");
        }
    }
    
    /// <summary>
    /// Search notes by title or content
    /// </summary>
    /// <param name="searchTerm">Term to search for</param>
    /// <returns>List of matching notes</returns>
    public List<Note> SearchNotes(string searchTerm)
    {
        return _notes.Where(n => 
            n.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
            n.Content.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
        ).ToList();
    }
    
    /// <summary>
    /// Update an existing note
    /// </summary>
    /// <param name="id">Note ID to update</param>
    /// <param name="newTitle">New title (optional)</param>
    /// <param name="newContent">New content (optional)</param>
    public void UpdateNote(int id, string? newTitle = null, string? newContent = null)
    {
        var note = _notes.FirstOrDefault(n => n.Id == id);
        if (note != null)
        {
            if (!string.IsNullOrWhiteSpace(newTitle))
                note.Title = newTitle;
            if (!string.IsNullOrWhiteSpace(newContent))
                note.Content = newContent;
            
            Console.WriteLine($"Note {id} updated successfully.");
        }
        else
        {
            Console.WriteLine($"Note with ID {id} not found.");
        }
    }
    
    /// <summary>
    /// Export notes to JSON file
    /// </summary>
    /// <param name="filePath">Path to export file</param>
    public void ExportNotes(string filePath)
    {
        try
        {
            var json = JsonSerializer.Serialize(_notes, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
            File.WriteAllText(filePath, json);
            Console.WriteLine($"Notes exported successfully to {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error exporting notes: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Import notes from JSON file
    /// </summary>
    /// <param name="filePath">Path to import file</param>
    public void ImportNotes(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File {filePath} not found.");
                return;
            }
            
            var json = File.ReadAllText(filePath);
            var importedNotes = JsonSerializer.Deserialize<List<Note>>(json);
            
            if (importedNotes != null)
            {
                foreach (var note in importedNotes)
                {
                    note.Id = _nextId++;
                    _notes.Add(note);
                }
                Console.WriteLine($"Imported {importedNotes.Count} notes successfully from {filePath}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error importing notes: {ex.Message}");
        }
    }
}
