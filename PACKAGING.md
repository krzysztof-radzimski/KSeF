# Pakowanie i Publikacja NuGet

## Przegląd

Projekt KSeF składa się z dwóch pakietów NuGet:
- **KSeF.Invoice** - Modele danych dla faktur KSeF FA(3)
- **KSeF.Api** - Serwisy integracji z API KSeF (zależy od KSeF.Invoice)

## Budowanie Pakietów

### Budowanie pojedynczego pakietu

```bash
# KSeF.Invoice
dotnet pack KSeF.Invoice/KSeF.Invoice.csproj -c Release -o ./nupkgs

# KSeF.Api
dotnet pack KSeF.Api/KSeF.Api.csproj -c Release -o ./nupkgs
```

### Budowanie wszystkich pakietów

```bash
# Z katalogu głównego rozwiązania
dotnet pack -c Release -o ./nupkgs
```

## Struktura Pakietów

### KSeF.Invoice.1.0.0.nupkg
- **Zawiera**: Modele danych, builderzy Fluent API, serializacja XML, walidacja
- **Zależności**: Microsoft.Extensions.DependencyInjection.Abstractions, Microsoft.Extensions.Options
- **Target Framework**: net9.0
- **Symbole**: KSeF.Invoice.1.0.0.snupkg

### KSeF.Api.1.0.0.nupkg
- **Zawiera**: Serwisy komunikacji z API KSeF, modele DTO, extension methods DI
- **Zależności**:
  - KSeF.Invoice (1.0.0)
  - KSeF.Client (2.1.1)
  - KSeF.Client.ClientFactory (2.1.1)
  - KSeF.Client.Core (2.1.1)
  - Microsoft.Extensions.* (DI, Logging, Options)
- **Target Framework**: net9.0
- **Symbole**: KSeF.Api.1.0.0.snupkg

## Publikacja

### Publikacja na NuGet.org

```bash
# Publikacja KSeF.Invoice
dotnet nuget push nupkgs/KSeF.Invoice.1.0.0.nupkg -k <API_KEY> -s https://api.nuget.org/v3/index.json

# Publikacja KSeF.Api
dotnet nuget push nupkgs/KSeF.Api.1.0.0.nupkg -k <API_KEY> -s https://api.nuget.org/v3/index.json
```

### Publikacja na GitHub Packages

```bash
# Dodaj źródło GitHub Packages (jednorazowo)
dotnet nuget add source --username <USERNAME> --password <PAT> --store-password-in-clear-text --name github "https://nuget.pkg.github.com/<OWNER>/index.json"

# Publikacja pakietów
dotnet nuget push nupkgs/KSeF.Invoice.1.0.0.nupkg --source github
dotnet nuget push nupkgs/KSeF.Api.1.0.0.nupkg --source github
```

### Publikacja na prywatny feed

```bash
dotnet nuget push nupkgs/KSeF.Invoice.1.0.0.nupkg -s <FEED_URL> -k <API_KEY>
dotnet nuget push nupkgs/KSeF.Api.1.0.0.nupkg -s <FEED_URL> -k <API_KEY>
```

## Wersjonowanie

Projekt używa Semantic Versioning (SemVer):
- **MAJOR**: Niezgodne zmiany API (breaking changes)
- **MINOR**: Nowe funkcjonalności zachowujące kompatybilność wsteczną
- **PATCH**: Poprawki błędów

Wersję można zmienić w plikach `.csproj`:

```xml
<PropertyGroup>
  <Version>1.0.0</Version>
  <PackageVersion>1.0.0</PackageVersion>
</PropertyGroup>
```

## Automatyczne Pakowanie przy Buildzie

Domyślnie `GeneratePackageOnBuild` jest ustawione na `false`. Aby włączyć automatyczne pakowanie:

```xml
<PropertyGroup>
  <GeneratePackageOnBuild>true</GeneratePackageOnBuild>
</PropertyGroup>
```

## Instalacja Pakietów

### Instalacja KSeF.Api (rekomendowane)

```bash
dotnet add package KSeF.Api
```

To automatycznie zainstaluje również KSeF.Invoice jako zależność.

### Instalacja tylko KSeF.Invoice

```bash
dotnet add package KSeF.Invoice
```

## Weryfikacja Zawartości Pakietu

```bash
# Rozpakowanie pakietu do folderu tymczasowego
unzip -d temp_extract nupkgs/KSeF.Api.1.0.0.nupkg

# Lub użycie narzędzia NuGet Package Explorer
# https://github.com/NuGetPackageExplorer/NuGetPackageExplorer
```

## Lista Kontrolna Przed Publikacją

- [ ] Zaktualizowano wersję w `.csproj`
- [ ] Zaktualizowano `PackageReleaseNotes`
- [ ] Przeszły wszystkie testy jednostkowe
- [ ] Zaktualizowano dokumentację (README.md)
- [ ] Przegląd zmian API (breaking changes?)
- [ ] Zbudowano pakiety lokalnie i przetestowano
- [ ] Przygotowano tag git z wersją (np. `v1.0.0`)

## Czyszczenie

```bash
# Usunięcie wygenerowanych pakietów
rm -rf nupkgs

# Czyszczenie artefaktów buildów
dotnet clean
```

## Rozwiązywanie Problemów

### Błąd: "README.md not found"

Upewnij się, że plik README.md istnieje w katalogu projektu i jest dodany do pakietu:

```xml
<ItemGroup>
  <None Include="README.md" Pack="true" PackagePath="\" />
</ItemGroup>
```

### Błąd: "License expression is invalid"

Używaj poprawnych identyfikatorów licencji SPDX:
- MIT
- Apache-2.0
- GPL-3.0-or-later

Lub użyj `<PackageLicenseFile>` dla niestandardowych licencji.

### Pakiet nie zawiera symboli

Upewnij się, że włączone są symbole w `.csproj`:

```xml
<PropertyGroup>
  <IncludeSymbols>true</IncludeSymbols>
  <SymbolPackageFormat>snupkg</SymbolPackageFormat>
</PropertyGroup>
```

## Dodatkowe Zasoby

- [NuGet Package Best Practices](https://docs.microsoft.com/en-us/nuget/create-packages/package-authoring-best-practices)
- [Semantic Versioning](https://semver.org/)
- [GitHub Packages Documentation](https://docs.github.com/en/packages)
