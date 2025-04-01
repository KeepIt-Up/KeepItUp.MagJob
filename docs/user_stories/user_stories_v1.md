## [Ogólne](https://chatgpt.com/share/0c090b12-13f0-4294-89f4-066703b41de7)

### US 1.1 Logowanie
**Jako** użytkownik,  
**Chcę** móc zalogować się do aplikacji za pomocą mojego adresu e-mail i hasła,  
**Aby** uzyskać dostęp do mojego konta i jego funkcji.

### US 1.2 Rejestracja
**Jako** nowy użytkownik,  
**Chcę** móc zarejestrować się w aplikacji, podając moje dane (np. imię, nazwisko, adres e-mail i hasło),  
**Aby** móc utworzyć konto i korzystać z aplikacji.

### US 1.3 Wyświetlanie profilu
**Jako** zalogowany użytkownik,  
**Chcę** móc wyświetlić mój profil z informacjami o mnie (np. imię, nazwisko, adres e-mail, zdjęcie profilowe),  
**Aby** móc zobaczyć, jakie informacje o mnie są przechowywane w systemie.

### US 1.4 Edytowanie profilu
**Jako** zalogowany użytkownik,  
**Chcę** móc edytować moje informacje profilowe (np. zmienić imię, nazwisko, adres e-mail, zdjęcie profilowe),  
**Aby** móc aktualizować i utrzymywać moje dane w systemie na bieżąco.

### US 1.5 Usunięcie konta
**Jako** zalogowany użytkownik,  
**Chcę** mieć możliwość usunięcia mojego konta,  
**Aby** trwale usunąć moje dane z systemu, jeśli zdecyduję, że nie chcę już korzystać z aplikacji.

## [Organizacja](https://chatgpt.com/share/0c090b12-13f0-4294-89f4-066703b41de7)

### US 2.1 Tworzenie organizacji
**Jako** zalogowany użytkownik,  
**Chcę** móc utworzyć nową organizację, podając jej nazwę oraz podstawowe informacje,  
**Aby** móc zarządzać moją organizacją w aplikacji i zapraszać do niej innych użytkowników.

### US 2.2 Edycja informacji o organizacji
**Jako** uprawniony członek organizacji,  
**Chcę** móc edytować informacje o organizacji, takie jak nazwa, opis, logo czy inne dane,  
**Aby** aktualizować informacje o organizacji i zapewnić ich zgodność z bieżącym stanem.

### US 2.3 Usuwanie organizacji
**Jako** właściciel organizacji,  
**Chcę** móc usunąć organizację z systemu,  
**Aby** zakończyć jej istnienie w aplikacji oraz usunąć wszystkie powiązane dane.

## [Członkowie organizacji]()

### US 3.1 Zapraszanie użytkowników do organizacji
**Jako** uprawniony członek organizacji,  
**Chcę** mieć możliwość zapraszania innych użytkowników do mojej organizacji za pomocą e-maila, unikalnego linku lub poprzez wyszukanie użytkownika,  
**Aby** umożliwić im dołączenie i współpracę w ramach organizacji.

### US 3.2 Usuwanie aktywnych zaproszeń
**Jako** uprawniony członek organizacji,  
**Chcę** mieć możliwość usunięcia zapraszania,  
**Aby** mieć kontrolę nad tym, kto może dołączyć do organizacji oraz móc wycofać zaproszenia, które nie są już aktualne.

### US 3.3 Akceptowanie/odrzucanie zaproszeń
**Jako** użytkownik,  
**Chcę** mieć możliwość zakaceptowania/odrzucenia zapraszania,  
**Aby** zdecydować, czy chcę dołączyć do organizacji i współpracować z jej członkami.

### US 3.4 Dodawanie notatek do członków
**Jako** uprawniony członek organizacji,  
**Chcę** móc dodawać notatki do profili członków organizacji, zarówno czasowe, jak i trwałe,  
**Aby** móc śledzić ważne informacje i komentarze związane z członkami organizacji.

### US 3.5 Usuwanie członka z organizacji
**Jako** uprawniony członek organizacji,  
**Chcę** mieć możliwość usunięcia członka z organizacji,  
**Aby** zarządzać składem organizacji i usunąć osoby, które nie powinny już mieć dostępu do jej zasobów.

## [Role]()

## [Rozmowy]()

## [Dokumenty]()

## [Centrum pomocy]()

