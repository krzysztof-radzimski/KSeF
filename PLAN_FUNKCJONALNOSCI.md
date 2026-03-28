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
- [ ] F-001 