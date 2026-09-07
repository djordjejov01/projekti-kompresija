# Kratak izvestaj - Projekat 2 (LDPC kodovi)

## Analiza rezultata

Program implementira sve trazene celine projektnog zadatka:

1. **Konstrukcija matrice H (odeljak 8.8.1)**  
   Konstruisana je LDPC kontrolna matrica za:
   - `n = 15`
   - `n - k = 9` (odnosno `m = 9`)
   - `wr = 5`
   - `wc = 3`  
   Prva grupa redova formirana je blokovski, dok su preostale grupe dobijene permutacijom kolona uz fiksiran seed (`brojIndeksa = 502020`).  
   Provera matrice pokazuje da svaki red ima tacno `wr = 5` jedinica, a svaka kolona `wc = 3` jedinice.

2. **Tabela sindroma/korektora i kodno rastojanje**  
   Generisana je tabela sindroma sa korektorima minimalne tezine i odredjeno je kodno rastojanje:
   - `d = 2`  
   Broj popunjenih sindroma u tabeli je:
   - `128`  
   Dodatno je demonstrirano i dekodiranje pomocu sindroma, koje je na test primeru uspesno.

   **Znacenje rezultata `d = 2`:**
   - Kodno rastojanje je minimalan broj bitova u kojima se razlikuju dve razlicite kodne reci.
   - Kada je `d = 2`, kod **moze da detektuje najvise 1 gresku** (jer vazi `d - 1`).
   - Za pouzdanu korekciju gresaka vazi granica `t = floor((d - 1)/2)`, pa je ovde:
     - `t = floor((2 - 1)/2) = 0`
   - To znaci da ovaj kod **nema garantovanu sposobnost ispravljanja ni jedne bitne greske** u opstem slucaju.
   - Zato nije iznenadjujuce da iterativni Gallager B za neke jednobitne greske ne konvergira ka ispravnoj kodnoj reci.

3. **Gallager B algoritam**  
   Implementiran je Gallager B algoritam sa pragovima:
   - `th0 = 0.5`
   - `th1 = 0.5`  
   Za prikazani test vektor sa jednom greskom dekodiranje nije uspesno, sto je moguce za konkretnu matricu i odabrane pragove.

4. **Minimalna greska koju Gallager B ne ispravlja**  
   Pretragom po rastucoj tezini greske dobijeno je da je minimalna tezina greske koja obara dekoder:
   - `1`  
   Poredjenje sa kodnim rastojanjem:
   - `1 < d (=2)`  
   Dakle, minimalna neuspesna greska je manja od kodnog rastojanja dobijenog u tacki 2.

---

## Nacin pokretanja programa



### Pokretanje iz terminala
Pozicionirati se u folder projekta (gde je `.csproj` fajl), zatim pokrenuti:

```bash
dotnet run
```

### Pokretanje iz Visual Studio okruzenja
1. Otvoriti resenje/projekat.
2. Postaviti `Projekat2` kao startup projekat (ako vec nije).
3. Pokrenuti sa **F5** (Debug) ili **Ctrl+F5** (Run without debugging).

### Ocekivani izlaz
Program ispisuje:
- matricu `H` i statistiku po kolonama,
- kodno rastojanje i broj sindroma,
- rezultat dekodiranja pomocu sindroma,
- rezultat Gallager B dekodiranja,
- minimalnu gresku koju Gallager B ne ispravlja i poredjenje sa `d`.