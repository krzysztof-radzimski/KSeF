# Release 1.0.1 - Migracja na git submodule

**Data wydania:** 30 marca 2026

## 🎯 Główne zmiany

### Migracja z NuGet na git submodule

Projekt przeszedł na integrację z kodem źródłowym `ksef-client-csharp` jako git submodule zamiast pakietów NuGet z GitHub Packages.

**Korzyści:**
- ✅ Brak potrzeby konfiguracji GitHub Packages
- ✅ Brak potrzeby Personal Access Token (PAT)
- ✅ Pełna kontrola nad kodem zależności
- ✅ Łatwy debugging i development
- ✅ Łatwiejsze śledzenie zmian upstream

**Wersja:** `ksef-client-csharp` 2.3.0 (poprzednio NuGet 2.1.1)

---

## ✨ Nowe funkcjonalności

### Dodana licencja MIT
- Projekt jest teraz oficjalnie objęty licencją MIT
- Plik `LICENSE` dodany do repozytorium

---

## 🔧 Zmiany techniczne

### Struktura projektu

#### Dodane
- `.gitmodules` - konfiguracja submodułu git
- `lib/ksef-client-csharp` - submoduł z kodem źródłowym ksef-client-csharp v2.3.0
- `LICENSE` - licencja MIT
- `PLAN_FUNKCJONALNOSCI.md` - szczegółowy plan migracji i funkcjonalności

#### Zmodyfikowane
- **`KSeF.Api/KSeF.Api.csproj`**
  - Usunięte referencje NuGet: `KSeF.Client`, `KSeF.Client.Core`, `KSeF.Client.ClientFactory`
  - Dodane referencje projektowe (ProjectReference) do:
    - `lib/ksef-client-csharp/KSeF.Client/KSeF.Client.csproj`
    - `lib/ksef-client-csharp/KSeF.Client.Core/KSeF.Client.Core.csproj`
    - `lib/ksef-client-csharp/KSeF.Client.ClientFactory/KSeF.Client.ClientFactory.csproj`

- **`KSeF.sln`**
  - Dodany Solution Folder `lib`
  - Dodane projekty z submodułu do solution:
    - `KSeF.Client`
    - `KSeF.Client.Core`
    - `KSeF.Client.ClientFactory`

- **`nuget.config`**
  - Usunięte źródło `github-cirf` (GitHub Packages CIRFMF)
  - Pozostawione źródła: `nuget.org`, `github-krzysztof-radzimski`
  - Usunięta dokumentacja konfiguracji PAT

- **`README.md`**
  - Tytuł zmieniony na "KSeF - Wsparcie dla integratorów"
  - Przepisana sekcja "Instalacja":
    - Usunięta instrukcja konfiguracji GitHub Packages i PAT
    - Dodana instrukcja klonowania z submodułem: `git clone --recurse-submodules`
    - Dodana instrukcja inicjalizacji submodułu dla istniejących klonów
  - Zaktualizowana sekcja "Wymagania":
    - Usunięte pakiety NuGet z GitHub Packages
    - Dodana informacja o submodule `ksef-client-csharp` v2.3.0
  - Zaktualizowana sekcja "Wkład":
    - Dodany krok klonowania z submodułem

- **`KSeF.Api/README.md`**
  - Zaktualizowana sekcja instalacji - uproszczona bez konfiguracji PAT

---

## 📊 Statystyki zmian

```
9 plików zmienionych
+229 linii dodanych
-76 linii usuniętych
```

### Pliki zmodyfikowane:
- `.gitmodules` (nowy)
- `KSeF.Api/KSeF.Api.csproj`
- `KSeF.Api/README.md`
- `KSeF.sln`
- `LICENSE` (nowy)
- `PLAN_FUNKCJONALNOSCI.md` (nowy)
- `README.md`
- `lib/ksef-client-csharp` (nowy submoduł)
- `nuget.config`

---

## 🧪 Testy

### Wyniki testów jednostkowych

✅ **KSeF.Api.Tests**: 16/16 testów przeszło
✅ **KSeF.Invoice.Tests**: 653/653 testów przeszło

**Łącznie: 669 testów - 100% sukcesu**

### Kompilacja

✅ **Debug build**: 0 błędów, 561 ostrzeżeń (z upstream ksef-client-csharp)
✅ **Release build**: 0 błędów, 561 ostrzeżeń (z upstream ksef-client-csharp)

---

## 🔄 Analiza breaking changes (2.1.1 → 2.3.0)

Przeanalizowano zmiany w `ksef-client-csharp` między wersjami 2.1.1 (NuGet) a 2.3.0 (submodule):

| Zmiana | Status |
|--------|--------|
| `DateRange` → `DateTimeOffset` zamiast `DateTime` | ✅ Bez wpływu - kod już używa `DateTimeOffset` |
| `AuthorizationStatusCodeResponse` usunięty | ✅ Bez wpływu - typ nieużywany |
| `StatusInfo` → `InvoiceStatusInfo` + `OperationStatusInfo` | ✅ Bez wpływu - używamy własnych typów |
| `AuthenticationMethod` obsolete | ✅ Bez wpływu - używamy `KsefAuthMethod` |
| Nowy `ILighthouseClient` (monitoring) | ℹ️ Nowa funkcjonalność - opcjonalna |
| FA-3 + załączniki | ℹ️ Nowa funkcjonalność - opcjonalna |
| Problem Details (401/403) | ✅ Lepsza obsługa błędów HTTP |
| Target net10.0 | ℹ️ Opcjonalne rozszerzenie |

**Wniosek:** Brak breaking changes wpływających na istniejący kod. Migracja bezbolesna.

---

## 📦 Instalacja i aktualizacja

### Nowa instalacja

```bash
# Klonowanie z submodułem (zalecane)
git clone --recurse-submodules https://github.com/krzysztof-radzimski/KSeF.git

# Kompilacja
cd KSeF
dotnet build KSeF.sln
```

### Aktualizacja istniejącego repozytorium

```bash
# Pobranie najnowszych zmian
git pull

# Inicjalizacja i pobranie submodułu
git submodule init
git submodule update

# Kompilacja
dotnet build KSeF.sln
```

### Usunięcie starej konfiguracji (opcjonalnie)

Jeśli masz skonfigurowane źródło `github-cirf` z poprzednich wersji, możesz je usunąć:

```bash
dotnet nuget remove source github-cirf
```

---

## 🔮 Procedura aktualizacji submodułu (dla maintainerów)

Gdy pojawi się nowa wersja `ksef-client-csharp`:

```bash
cd lib/ksef-client-csharp
git fetch upstream
git merge upstream/main
cd ../..
git add lib/ksef-client-csharp
git commit -m "Update ksef-client-csharp submodule to vX.Y.Z"
```

---

## 📝 Committy w release

- `afb5f63` - [POST] Task #1595: Naprawić link do licencji MIT
- `1553101` - [POST] Task #1584: Zrealizować funkcjonalności według planu PLAN_FUNKCJONALNOSCI.md
- `f72414c` - [POST] Task #1583: Integracja z kodem ksef-client-csharp zamiast NuGet
- `5e39a6b` - Create PLAN_FUNKCJONALNOSCI.md
- `3703d95` - Update README.md

---

## 🙏 Podziękowania

Specjalne podziękowania dla zespołu [CIRFMF/ksef-client-csharp](https://github.com/CIRFMF/ksef-client-csharp) za udostępnienie i utrzymanie klienta HTTP dla API KSeF.

---

## 📄 Licencja

Ten projekt jest objęty licencją MIT - szczegóły w pliku [LICENSE](LICENSE).
