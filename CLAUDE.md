C# .NET 9.0
Używasz Dependency Injection.
Nie stosujesz klas i metod statycznych.

## Multitargeting

Projekty KSeF.Invoice i KSeF.Api wspierają multitargeting dla szerokiej kompatybilności:
- **netstandard2.0** - Kompatybilność z .NET Framework 4.6.2+, .NET Core 2.0+
- **net8.0** - .NET 8 LTS
- **net9.0** - .NET 9 STS
- **net10.0** - .NET 10 (najnowsza wersja)

### Polyfills dla netstandard2.0

Projekty używają następujących polyfills dla zapewnienia kompatybilności:
- **PolySharp** (1.15.0) - IsExternalInit, nullable attributes, Index/Range
- **Portable.System.DateTimeOnly** (9.0.1, tylko KSeF.Invoice) - DateOnly/TimeOnly dla netstandard2.0
- **ThrowHelper** - Polyfill dla ArgumentNullException.ThrowIfNull/ThrowIfNullOrWhiteSpace w Polyfills/ArgumentHelpers.cs

### Wersjonowanie zależności

Zależności Microsoft.Extensions.* są wersjonowane per framework:
- **netstandard2.0, net8.0**: wersje 8.0.x
- **net9.0**: wersje 9.0.x
- **net10.0**: wersje 10.0.x

Kondycjonalne ItemGroup w .csproj zapewniają właściwe wersje dla każdego targetu.