## [Rejestracja czasu pracy](https://chatgpt.com/share/2312aecc-f46b-42e9-aa4b-7e286eca0933)

### US 5.1: Rejestracja wejścia do pracy

**Jako** pracownik  
**Chcę** zaznaczyć swoją obecność w pracy za pomocą aplikacji  
**Aby** system odnotował, że rozpocząłem dzień pracy  

#### Scenariusz:
1. **Kontekst**: Pracownik przychodzi do pracy i otwiera aplikację na swoim urządzeniu mobilnym lub komputerze.
2. **Warunek początkowy**: Pracownik jest zalogowany do aplikacji.
3. **Kroki**:
   1. Pracownik wybiera opcję "Zarejestruj wejście" na ekranie głównym aplikacji.
   2. Aplikacja prosi o potwierdzenie godziny wejścia (domyślnie aktualny czas).
   3. Pracownik potwierdza czas wejścia.
4. **Rezultat**: Aplikacja zapisuje czas wejścia pracownika do systemu, wyświetla potwierdzenie oraz aktualizuje status pracownika na "obecny".

#### Kryteria akceptacji:
- Pracownik może zaznaczyć swoją obecność tylko raz na dzień pracy.
- System poprawnie zapisuje i wyświetla czas wejścia.
- Pracownik otrzymuje potwierdzenie rejestracji wejścia.

### US 5.2: Rejestracja wyjścia z pracy

**Jako** pracownik  
**Chcę** zaznaczyć swoje wyjście z pracy za pomocą aplikacji  
**Aby** system odnotował, że zakończyłem dzień pracy  

#### Scenariusz:
1. **Kontekst**: Pracownik kończy pracę i otwiera aplikację na swoim urządzeniu mobilnym lub komputerze.
2. **Warunek początkowy**: Pracownik jest zalogowany do aplikacji i wcześniej zarejestrował swoje wejście.
3. **Kroki**:
   1. Pracownik wybiera opcję "Zarejestruj wyjście" na ekranie głównym aplikacji.
   2. Aplikacja prosi o potwierdzenie godziny wyjścia (domyślnie aktualny czas).
   3. Pracownik potwierdza czas wyjścia.
4. **Rezultat**: Aplikacja zapisuje czas wyjścia pracownika do systemu, wyświetla potwierdzenie oraz aktualizuje status pracownika na "nieobecny".

#### Kryteria akceptacji:
- Pracownik może zaznaczyć swoje wyjście tylko po wcześniejszym zaznaczeniu wejścia tego samego dnia.
- System poprawnie zapisuje i wyświetla czas wyjścia.
- Pracownik otrzymuje potwierdzenie rejestracji wyjścia.

### US 5.3: Edycja czasu wejścia lub wyjścia

**Jako** pracownik  
**Chcę** mieć możliwość edytowania czasu wejścia lub wyjścia w przypadku błędu  
**Aby** moje rzeczywiste godziny pracy były prawidłowo odnotowane w systemie  

#### Scenariusz:
1. **Kontekst**: Pracownik zauważa błąd w zarejestrowanym czasie wejścia lub wyjścia.
2. **Warunek początkowy**: Pracownik jest zalogowany do aplikacji i zarejestrował wcześniej wejście lub wyjście.
3. **Kroki**:
   1. Pracownik wybiera opcję "Edytuj czas wejścia/wyjścia" na ekranie głównym aplikacji.
   2. Pracownik wprowadza nowy czas wejścia lub wyjścia.
   3. Aplikacja prosi o opcjonalne podanie powodu zmiany.
   4. Pracownik zatwierdza zmianę.
   5. Jeśli polityka organizacji wymaga zatwierdzenia zmian, aplikacja wysyła prośbę do administratora o akceptację, zawierającą nowe czasy oraz powód (jeśli podany).
4. **Rezultat**: 
   - Jeśli zmiana nie wymaga zatwierdzenia, czas jest natychmiast aktualizowany w systemie.
   - Jeśli zmiana wymaga zatwierdzenia, pracownik otrzymuje powiadomienie o wysłaniu prośby do administratora oraz późniejsze powiadomienie o akceptacji lub odrzuceniu zmiany.

#### Kryteria akceptacji:
- Pracownik może edytować czas wejścia lub wyjścia do rozpoczęcia nowego okresu rozliczeniowego + 1 dzień.
- System poprawnie zapisuje i wyświetla czas po edycji lub przesyła prośbę do administratora.
- Pracownik otrzymuje powiadomienie o wyniku prośby o zmianę czasu.

