# Izveštaj o Kompresiji Podataka

**Autor:** Đorđe Jovanović 50/2020

Ovaj projekat implementira četiri osnovna algoritma za kompresiju i dekompresiju binarnih fajlova:
1. Shannon-Fano
2. Huffman
3. LZ77
4. LZW (sa 12-bitnom optimizacijom)

Za svaki algoritam se vrši kodiranje, kompresija, dekodiranje i provera uspešnosti dekompresije. Rezultati su poređeni po stepenu kompresije i tačnosti povratnog fajla.

---

## Rezultati kompresije za testirane fajlove

### Primer 1: primer.txt (niska entropija)

- **Entropija fajla:** 1.5116
- **Karakteristike:** Fajl sa niskom entropijom (dominiraju pojedini karakteri)
- **Stepen kompresije:**
  - Shannon-Fano: 4.6587
  - Huffman: 4.6596
  - LZW: 4.4505
  - LZ77: 1.3345

- **Ušteda u procentima:**
  - Shannon-Fano: 78.53%
  - Huffman: 78.54%
  - LZW: 77.53%
  - LZ77: 25.07%

- **Uspešnost dekompresije:**
  - Shannon-Fano kompresija je uspela. ✓
  - Huffman-ova kompresija je uspela. ✓
  - LZW kompresija je uspela. ✓
  - LZ77 kompresija je uspela. ✓

**Analiza:** Shannon-Fano i Huffman daju identične rezultate jer oba generišu optimalne kodove. LZW, zahvaljujući 12-bitnoj optimizaciji, daje skoro identične rezultate. LZ77 je neefikasan jer nema dugih ponavljanja u tekstualnom fajlu.

---

### Primer 2: primer_manje_pogodan.txt (visoka entropija)

- **Entropija fajla:** 4.3809
- **Karakteristike:** Fajl sa visokom entropijom (ravnomerna raspodela karaktera)
- **Stepen kompresije:**
  - Shannon-Fano: 1.7926
  - Huffman: 1.7927
  - LZW: 1.5629
  - LZ77: 0.4631

- **Ušteda u procentima:**
  - Shannon-Fano: 44.21%
  - Huffman: 44.22%
  - LZW: 36.01%
  - LZ77: -115.96%

- **Uspešnost dekompresije:**
  - Shannon-Fano kompresija je uspela. ✓
  - Huffman-ova kompresija je uspela. ✓
  - LZW kompresija je uspela. ✓
  - LZ77 kompresija je uspela. ✓

**Analiza:** Kod fajla sa visokom entropijom (4.38), mogućnost kompresije je manja jer su svi karakteri podjednako zastupljeni. Shannon-Fano i Huffman daju malo bolji rezultat (44%). LZW sa 12-bitnom optimizacijom postižu 36% uštede umesto negativne vrednosti sa 32-bitnim kodovima. LZ77 je negativan jer overhead markera prevazilazi originalnu veličinu.

*Stepen kompresije predstavlja odnos veličine originalnog i kompresovanog fajla (veća vrednost znači bolju kompresiju).*

---

## Struktura izlaznih fajlova

Nakon izvršavanja programa, u folderu `bin\Debug` nalaze se sledeći fajlovi:

- `shannon_fano_compress.bin` (kompresovani fajl, Shannon-Fano)
- `huffman_compress.bin` (kompresovani fajl, Huffman)
- `lz77_compress.bin` (kompresovani fajl, LZ77)
- `lzw_compress.bin` (kompresovani fajl, LZW sa 12-bitnom optimizacijom)

### Format LZW fajla (12-bitni kodovi)

Umesto čuvanja svakog koda kao 32-bitnog int-a (4 bajta), LZW kodovi su optimizovani na 12 bita:
- 1. Broj kodova (4 bajta)
- 2. Broj spakovanih bajtova (4 bajta)
- 3. Kompresovani podaci sa 12-bitnim kodovima

Ova optimizacija omogućava da LZW bude konkurentan sa entropijskim kodovima.

---

## Kako pokrenuti projekat

1. Pripremiti ulazni fajl (npr. `primer.txt`) u folderu gde je buildovana aplikacija.
2. Pokrenuti izvršni fajl `Projekat1.exe` ili startovati iz Visual Studio okruženja (F5).
3. Uneti putanju do fajla kada se zatraži.(u ovom slucaju samo uneti npr. primer.txt)
4. Program automatski obrađuje fajl, primenjuje sve algoritme, prikazuje rezultate kompresije i dekompresije na konzoli.
5. Kompresovani fajlovi se smeštaju u `bin\Debug` folderu projekta.

---

## Zaključak

Svi algoritmi su uspešno kompresovali i dekompresovali testirane fajlove. 

**Shannon-Fano i Huffman** daju najbolje rezultate, posebno kod fajlova sa niskom entropijom (78.53% uštede). Oba algoritma generišu optimalne kodove, pa su njihovi rezultati identični.

**LZW sa 12-bitnom optimizacijom** je značajno poboljšan i sada konkurira entropijskim kodovima. U primeru 1 daje 77.53% uštede, dok u primeru 2 daje 36% uštede umesto negativne vrednosti sa 32-bitnim formatom.

**LZ77** daje slabije rezultate na tekstualnim fajlovima jer nema dugih ponavljanja. Negativna ušteda u primeru 2 je očekivana i pokazuje da LZ77 nije pogodan za fajlove sa visokom entropijom.

Dekompresovani fajlovi su identični originalu, što potvrđuje ispravnost implementacije svih algoritama.
