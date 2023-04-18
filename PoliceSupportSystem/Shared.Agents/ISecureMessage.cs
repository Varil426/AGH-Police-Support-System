namespace Shared.Agents;

[Obsolete(message: "Placeholder for an eventual future functionality.")]
public interface ISecureMessage : IMessage
{
    string Hash { get; }
}