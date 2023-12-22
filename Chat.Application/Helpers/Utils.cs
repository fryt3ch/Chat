namespace Chat.Application.Helpers;

public static class Utils
{
    public static T Clone<T>(this T obj)
    {
        var inst = obj.GetType().GetMethod("MemberwiseClone", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

        return (T)inst?.Invoke(obj, null);
    }
}