#if NETSTANDARD2_0
using System.Runtime.CompilerServices;

namespace KSeF.Invoice;

/// <summary>
/// Polyfills for ArgumentException.ThrowIfNullOrWhiteSpace and ArgumentNullException.ThrowIfNull
/// which are not available in netstandard2.0.
/// </summary>
internal static class ThrowHelper
{
    public static void ThrowIfNullOrWhiteSpace(
        string? argument,
        [CallerArgumentExpression("argument")] string? paramName = null)
    {
        if (string.IsNullOrWhiteSpace(argument))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", paramName);
        }
    }

    public static void ThrowIfNull(
        object? argument,
        [CallerArgumentExpression("argument")] string? paramName = null)
    {
        if (argument is null)
        {
            throw new ArgumentNullException(paramName);
        }
    }
}
#endif
