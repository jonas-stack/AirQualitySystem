namespace Application.Enums;

public static class GraphEnumExtensions
{
    public static string GetTopicName(this GraphEnum graph)
    {
        var type = graph.GetType();
        var member = type.GetMember(graph.ToString()).FirstOrDefault();
        var attribute = member?.GetCustomAttributes(typeof(TopicNameAttribute), false).FirstOrDefault() as TopicNameAttribute;
        return attribute?.Name ?? graph.ToString();
    }
}
