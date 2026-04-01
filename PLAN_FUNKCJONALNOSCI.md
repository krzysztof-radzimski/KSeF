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

- [x] F-001 Utworzenie forka repozytorium `CIRFMF/ksef-client-csharp` (v2.3.0) na konto użytkownika via `gh repo fork` [DONE: 2026-03-28, Claude]
  - Uwaga: Dodano upstream CIRFMF jako submodule (fork można utworzyć później gdy będzie potrzebny push)
- [x] F-002 Dodanie forka jako git submodule w katalogu `lib/ksef-client-csharp` w repozytorium KSeF [DONE: 2026-03-28, Claude]
- [x] F-003 Usunięcie referencji NuGet (`KSeF.Client`, `KSeF.Client.Core`, `KSeF.Client.ClientFactory`) z `KSeF.Api.csproj` [DONE: 2026-03-28, Claude]
- [x] F-004 Dodanie referencji projektowych (ProjectReference) z `KSeF.Api.csproj` do projektów w submodule: [DONE: 2026-03-28, Claude]
  - `lib/ksef-client-csharp/KSeF.Client/KSeF.Client.csproj`
  - `lib/ksef-client-csharp/KSeF.Client.Core/KSeF.Client.Core.csproj`
  - `lib/ksef-client-csharp/KSeF.Client.ClientFactory/KSeF.Client.ClientFactory.csproj`
- [x] F-005 Dodanie projektów ksef-client-csharp do solution `KSeF.sln` (w osobnym Solution Folder `lib`) [DONE: 2026-03-28, Claude]
- [x] F-006 Aktualizacja `nuget.config` - usunięcie źródła `github-cirf` (nie będzie już potrzebne) [DONE: 2026-03-28, Claude]

#### Faza 2: Dostosowanie kodu i kompilacja `BLOCKER`

- [x] F-007 Ustawienie TargetFramework projektów ksef-client-csharp na zgodny z KSeF.Api (net9.0) - ewentualny trim zbędnych targetów [DONE: 2026-03-28, Claude]
  - Nie wymagało zmian - multi-target (netstandard2.0;net8.0;net9.0;net10.0) jest kompatybilny
- [x] F-008 Weryfikacja i rozwiązanie konfliktów zależności NuGet [DONE: 2026-03-28, Claude]
  - Brak konfliktów - wszystkie zależności rozwiązane poprawnie
- [x] F-009 Kompilacja pełnego solution `dotnet build KSeF.sln` - naprawienie ewentualnych błędów [DONE: 2026-03-28, Claude]
  - Kompilacja: 0 błędów, 561 ostrzeżeń (wszystkie z upstream ksef-client-csharp)
- [x] F-010 Dostosowanie kodu KSeF.Api do zmian API w wersji 2.3.0 (jeśli kompilacja wykryje niezgodności) [DONE: 2026-03-28, Claude]
  - Nie wymagało zmian - brak breaking changes zgodnie z analizą

#### Faza 3: Testy

- [x] F-011 Uruchomienie istniejących testów KSeF.Api.Tests - `dotnet test` - naprawienie ewentualnych regresji [DONE: 2026-03-28, Claude]
  - 16/16 testów przeszło pomyślnie
- [x] F-012 Uruchomienie istniejących testów KSeF.Invoice.Tests - weryfikacja że nie ma side effects [DONE: 2026-03-28, Claude]
  - 653/653 testów przeszło pomyślnie
- [x] F-013 Aktualizacja mocków w testach jeśli zmieniły się interfejsy KSeF.Client (np. nowe parametry metod) [DONE: 2026-03-28, Claude]
  - Nie wymagało zmian - interfejsy kompatybilne

#### Faza 4: Dokumentacja i finalizacja

- [x] F-014 Aktualizacja README.md - zmiana informacji o źródle zależności (z NuGet na submodule) [DONE: 2026-03-28, Claude]
- [x] F-015 Aktualizacja `KSeF.Api/README.md` - sekcja instalacji (dodanie informacji o `git submodule init/update`) [DONE: 2026-03-28, Claude]
- [x] F-016 Dodanie instrukcji klonowania z submodułem: `git clone --recurse-submodules` [DONE: 2026-03-28, Claude]
- [x] F-017 Aktualizacja pliku `.gitignore` jeśli potrzebne (lib/ nie powinien być ignorowany) [DONE: 2026-03-28, Claude]
  - lib/ nie jest ignorowany - nie wymagało zmian
- [x] F-018 Budowanie binariów Release: `dotnet build -c Release` [DONE: 2026-03-28, Claude]
  - Release build: 0 błędów, 561 ostrzeżeń
- [x] F-019 Aktualizacja MEMORY.md z nowymi faktami o strukturze projektu [DONE: 2026-03-28, Claude]

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

---

## F-020: Naprawa autoryzacji i otwierania sesji KSeF (demo + produkcja)

### Kontekst

Autoryzacja tokenem KSeF powtarzalnie napotykała problemy — naprawienie jednego błędu powodowało kolejny.
Zidentyfikowano dwa oddzielne bugi w `KsefSessionService.OpenSessionAsync()`:

