# Model Danych - MagJob

Ten dokument opisuje wstępny model danych dla projektu MagJob, w tym główne encje, ich atrybuty i relacje.

## Przegląd

System MagJob wykorzystuje bazę danych PostgreSQL z oddzielnymi schematami dla każdej mikrousługi:
- **identity** - schemat dla Serwisu Tożsamości
- **schedules** - schemat dla Serwisu Zarządzania Dyspozycyjnością i Grafikami
- **timetracking** - schemat dla Serwisu Ewidencji Czasu Pracy

## Schemat identity

### Tabela: Users

Przechowuje informacje o użytkownikach systemu.

| Kolumna | Typ | Opis |
|---------|-----|------|
| id | UUID | Unikalny identyfikator użytkownika |
| external_id | VARCHAR | Identyfikator użytkownika w Keycloak |
| email | VARCHAR | Adres email użytkownika (unikalny) |
| first_name | VARCHAR | Imię użytkownika |
| last_name | VARCHAR | Nazwisko użytkownika |
| phone_number | VARCHAR | Numer telefonu użytkownika (opcjonalny) |
| created_at | TIMESTAMP | Data utworzenia rekordu |
| updated_at | TIMESTAMP | Data ostatniej aktualizacji rekordu |
| is_active | BOOLEAN | Czy użytkownik jest aktywny |

### Tabela: Organizations

Przechowuje informacje o organizacjach.

| Kolumna | Typ | Opis |
|---------|-----|------|
| id | UUID | Unikalny identyfikator organizacji |
| name | VARCHAR | Nazwa organizacji |
| description | TEXT | Opis organizacji (opcjonalny) |
| created_at | TIMESTAMP | Data utworzenia rekordu |
| updated_at | TIMESTAMP | Data ostatniej aktualizacji rekordu |
| is_active | BOOLEAN | Czy organizacja jest aktywna |
| owner_id | UUID | Identyfikator właściciela organizacji (referencja do Users.id) |

### Tabela: Members

Przechowuje informacje o członkostwie użytkowników w organizacjach.

| Kolumna | Typ | Opis |
|---------|-----|------|
| id | UUID | Unikalny identyfikator członkostwa |
| user_id | UUID | Identyfikator użytkownika (referencja do Users.id) |
| organization_id | UUID | Identyfikator organizacji (referencja do Organizations.id) |
| role_id | UUID | Identyfikator roli (referencja do Roles.id) |
| created_at | TIMESTAMP | Data utworzenia rekordu |
| updated_at | TIMESTAMP | Data ostatniej aktualizacji rekordu |
| is_active | BOOLEAN | Czy członkostwo jest aktywne |

### Tabela: Roles

Przechowuje informacje o rolach w organizacjach.

| Kolumna | Typ | Opis |
|---------|-----|------|
| id | UUID | Unikalny identyfikator roli |
| organization_id | UUID | Identyfikator organizacji (referencja do Organizations.id) |
| name | VARCHAR | Nazwa roli |
| description | TEXT | Opis roli (opcjonalny) |
| created_at | TIMESTAMP | Data utworzenia rekordu |
| updated_at | TIMESTAMP | Data ostatniej aktualizacji rekordu |

### Tabela: Permissions

Przechowuje informacje o uprawnieniach.

| Kolumna | Typ | Opis |
|---------|-----|------|
| id | UUID | Unikalny identyfikator uprawnienia |
| name | VARCHAR | Nazwa uprawnienia |
| description | TEXT | Opis uprawnienia |
| created_at | TIMESTAMP | Data utworzenia rekordu |
| updated_at | TIMESTAMP | Data ostatniej aktualizacji rekordu |

### Tabela: RolePermissions

Przechowuje informacje o uprawnieniach przypisanych do ról.

| Kolumna | Typ | Opis |
|---------|-----|------|
| id | UUID | Unikalny identyfikator |
| role_id | UUID | Identyfikator roli (referencja do Roles.id) |
| permission_id | UUID | Identyfikator uprawnienia (referencja do Permissions.id) |
| created_at | TIMESTAMP | Data utworzenia rekordu |
| updated_at | TIMESTAMP | Data ostatniej aktualizacji rekordu |

### Tabela: Invitations

Przechowuje informacje o zaproszeniach do organizacji.

