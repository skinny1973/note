using NoteManager.Services;

namespace NoteManager;

/// <summary>
/// Main application entry point for Note Management Console Application
/// </summary>
class Program
{
    /// <summary>
    /// Main entry point of the application
    /// </summary>
    /// <param name="args">Command line arguments</param>
    static void Main(string[] args)
    {
        Console.WriteLine("=== Note Manager Console Application ===");
        Console.WriteLine("Manage your notes from the command line!");
        Console.WriteLine("Type 'help' for available commands or 'exit' to quit.");
        Console.WriteLine();
        
        var processor = new CommandProcessor();
        
        // Handle Ctrl+C to save before exit
        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true; // Prevent immediate exit
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
                Console.WriteLine(); // Empty line to separate commands
            }
        }
    }
}