### Treść prośby do administratora:
- Imię i nazwisko pracownika.
- Data zmiany.
- Nowy czas wejścia lub wyjścia.
- Opcjonalnie: powód zmiany.

### US 5.4: Powiadomienia o rejestracji wyjścia

**Jako** pracownik  
**Chcę** otrzymywać powiadomienia o konieczności zarejestrowania wyjścia  
**Aby** nie zapomnieć o odnotowaniu zakończenia pracy  

#### Scenariusz:
1. **Kontekst**: Pracownik kończy zmianę i zapomina zarejestrować wyjście.
2. **Warunek początkowy**: Pracownik jest zalogowany do aplikacji, a w organizacji jest ustawiony grafik ze zmianami.
3. **Kroki**:
   1. System wysyła powiadomienie do pracownika na 15 minut przed zakończeniem zmiany.
   2. Pracownik otrzymuje powiadomienie na urządzeniu mobilnym lub komputerze.
   3. Pracownik otwiera aplikację i wybiera opcję "Zarejestruj wyjście".
4. **Rezultat**: Pracownik rejestruje swoje wyjście na czas dzięki przypomnieniu.

#### Kryteria akceptacji:
- Powiadomienie jest wysyłane zgodnie z grafikiem zmian.
- Pracownik otrzymuje powiadomienie na 15 minut przed zakończeniem zmiany.
- Powiadomienia są wyraźne i łatwe do zauważenia.

### US 5.5: Rejestracja za pomocą NFC

**Jako** pracownik  
**Chcę** rejestrować swoje wejście i wyjście z pracy za pomocą telefonu i czytnika NFC  
**Aby** proces rejestracji był szybki i wygodny  

#### Scenariusz:
1. **Kontekst**: Pracownik przychodzi lub wychodzi z pracy i ma przy sobie telefon z NFC.
2. **Warunek początkowy**: Pracownik jest zalogowany do aplikacji i ma włączoną funkcję NFC na telefonie.
3. **Kroki**:
   1. Pracownik zbliża telefon do czytnika NFC przy wejściu lub wyjściu z pracy.
   2. Telefon automatycznie uruchamia aplikację i wyświetla opcję "Zarejestruj wejście" lub "Zarejestruj wyjście".
   3. Pracownik potwierdza rejestrację w aplikacji.
4. **Rezultat**: Aplikacja zapisuje czas wejścia lub wyjścia pracownika do systemu, wyświetla potwierdzenie oraz aktualizuje status pracownika.

#### Kryteria akceptacji:
- System poprawnie rozpoznaje i rejestruje wejście lub wyjście za pomocą NFC.
- Pracownik otrzymuje potwierdzenie rejestracji.
- Proces rejestracji za pomocą NFC jest szybki i wygodny.

## [Grafik](https://chatgpt.com/share/67a7223d-ace7-49fa-befb-0813679b9b3a)

### User Story 1: Wysyłanie dyspozycyjności

**Jako** zarządzający grafikami  
**Chcę** wysyłać powiadomienia do określonej grupy lub wszystkich pracowników z prośbą o wypełnienie dyspozycyjności na dany okres  
**Aby** mieć aktualne informacje na temat dyspozycyjności pracowników  

#### Scenariusz:
1. **Kontekst**: Zarządzający grafikami chce zebrać informacje o dyspozycyjności pracowników na nadchodzący okres.
2. **Warunek początkowy**: Zarządzający jest zalogowany do systemu i ma dostęp do panelu zarządzania grafikami.
3. **Kroki**:
   1. Zarządzający wybiera opcję "Wyślij prośbę o dyspozycyjność" w panelu zarządzania grafikami.
   2. Zarządzający wybiera grupę pracowników lub wszystkich pracowników.
   3. Zarządzający ustala okres, na który zbierane są informacje o dyspozycyjności.
   4. Zarządzający ustala termin wypełnienia formularza dyspozycyjności.
   5. Zarządzający zatwierdza i wysyła prośbę o wypełnienie dyspozycyjności.
4. **Rezultat**: System wysyła powiadomienia do wybranych pracowników z prośbą o wypełnienie formularza dyspozycyjności na dany okres.

#### Kryteria akceptacji:
- Powiadomienia są wysyłane do wybranych pracowników.
- Pracownicy otrzymują powiadomienia na swoje urządzenia mobilne lub komputerowe.
- System umożliwia ustalenie terminu wypełnienia dyspozycyjności.