| Kolumna | Typ | Opis |
|---------|-----|------|
| id | UUID | Unikalny identyfikator zaproszenia |
| organization_id | UUID | Identyfikator organizacji (referencja do Organizations.id) |
| email | VARCHAR | Adres email zaproszonego użytkownika |
| role_id | UUID | Identyfikator roli (referencja do Roles.id) |
| token | VARCHAR | Token zaproszenia |
| expires_at | TIMESTAMP | Data wygaśnięcia zaproszenia |
| created_at | TIMESTAMP | Data utworzenia rekordu |
| updated_at | TIMESTAMP | Data ostatniej aktualizacji rekordu |
| created_by | UUID | Identyfikator użytkownika, który utworzył zaproszenie (referencja do Users.id) |
| status | VARCHAR | Status zaproszenia (pending, accepted, rejected, expired) |

## Schemat schedules

### Tabela: Availabilities

Przechowuje informacje o dyspozycyjności pracowników.

| Kolumna | Typ | Opis |
|---------|-----|------|
| id | UUID | Unikalny identyfikator dyspozycyjności |
| user_id | UUID | Identyfikator użytkownika |
| organization_id | UUID | Identyfikator organizacji |
| start_time | TIMESTAMP | Czas rozpoczęcia dyspozycyjności |
| end_time | TIMESTAMP | Czas zakończenia dyspozycyjności |
| recurrence_rule | VARCHAR | Reguła powtarzania (opcjonalna, format iCalendar RRULE) |
| created_at | TIMESTAMP | Data utworzenia rekordu |
| updated_at | TIMESTAMP | Data ostatniej aktualizacji rekordu |
| notes | TEXT | Dodatkowe informacje (opcjonalne) |

### Tabela: Schedules

Przechowuje informacje o grafikach.

| Kolumna | Typ | Opis |
|---------|-----|------|
| id | UUID | Unikalny identyfikator grafiku |
| organization_id | UUID | Identyfikator organizacji |
| name | VARCHAR | Nazwa grafiku |
| description | TEXT | Opis grafiku (opcjonalny) |
| start_date | DATE | Data rozpoczęcia okresu grafiku |
| end_date | DATE | Data zakończenia okresu grafiku |
| status | VARCHAR | Status grafiku (draft, published, archived) |
| created_at | TIMESTAMP | Data utworzenia rekordu |
| updated_at | TIMESTAMP | Data ostatniej aktualizacji rekordu |
| created_by | UUID | Identyfikator użytkownika, który utworzył grafik |

### Tabela: ScheduleEntries

Przechowuje informacje o wpisach w grafiku.

| Kolumna | Typ | Opis |
|---------|-----|------|
| id | UUID | Unikalny identyfikator wpisu |
| schedule_id | UUID | Identyfikator grafiku (referencja do Schedules.id) |
| user_id | UUID | Identyfikator użytkownika |
| start_time | TIMESTAMP | Czas rozpoczęcia pracy |
| end_time | TIMESTAMP | Czas zakończenia pracy |
| position | VARCHAR | Stanowisko/rola w grafiku (opcjonalne) |
| notes | TEXT | Dodatkowe informacje (opcjonalne) |
| created_at | TIMESTAMP | Data utworzenia rekordu |
| updated_at | TIMESTAMP | Data ostatniej aktualizacji rekordu |
| created_by | UUID | Identyfikator użytkownika, który utworzył wpis |
| status | VARCHAR | Status wpisu (scheduled, confirmed, canceled) |

### Tabela: ScheduleTemplates

Przechowuje informacje o szablonach grafików.

| Kolumna | Typ | Opis |
|---------|-----|------|
| id | UUID | Unikalny identyfikator szablonu |
| organization_id | UUID | Identyfikator organizacji |
| name | VARCHAR | Nazwa szablonu |
| description | TEXT | Opis szablonu (opcjonalny) |
| created_at | TIMESTAMP | Data utworzenia rekordu |
| updated_at | TIMESTAMP | Data ostatniej aktualizacji rekordu |
| created_by | UUID | Identyfikator użytkownika, który utworzył szablon |

### Tabela: ScheduleTemplateEntries

Przechowuje informacje o wpisach w szablonach grafików.

| Kolumna | Typ | Opis |
|---------|-----|------|
| id | UUID | Unikalny identyfikator wpisu |
| template_id | UUID | Identyfikator szablonu (referencja do ScheduleTemplates.id) |
| day_of_week | INTEGER | Dzień tygodnia (1-7, gdzie 1 to poniedziałek) |
| start_time | TIME | Czas rozpoczęcia pracy |
| end_time | TIME | Czas zakończenia pracy |
| position | VARCHAR | Stanowisko/rola w grafiku (opcjonalne) |
| required_users | INTEGER | Liczba wymaganych użytkowników |
| notes | TEXT | Dodatkowe informacje (opcjonalne) |
| created_at | TIMESTAMP | Data utworzenia rekordu |
| updated_at | TIMESTAMP | Data ostatniej aktualizacji rekordu |

