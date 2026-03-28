# Plan Funkcjonalnosci - Zmiany w bibliotece integracyjnej KSEF

## Jak pracowac z tym planem

1. Realizujemy funkcjonalnosci sekwencyjnie od `F-001` do `F-130`.
2. Po zakonczeniu zadania zmieniamy `[ ]` na `[x]` i dopisujemy date oraz inicjaly.
3. Dla funkcji krytycznych oznaczonych `BLOCKER` kolejne kroki sa zalezne.
4. Gdy cos zmieniamy w zakresie, dopisujemy krotka notatke pod pozycja.
5. **UWAGA!** Jeżeli coś nie opisano w funkcjonalności to samodzielnie podejmujesz decyzje nawet te dotyczące decyzji architektonicznych, wybierając najlepszy możliwy wariant. Samodzielnie budujesz plan implementacji i realizujesz go bez pytania o zatwierdzenie.

Przyklad oznaczenia:

- [x] F-001 Opis funkcji [DONE: 2026-02-14, KR]

## Definicja "zrobione" (DoD)

Kazda funkcjonalnosc jest uznana za zrobiona, gdy:

1. Dziala lokalnie.
2. Ma testy (minimum jednostkowe/integracyjne tam, gdzie ma sens).
3. Ma logowanie bledow i metryki podstawowe.
4. Jest opisana w dokumentacji technicznej lub runbooku.
5. Przeszla review kodu.

## Zmiana bazy kodu ksef-client-csharp z nuget na fork

### Kontekst

Pakiety NuGet `KSeF.Client`, `KSeF.Client.Core`, `KSeF.Client.ClientFactory` z GitHub Packages (org CIRFMF)
wymagają ciągłego uwierzytelniania tokenem PAT z uprawnieniem `read:packages`.
Token wygasa i wymaga odnawiania, co utrudnia pracę z projektem.

**Rozwiązanie:** Fork repozytorium `CIRFMF/ksef-client-csharp` do organizacji/konta użytkownika,
integracja jako git submodule z referencjami projektowymi (ProjectReference) zamiast NuGet (PackageReference).

**Aktualna wersja pakietów:** 2.1.1 → **Docelowa wersja kodu forka:** 2.3.0

### Analiza breaking changes (2.1.1 → 2.3.0)

| Zmiana | Wersja | Wpływ na KSeF.Api |
|--------|--------|-------------------|
| `DateRange` → `DateTimeOffset` zamiast `DateTime` | 2.1.1 | Brak - kod już używa `DateTimeOffset` |
| `AuthorizationStatusCodeResponse` usunięty → `AuthenticationStatusCodeResponse` | 2.1.1 | Brak - typ nieużywany w KSeF.Api |
| `StatusInfo` → `InvoiceStatusInfo` + `OperationStatusInfo` | 2.0.1 | Brak - KSeF.Api używa własnego `InvoiceProcessingStatus` |
| `AuthenticationMethod` oznaczone obsolete | 2.1.1 | Brak - KSeF.Api używa własnego `KsefAuthMethod` |
| Nowy `ILighthouseClient` (monitoring KSeF) | 2.0.1 | Nowa funkcjonalność - opcjonalna |
| Entity permission grant query | 2.2.0 | Nowa funkcjonalność - opcjonalna |
| PascalCase/camelCase JSON switching | 2.2.0 | Nowa funkcjonalność - wewnętrzna |
| `OnlyMetadata` w `InvoiceExportRequest` | 2.3.0 | Nowa funkcjonalność - opcjonalna |
| FA-3 + załączniki | 2.2.0 | Nowa funkcjonalność - opcjonalna |
| Problem Details (401/403) | 2.2.0 | Lepsza obsługa błędów HTTP |
| Dodano target net10.0 | 2.3.0 | Opcjonalne rozszerzenie |

**Wniosek:** Brak breaking changes wpływających na istniejący kod KSeF.Api. Migracja powinna być bezbolesna.

### Plan implementacji

#### Faza 1: Fork i integracja kodu źródłowego `BLOCKER`

