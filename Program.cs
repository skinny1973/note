// Command Pattern Example for Note Management Console Application
using System.Text;

// Interface for commands
public interface ICommand
{
    void Execute(string[]? args = null);
    string Name { get; }
    string Description { get; }
}

// Note model (per ora semplice, poi collegheremo al DB Oracle)
public class Note
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

// Classe per gestire i dati di salvataggio automatico
public class SaveData
{
    public List<Note> Notes { get; set; } = new();
    public int NextId { get; set; } = 1;
    public DateTime LastSaved { get; set; }
}

// Service per gestire le note (per ora in memoria, poi Oracle DB)
public class NoteService
{
    private readonly List<Note> _notes = new();
    private int _nextId = 1;
    private readonly string _autoSaveFilePath;
    
    public NoteService()
    {
        // Ottieni il nome dell'eseguibile senza estensione
        var executableName = Path.GetFileNameWithoutExtension(System.Diagnostics.Process.GetCurrentProcess().ProcessName);
        _autoSaveFilePath = $"{executableName}.json";
        
        // Carica automaticamente i dati dell'ultima sessione se esistono
        LoadAutoSaveFile();
    }
    
    private void LoadAutoSaveFile()
    {
        try
        {
            if (File.Exists(_autoSaveFilePath))
            {
                var json = File.ReadAllText(_autoSaveFilePath);
                var savedData = System.Text.Json.JsonSerializer.Deserialize<SaveData>(json);
                
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
            
            var json = System.Text.Json.JsonSerializer.Serialize(saveData, new System.Text.Json.JsonSerializerOptions 
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
    
    public List<Note> SearchNotes(string searchTerm)
    {
        return _notes.Where(n => 
            n.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
            n.Content.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
        ).ToList();
    }
    
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
    
    public void ExportNotes(string filePath)
    {
        try
        {
            var json = System.Text.Json.JsonSerializer.Serialize(_notes, new System.Text.Json.JsonSerializerOptions 
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
            var importedNotes = System.Text.Json.JsonSerializer.Deserialize<List<Note>>(json);
            
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

// Concrete commands per gestire le note
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
        
        // Controllo rigoroso dei parametri
        if (args != null && args.Length > 0)
        {
            if (args.Length == 2)
            {
                // Esattamente 2 parametri: usa modalità parametri
                title = args[0].Trim('"');
                content = args[1].Trim('"');
            }
            else if (args.Length == 1)
            {
                // Solo 1 parametro: errore, servono entrambi
                Console.WriteLine("Error: Missing content parameter.");
                Console.WriteLine("Usage: add \"title\" \"content\" OR just 'add' for interactive mode");
                return;
            }
            else
            {
                // Più di 2 parametri: errore
                Console.WriteLine($"Error: Too many parameters ({args.Length}). Expected exactly 2.");
                Console.WriteLine("Usage: add \"title\" \"content\" OR just 'add' for interactive mode");
                return;
            }
        }
        else
        {
            // Nessun parametro: modalità interattiva
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
        // Se c'è un parametro, usalo; altrimenti chiedi interattivamente
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

public class ExitCommand : ICommand
{
    private readonly NoteService _noteService;
    
    public ExitCommand(NoteService noteService)
    {
        _noteService = noteService;
    }
    
    public string Name => "exit";
    public string Description => "Exit the application";
    
    public void Execute(string[]? args = null)
    {
        Console.WriteLine("Saving session data...");
        _noteService.SaveAutoSaveFile();
        Console.WriteLine("Goodbye!");
        Environment.Exit(0);
    }
}

public class HelpCommand : ICommand
{
    private readonly Dictionary<string, ICommand> _commands;
    
    public HelpCommand(Dictionary<string, ICommand> commands)
    {
        _commands = commands;
    }
    
    public string Name => "help";
    public string Description => "Show available commands";
    
    public void Execute(string[]? args = null)
    {
        Console.WriteLine("Available commands:");
        foreach (var cmd in _commands.Values)
        {
            Console.WriteLine($"  {cmd.Name} - {cmd.Description}");
        }
    }
}

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

// Command invoker
public class CommandProcessor
{
    private readonly Dictionary<string, ICommand> _commands;
    private readonly NoteService _noteService;
    
    public CommandProcessor()
    {
        _noteService = new NoteService();
        _commands = new Dictionary<string, ICommand>();
        
        // Registra tutti i comandi disponibili
        RegisterCommand(new AddNoteCommand(_noteService));
        RegisterCommand(new ListNotesCommand(_noteService));
        RegisterCommand(new DeleteNoteCommand(_noteService));
        RegisterCommand(new ExitCommand(_noteService));
        RegisterCommand(new HelpCommand(_commands));
        RegisterCommand(new SearchNoteCommand(_noteService));
        RegisterCommand(new UpdateNoteCommand(_noteService));
        RegisterCommand(new ExportNotesCommand(_noteService));
        RegisterCommand(new ImportNotesCommand(_noteService));
    }
    
    public void RegisterCommand(ICommand command)
    {
        _commands[command.Name] = command;
    }
    
    public void SaveAndExit()
    {
        _noteService.SaveAutoSaveFile();
        Console.WriteLine("Goodbye!");
        Environment.Exit(0);
    }
    
    public void ProcessCommand(string input)
    {
        var parts = ParseCommandLine(input);
        if (parts.Length == 0) return;
        
        var commandName = parts[0].ToLower();
        var args = parts.Length > 1 ? parts[1..] : null;
        
        if (_commands.TryGetValue(commandName, out var command))
        {
            command.Execute(args);
        }
        else
        {
            Console.WriteLine($"Unknown command: {commandName}. Type 'help' for available commands.");
        }
    }
    
    private static string[] ParseCommandLine(string input)
    {
        var parts = new List<string>();
        var current = new StringBuilder();
        bool inQuotes = false;
        
        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            
            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ' ' && !inQuotes)
            {
                if (current.Length > 0)
                {
                    parts.Add(current.ToString());
                    current.Clear();
                }
            }
            else
            {
                current.Append(c);
            }
        }
        
        if (current.Length > 0)
        {
            parts.Add(current.ToString());
        }
        
        return parts.ToArray();
    }
}

// Main application
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Note Manager Console Application ===");
        Console.WriteLine("Manage your notes from the command line!");
        Console.WriteLine("Type 'help' for available commands or 'exit' to quit.");
        Console.WriteLine();
        
        var processor = new CommandProcessor();
        
        // Gestisci Ctrl+C per salvare prima di uscire
        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true; // Previeni l'uscita immediata
            Console.WriteLine("\nSaving session data before exit...");
            processor.SaveAndExit();
        };
        
        while (true)
        {
            Console.Write("note> ");
            var input = Console.ReadLine();
            
            if (!string.IsNullOrWhiteSpace(input))
            {
                processor.ProcessCommand(input);
                Console.WriteLine(); // Riga vuota per separare i comandi
            }
        }
    }
}
