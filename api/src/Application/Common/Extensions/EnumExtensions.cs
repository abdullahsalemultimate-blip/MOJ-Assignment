namespace InventorySys.Application.Common.Extensions;

public static class EnumExtensions
{
    /// <summary>
    /// Converts an Enum type into a List of LookupDto objects.
    /// </summary>
    /// <typeparam name="TEnum">The Enum type to convert.</typeparam>
    /// <returns>A List of LookupDto containing the enum's integer value and string name.</returns>
    /// <exception cref="ArgumentException">Thrown if TEnum is not an Enum type.</exception>
    public static List<LookupDto> ToLookupList<TEnum>() where TEnum : struct, Enum
    {
        if (!typeof(TEnum).IsEnum)
        {
            throw new ArgumentException("TEnum must be an enumerated type.");
        }

        return Enum.GetValues<TEnum>()
            .Select(p => new LookupDto 
            { 
                Id = Convert.ToInt32(p), 
                Title = p.ToString() 
            })
            .ToList();
    }
}