namespace NoteManager.Interfaces;

/// <summary>
/// Interface for implementing command pattern for note management operations
/// </summary>
public interface ICommand
{
    /// <summary>
    /// Execute the command with optional arguments
    /// </summary>
    /// <param name="args">Optional command arguments</param>
    void Execute(string[]? args = null);
    
    /// <summary>
    /// The name of the command used for invocation
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Description of what the command does
    /// </summary>
    string Description { get; }
}
