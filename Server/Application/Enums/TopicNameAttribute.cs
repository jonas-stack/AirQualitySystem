namespace Application.Enums;

[AttributeUsage(AttributeTargets.Field)]
public class TopicNameAttribute : Attribute
{
    public string Name { get; }

    public TopicNameAttribute(string name)
    {
        Name = name;
    }
}