### User Story 2: Automatyczne wysyłanie próśb o dyspozycyjność

**Jako** zarządzający grafikami  
**Chcę** ustawić automatyczne wysyłanie próśb o wypełnienie dyspozycyjności co zadany okres czasu  
**Aby** nie musieć ręcznie wysyłać próśb za każdym razem  

#### Scenariusz:
1. **Kontekst**: Zarządzający grafikami chce zautomatyzować proces zbierania dyspozycyjności od pracowników.
2. **Warunek początkowy**: Zarządzający jest zalogowany do systemu i ma dostęp do panelu zarządzania grafikami.
3. **Kroki**:
   1. Zarządzający wybiera opcję "Automatyczne wysyłanie próśb o dyspozycyjność" w panelu zarządzania grafikami.
   2. Zarządzający ustala okres, co jaki system ma wysyłać prośby (np. co tydzień, co miesiąc).
   3. Zarządzający ustala grupę pracowników lub wszystkich pracowników, do których będą wysyłane prośby.
   4. Zarządzający zatwierdza ustawienia automatycznego wysyłania próśb.
4. **Rezultat**: System automatycznie wysyła powiadomienia o wypełnienie dyspozycyjności w ustalonym okresie do wybranych pracowników.

#### Kryteria akceptacji:
- System umożliwia ustalenie okresu automatycznego wysyłania próśb.
- Powiadomienia są automatycznie wysyłane do wybranych pracowników w ustalonym czasie.
- Pracownicy otrzymują powiadomienia na swoje urządzenia mobilne lub komputerowe.

### User Story 3: Powiadomienia o niewypełnieniu dyspozycyjności

**Jako** zarządzający grafikami  
**Chcę** ustawić dodatkowe powiadomienia dla pracowników, którzy nie wypełnili dyspozycyjności w wyznaczonym terminie  
**Aby** przypomnieć im o konieczności wypełnienia formularza  

#### Scenariusz:
1. **Kontekst**: Pracownicy nie wypełnili formularza dyspozycyjności w wyznaczonym terminie.
2. **Warunek początkowy**: Pracownik jest zalogowany do systemu i nie wypełnił formularza dyspozycyjności.
3. **Kroki**:
   1. System sprawdza, czy pracownik wypełnił formularz w wyznaczonym terminie.
   2. Jeśli pracownik nie wypełnił formularza, system wysyła dodatkowe powiadomienie.
   3. Powiadomienia są wysyłane regularnie, aż do momentu wypełnienia formularza przez pracownika.
4. **Rezultat**: Pracownik otrzymuje dodatkowe powiadomienia o konieczności wypełnienia dyspozycyjności.

#### Kryteria akceptacji:
- System automatycznie sprawdza status wypełnienia formularza.
- Pracownicy otrzymują dodatkowe powiadomienia, jeśli nie wypełnili formularza w terminie.
- Powiadomienia są wysyłane regularnie, aż do momentu wypełnienia formularza.

## [Wypełnianie oraz edycja dyspozycyjności](https://chatgpt.com/share/769ca347-e3be-46cb-a976-54706e6faf19)

### User Story 1: Otrzymywanie prośby o dyspozycyjność

**Jako** pracownik  
**Chcę** otrzymywać powiadomienia o konieczności wypełnienia formularza dyspozycyjności  
**Aby** system mógł zbierać informacje o mojej dostępności na nadchodzące dni

#### Scenariusz:
1. **Kontekst**: Zbliża się okres, w którym trzeba ustalić dyspozycyjność pracowników.
2. **Warunek początkowy**: Pracownik jest zalogowany do aplikacji.
3. **Kroki**:
   1. System wysyła powiadomienie o konieczności wypełnienia formularza dyspozycyjności.
   2. Pracownik otrzymuje powiadomienie na urządzeniu mobilnym lub komputerze.
   3. Pracownik otwiera powiadomienie i przechodzi do formularza dyspozycyjności.
4. **Rezultat**: Pracownik otrzymuje informację o konieczności wypełnienia formularza.

#### Kryteria akceptacji:
- System wysyła powiadomienia na czas.
- Pracownik otrzymuje powiadomienia na urządzeniu mobilnym lub komputerze.
- Pracownik może łatwo przejść do formularza z powiadomienia.

### User Story 2: Wypełnianie formularza dyspozycyjności

