## Zadanie rekrutacyjne dla firmy W2S ##
# W2S_RecruitmentTask

## Opis projektu

Way2SendAPI to aplikacja webowa stworzona w **ASP.NET Core Web API** z wykorzystaniem **.NET 8.0**. Główne funkcjonalności projektu to:

- Tworzenie, edytowanie i miękkie usuwanie zadań.
- Wysyłanie przypomnień o zadaniach za pomocą e-maili (integracja z serwerem SMTP).
- Obsługa uwierzytelniania i autoryzacji przy użyciu tokenów **JWT**.
- Paginacja i filtrowanie w metodzie GET.
- Przechowywanie logów z wykorzystaniem Serilog.
- Testy jednostkowe z wykorzystaniem **xUnit**, **Moq** i **FluentAssertions**.

## Funkcjonalności

### 1. Zarządzanie zadaniami
- Tworzenie, edytowanie i oznaczanie zadań jako "usunięte" (miękkie usuwanie, z użyciem kolumny `Removed`).
- Pobieranie zadań z opcją paginacji oraz filtrowania.

### 2. Przypomnienia o zadaniach
- System przypomnień o zadaniach bliskich wygaśnięcia (mniej niż godzina do terminu).
- Przypomnienia wysyłane są za pomocą e-maili z wykorzystaniem danych serwera SMTP zapisanych w `appsettings.json`.

### 3. Uwierzytelnianie i autoryzacja
- Obsługa logowania za pomocą tokenów JWT.
- Endpoint `/api/auth/login` generuje token JWT dla użytkownika.

### 4. Logowanie zdarzeń
- Logi aplikacji zapisywane są do plików w katalogu `Logs/` z użyciem Serilog.

### 5. Testy jednostkowe
- Testy kontrolerów i repozytoriów wykonane w **xUnit**.
- Mockowanie zależności przy użyciu **Moq**.
- Sprawdzenie poprawności kodów HTTP i danych zwracanych przez API.

## Wymagania

- **.NET 8.0 SDK**
- **SQL Server** jako baza danych.
- Dostępny serwer SMTP do wysyłania e-maili.

## Użycie

### 1. Autoryzacja
- Uzyskaj token JWT, wysyłając żądanie POST na `/api/auth/login` z przykładowym ciałem:
   ~~~bash
   {
       "username": "admin",
       "password": "admin"
   }
   ~~~bash
- Użyj tokena w nagłówku `Authorization` w formacie `Bearer {token}`.

### 2. Endpointy

| Metoda | Endpoint         | Opis                                |
|--------|------------------|--------------------------------------|
| GET    | /api/tasks       | Pobierz wszystkie zadania           |
| POST   | /api/tasks       | Utwórz nowe zadanie                 |
| PUT    | /api/tasks/{id}  | Zaktualizuj istniejące zadanie      |
| DELETE | /api/tasks/{id}  | Usuń zadanie (miękkie usunięcie)    |

## Testy jednostkowe

1. Przejdź do folderu projektu testów:
   ~~~bash
   cd Way2SendApi.Tests
   ~~~bash

2. Uruchom testy:
   ~~~bash
   dotnet test
   ~~~bash