- [ ] F-001 Utworzenie forka repozytorium `CIRFMF/ksef-client-csharp` (v2.3.0) na konto użytkownika via `gh repo fork`
- [ ] F-002 Dodanie forka jako git submodule w katalogu `lib/ksef-client-csharp` w repozytorium KSeF
- [ ] F-003 Usunięcie referencji NuGet (`KSeF.Client`, `KSeF.Client.Core`, `KSeF.Client.ClientFactory`) z `KSeF.Api.csproj`
- [ ] F-004 Dodanie referencji projektowych (ProjectReference) z `KSeF.Api.csproj` do projektów w submodule:
  - `lib/ksef-client-csharp/KSeF.Client/KSeF.Client.csproj`
  - `lib/ksef-client-csharp/KSeF.Client.Core/KSeF.Client.Core.csproj`
  - `lib/ksef-client-csharp/KSeF.Client.ClientFactory/KSeF.Client.ClientFactory.csproj`
- [ ] F-005 Dodanie projektów ksef-client-csharp do solution `KSeF.sln` (w osobnym Solution Folder `lib`)
- [ ] F-006 Aktualizacja `nuget.config` - usunięcie źródła `github-cirf` (nie będzie już potrzebne)

#### Faza 2: Dostosowanie kodu i kompilacja `BLOCKER`

- [ ] F-007 Ustawienie TargetFramework projektów ksef-client-csharp na zgodny z KSeF.Api (net9.0) - ewentualny trim zbędnych targetów
- [ ] F-008 Weryfikacja i rozwiązanie konfliktów zależności NuGet (KSeF.Client ma dodatkowe zależności: `System.IdentityModel.Tokens.Jwt`, `QRCoder`, `PolySharp`, `Microsoft.AspNetCore.Localization`, `Microsoft.Maui.Graphics.Skia`)
- [ ] F-009 Kompilacja pełnego solution `dotnet build KSeF.sln` - naprawienie ewentualnych błędów
- [ ] F-010 Dostosowanie kodu KSeF.Api do zmian API w wersji 2.3.0 (jeśli kompilacja wykryje niezgodności)

#### Faza 3: Testy

- [ ] F-011 Uruchomienie istniejących testów KSeF.Api.Tests - `dotnet test` - naprawienie ewentualnych regresji
- [ ] F-012 Uruchomienie istniejących testów KSeF.Invoice.Tests - weryfikacja że nie ma side effects
- [ ] F-013 Aktualizacja mocków w testach jeśli zmieniły się interfejsy KSeF.Client (np. nowe parametry metod)

#### Faza 4: Dokumentacja i finalizacja

- [ ] F-014 Aktualizacja README.md - zmiana informacji o źródle zależności (z NuGet na submodule)
- [ ] F-015 Aktualizacja `KSeF.Api/README.md` - sekcja instalacji (dodanie informacji o `git submodule init/update`)
- [ ] F-016 Dodanie instrukcji klonowania z submodułem: `git clone --recurse-submodules`
- [ ] F-017 Aktualizacja pliku `.gitignore` jeśli potrzebne (lib/ nie powinien być ignorowany)
- [ ] F-018 Budowanie binariów Release: `dotnet build -c Release`
- [ ] F-019 Aktualizacja MEMORY.md z nowymi faktami o strukturze projektu

### Procedura aktualizacji forka (na przyszłość)

Gdy pojawi się nowa wersja `ksef-client-csharp`:
```bash
cd lib/ksef-client-csharp
git fetch upstream
git merge upstream/main
cd ../..
git add lib/ksef-client-csharp
git commit -m "Update ksef-client-csharp submodule to vX.Y.Z"
```

### Alternatywa rozważona i odrzucona

**Kopiowanie kodu źródłowego bez submodule** - odrzucone, bo utrudnia śledzenie zmian upstream i aktualizację.
**Lokalne źródło NuGet (nupkg)** - odrzucone, bo nie rozwiązuje problemu wersjonowania i wymaga ręcznego budowania pakietów.