**Jako** pracownik  
**Chcę** wypełnić formularz dyspozycyjności w aplikacji  
**Aby** poinformować pracodawcę o mojej dostępności na nadchodzący okres

#### Scenariusz:
1. **Kontekst**: Pracownik otrzymuje powiadomienie o konieczności wypełnienia formularza dyspozycyjności.
2. **Warunek początkowy**: Pracownik jest zalogowany do aplikacji i otworzył formularz dyspozycyjności.
3. **Kroki**:
   1. Pracownik wybiera daty i godziny, w których jest dostępny do pracy.
   2. Pracownik wypełnia wszelkie dodatkowe informacje wymagane przez formularz.
   3. Pracownik zapisuje i przesyła wypełniony formularz.
4. **Rezultat**: Formularz dyspozycyjności jest zapisany i przesłany do systemu.

#### Kryteria akceptacji:
- Formularz jest prosty i intuicyjny do wypełnienia.
- System poprawnie zapisuje i przesyła dane z formularza.
- Pracownik otrzymuje potwierdzenie, że formularz został przesłany.

### User Story 3: Edytowanie formularza dyspozycyjności

**Jako** pracownik  
**Chcę** móc edytować wcześniej przesłany formularz dyspozycyjności  
**Aby** zaktualizować moje informacje o dostępności w razie zmiany planów

#### Scenariusz:
1. **Kontekst**: Pracownik przesłał formularz dyspozycyjności, ale jego plany się zmieniły.
2. **Warunek początkowy**: Pracownik jest zalogowany do aplikacji i ma dostęp do swojego przesłanego formularza.
3. **Kroki**:
   1. Pracownik otwiera wypełniony formularz dyspozycyjności.
   2. Pracownik edytuje daty i godziny swojej dostępności.
   3. Pracownik zapisuje zmiany i przesyła zaktualizowany formularz.
4. **Rezultat**: Zaktualizowany formularz dyspozycyjności jest zapisany i przesłany do systemu.

#### Kryteria akceptacji:
- Pracownik może edytować formularz do określonej daty.
- System poprawnie zapisuje i przesyła zaktualizowane dane.
- Pracownik otrzymuje potwierdzenie, że zmiany zostały zapisane.

### User Story 4: Przeglądanie zgłoszonych dyspozycyjności

**Jako** menedżer  
**Chcę** przeglądać zgłoszone formularze dyspozycyjności pracowników  
**Aby** planować grafik pracy na podstawie dostępności zespołu

#### Scenariusz:
1. **Kontekst**: Menedżer musi zaplanować grafik pracy na nadchodzący okres.
2. **Warunek początkowy**: Menedżer jest zalogowany do aplikacji.
3. **Kroki**:
   1. Menedżer otwiera sekcję formularzy dyspozycyjności.
   2. Menedżer przegląda zgłoszone formularze według pracowników.
   3. Menedżer eksportuje dane do systemu planowania grafiku.
4. **Rezultat**: Menedżer ma dostęp do wszystkich zgłoszonych formularzy dyspozycyjności.

#### Kryteria akceptacji:
- Menedżer może łatwo przeglądać formularze według pracowników i dat.
- System poprawnie eksportuje dane do zewnętrznego systemu planowania.
- Menedżer może filtrować i sortować formularze według różnych kryteriów.

### User Story 5: Powiadomienia o nieprzesłanych formularzach

**Jako** menedżer  
**Chcę** otrzymywać powiadomienia o pracownikach, którzy nie przesłali formularzy dyspozycyjności  
**Aby** móc przypomnieć im o konieczności ich wypełnienia

#### Scenariusz:
1. **Kontekst**: Zbliża się termin, do którego pracownicy muszą przesłać swoje formularze dyspozycyjności.
2. **Warunek początkowy**: Menedżer jest zalogowany do aplikacji.
3. **Kroki**:
   1. System sprawdza, którzy pracownicy nie przesłali formularzy.
   2. System wysyła powiadomienie do menedżera z listą tych pracowników.
   3. Menedżer kontaktuje się z pracownikami, aby przypomnieć im o konieczności wypełnienia formularzy.
4. **Rezultat**: Menedżer jest informowany o brakujących formularzach i może podjąć odpowiednie działania.

#### Kryteria akceptacji:
- System poprawnie identyfikuje brakujące formularze.
- Menedżer otrzymuje powiadomienia z listą pracowników, którzy nie przesłali formularzy.
- Powiadomienia są wysyłane na czas, aby umożliwić reakcję przed terminem.