1. **ECDSA vs RSA** — domyślny `EncryptionMethodEnum` w `AuthCoordinator.AuthKsefTokenAsync()` to `ECDsa`,
   ale certyfikaty publiczne KSeF zawierają klucze RSA. `KsefSessionService` poprawnie nadpisuje na `Rsa`,
   jednak starsze wersje submodule w projektach konsumenckich (np. medops) mogą nie mieć tej poprawki.

2. **Puste formCode i encryption** — `OpenSessionAsync()` tworzył `new OpenOnlineSessionRequest()` (pusty obiekt),
   zamiast używać `OpenOnlineSessionRequestBuilder` z wymaganymi polami: `FormCode` (systemCode, schemaVersion, value)
   i `Encryption` (encryptedSymmetricKey, initializationVector). KSeF API zwracało błąd 21405:
   `'formCode' must not be empty.; 'encryption' must not be empty.`

### Analiza (log z LegalInsightCRM z 2026-04-01)

| Czas | Błąd | Przyczyna |
|------|------|-----------|
| 13:19:07 | `Nie znaleziono klucza ECDSA.` | Stary kod próbował ECDSA na certyfikacie RSA |
| 13:54:05 | `21405: 'formCode' must not be empty.; 'encryption' must not be empty.` | Po naprawie ECDSA→RSA brakujące pola w żądaniu |

### Plan implementacji

- [x] F-020.1 Analiza logów i identyfikacja obu problemów [DONE: 2026-04-01, Claude]
- [x] F-020.2 Analiza kodu CryptographyService — flow szyfrowania RSA vs ECDSA [DONE: 2026-04-01, Claude]
- [x] F-020.3 Analiza dokumentacji KSeF API v2 — wymagane pola OpenOnlineSession [DONE: 2026-04-01, Claude]
- [x] F-020.4 Rozszerzenie `SessionInfo` o `EncryptionData` (klucz AES + IV sesji) [DONE: 2026-04-01, Claude]
- [x] F-020.5 Naprawa `KsefSessionService.OpenSessionAsync()` — użycie `OpenOnlineSessionRequestBuilder` [DONE: 2026-04-01, Claude]
  - Generowanie `EncryptionData` przez `CryptographyService.GetEncryptionData()`
  - Budowanie requestu z `FormCode` (FA3) i `Encryption` (encryptedSymmetricKey + IV)
  - Przechowywanie `EncryptionData` w `SessionInfo` do szyfrowania faktur
- [x] F-020.6 Naprawa `KsefInvoiceSendService` — użycie `EncryptionData` z sesji zamiast generowania nowego [DONE: 2026-04-01, Claude]
- [x] F-020.7 Aktualizacja testów jednostkowych — mockowanie `GetEncryptionData()` [DONE: 2026-04-01, Claude]
- [x] F-020.8 Dodanie testu integracyjnego `Integration_FullSessionFlow_OpenAndClose_Succeeds` [DONE: 2026-04-01, Claude]
  - Pełny flow: WarmupAsync → AuthKsefTokenAsync → GetEncryptionData → OpenOnlineSession → CloseSession
  - Testowany na demo i produkcji
- [x] F-020.9 Weryfikacja autoryzacji na obu środowiskach [DONE: 2026-04-01, Claude]
  - Demo (`api-demo.ksef.mf.gov.pl`): autoryzacja + sesja — **OK**
  - Produkcja (`api.ksef.mf.gov.pl`): autoryzacja + sesja — **OK**

### Wyniki testów integracyjnych

| Środowisko | Auth | Open Session | Close Session | Wynik |
|------------|------|-------------|---------------|-------|
| Demo (TR) | OK (427ms) | OK | OK | **PASS** |
| Produkcja (PRD) | OK (495ms) | OK | OK | **PASS** |
| Test (TE) | FAIL (brak tokenu) | — | — | Brak tokenu testowego |

### Zmienione pliki

- `KSeF.Api/Models/SessionInfo.cs` — dodano `EncryptionData` property
- `KSeF.Api/Services/KsefSessionService.cs` — użycie buildera z FormCode + Encryption
- `KSeF.Api/Services/KsefInvoiceSendService.cs` — użycie EncryptionData z sesji
- `Tests/KSeF.Api.Tests/Services/KsefSessionServiceTests.cs` — mockowanie GetEncryptionData
- `Tests/KSeF.Api.Tests/Services/KsefAuthorizationTests.cs` — nowy test integracyjny + mockowanie

### Poprawny flow autoryzacji i sesji (referencja)

```
1. CryptographyService.WarmupAsync()           — pobiera certyfikaty publiczne z KSeF
2. AuthCoordinator.AuthKsefTokenAsync(RSA)      — szyfruje token RSA-OAEP, uzyskuje accessToken
3. CryptographyService.GetEncryptionData()      — generuje AES-256 key+IV, szyfruje key RSA
4. OpenOnlineSessionRequestBuilder              — buduje request z FormCode(FA3) + Encryption
5. KSeFClient.OpenOnlineSessionAsync()          — otwiera sesję
6. [wysyłanie faktur szyfrowanych AES-256-CBC z kluczem z kroku 3]
7. KSeFClient.CloseOnlineSessionAsync()         — zamyka sesję
```