## Schemat timetracking

### Tabela: TimeEntries

Przechowuje informacje o wpisach czasu pracy.

| Kolumna | Typ | Opis |
|---------|-----|------|
| id | UUID | Unikalny identyfikator wpisu |
| user_id | UUID | Identyfikator użytkownika |
| organization_id | UUID | Identyfikator organizacji |
| schedule_entry_id | UUID | Identyfikator wpisu w grafiku (opcjonalny, referencja do schedules.ScheduleEntries.id) |
| start_time | TIMESTAMP | Czas rozpoczęcia pracy |
| end_time | TIMESTAMP | Czas zakończenia pracy |
| break_duration | INTEGER | Czas przerwy w minutach |
| notes | TEXT | Dodatkowe informacje (opcjonalne) |
| status | VARCHAR | Status wpisu (pending, approved, rejected) |
| created_at | TIMESTAMP | Data utworzenia rekordu |
| updated_at | TIMESTAMP | Data ostatniej aktualizacji rekordu |
| approved_by | UUID | Identyfikator użytkownika, który zatwierdził wpis (opcjonalny) |
| approved_at | TIMESTAMP | Data zatwierdzenia wpisu (opcjonalna) |

### Tabela: Absences

Przechowuje informacje o nieobecnościach.

| Kolumna | Typ | Opis |
|---------|-----|------|
| id | UUID | Unikalny identyfikator nieobecności |
| user_id | UUID | Identyfikator użytkownika |
| organization_id | UUID | Identyfikator organizacji |
| start_date | DATE | Data rozpoczęcia nieobecności |
| end_date | DATE | Data zakończenia nieobecności |
| type | VARCHAR | Typ nieobecności (vacation, sick_leave, personal_leave, etc.) |
| status | VARCHAR | Status nieobecności (pending, approved, rejected) |
| notes | TEXT | Dodatkowe informacje (opcjonalne) |
| created_at | TIMESTAMP | Data utworzenia rekordu |
| updated_at | TIMESTAMP | Data ostatniej aktualizacji rekordu |
| approved_by | UUID | Identyfikator użytkownika, który zatwierdził nieobecność (opcjonalny) |
| approved_at | TIMESTAMP | Data zatwierdzenia nieobecności (opcjonalna) |

### Tabela: AbsenceTypes

Przechowuje informacje o typach nieobecności.

| Kolumna | Typ | Opis |
|---------|-----|------|
| id | UUID | Unikalny identyfikator typu nieobecności |
| organization_id | UUID | Identyfikator organizacji |
| name | VARCHAR | Nazwa typu nieobecności |
| description | TEXT | Opis typu nieobecności (opcjonalny) |
| color | VARCHAR | Kolor dla interfejsu użytkownika |
| requires_approval | BOOLEAN | Czy wymaga zatwierdzenia |
| created_at | TIMESTAMP | Data utworzenia rekordu |
| updated_at | TIMESTAMP | Data ostatniej aktualizacji rekordu |

### Tabela: Reports

Przechowuje informacje o raportach czasu pracy.

| Kolumna | Typ | Opis |
|---------|-----|------|
| id | UUID | Unikalny identyfikator raportu |
| organization_id | UUID | Identyfikator organizacji |
| name | VARCHAR | Nazwa raportu |
| description | TEXT | Opis raportu (opcjonalny) |
| start_date | DATE | Data rozpoczęcia okresu raportu |
| end_date | DATE | Data zakończenia okresu raportu |
| created_at | TIMESTAMP | Data utworzenia rekordu |
| updated_at | TIMESTAMP | Data ostatniej aktualizacji rekordu |
| created_by | UUID | Identyfikator użytkownika, który utworzył raport |
| status | VARCHAR | Status raportu (generating, ready, error) |
| file_path | VARCHAR | Ścieżka do pliku raportu (opcjonalna) |

## Diagram ERD

![Model Danych](../images/data-model.png)

Diagram ERD przedstawia relacje między głównymi encjami w systemie. Diagram można wygenerować na podstawie powyższego modelu danych za pomocą narzędzi takich jak dbdiagram.io, Lucidchart lub draw.io. 
