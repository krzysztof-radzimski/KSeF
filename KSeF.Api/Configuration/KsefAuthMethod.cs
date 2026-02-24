namespace KSeF.Api.Configuration;

/// <summary>
/// Metoda autoryzacji do KSeF
/// </summary>
public enum KsefAuthMethod
{
    /// <summary>
    /// Autoryzacja tokenem KSeF
    /// </summary>
    Token,

    /// <summary>
    /// Autoryzacja certyfikatem kwalifikowanym (XAdES)
    /// </summary>
    Certificate
}
