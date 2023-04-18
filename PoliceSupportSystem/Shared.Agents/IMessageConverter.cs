namespace Shared.Agents;

public interface IMessageConverter
{
    string Serialize(IMessage message);

    IMessage Deserialize(string message);
}