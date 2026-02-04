# Photon Fusion 2 u Unity-ju - Osnovni multiplayer FPS demo

📜 **Sadržaj**
- [Uvod](#uvod)
- [Problem koji Photon Fusion rešava](#problem-koji-photon-fusion-rešava)
- [Šta je Photon Fusion 2](#šta-je-photon-fusion-2)
- [Ključne karakteristike](#ključne-karakteristike)
- [Poređenje sa alternativnim rešenjima](#poređenje-sa-alternativnim-rešenjima)
- [Opis demonstracionog projekta](#opis-demonstracionog-projekta)
- [Arhitektura rešenja](#arhitektura-rešenja)
- [Implementirane funkcionalnosti](#implementirane-funkcionalnosti)
- [RPC komunikacija i sinhronizacija stanja](#rpc-komunikacija-i-sinhronizacija-stanja)
- [Pokretanje projekta](#pokretanje-projekta)
- [Zaključak](#zaključak)
- [Mogućnosti daljeg razvoja](#mogućnosti-daljeg-razvoja)

---

## 📖 Uvod

U okviru ovog tutorijala prikazana je primena **Photon Fusion 2** mrežne biblioteke u **Unity** okruženju, sa ciljem demonstracije osnovnih principa razvoja real-time multiplayer igara.

Kroz jednostavan demonstracioni projekat prikazano je:
- povezivanje više klijenata u istu sesiju,
- sinhronizacija kretanja dva igrača,
- detekcija hit-a (pogotka),
- reagovanje na hit (pogodak),
- korišćenje **RPC** poziva za obaveštavanje svih klijenata o smrti igrača

Tutorijal je koncipiran kao **praktičan primer** za Photon Fusion, sa fokusom na razumevanje osnovnih koncepata i mogućnosti lakog proširenja.

---

## ❗ Problem koji Photon Fusion rešava

Razvoj multiplayer igara nosi niz kompleksnijih problema, među kojima su najvažniji:
- sinhronizacija stanja između više klijenata,
- minimizacija mrežne latencije,
- konzistentnost simulacije,
- autoritet nad podacima (server vs klijent),
- skalabilnost i stabilnost mrežne komunikacije.

Implementacija ovih sistema „od nule“ zahteva veliko iskustvo i značajno povećava kompleksnost projekta. **Photon Fusion** postoji upravo da bi ove probleme apstrahovao i omogućio developerima da se fokusiraju na gameplay logiku.

---
## 🎮 Šta je Photon Fusion 2

Photon Fusion 2 je moderna **real-time networking** biblioteka za Unity, namenjena razvoju multiplayer igara koje zahtevaju brzu i pouzdanu sinhronizaciju.

Fusion koristi **tick-based** simulaciju i jasno definisane modele autoriteta, čime omogućava determinističko ponašanje i dobru kontrolu nad mrežnim stanjem.

Biblioteka je posebno pogodna za:
- FPS igre,
- akcione igre
- kompetitivne multiplayer scenarije,
- real-time kooperativne igre.

---
## ⭐ Ključne karakteristike

- **NetworkObject & NetworkBehaviour** - mrežni evivalenti Unity komponenti, GameObject i MonoBehaviour
- **State Authority / Input Authority** - jasan model vlasništva nad objektima
- **Tick-based simulacija** - determinističko ponašanje
- **RPC (Remote Procedure Calls)** - jednostavna komunikacija između klijenata
- **Lag Compensation** - korekcija kašnjenja kod hit detekcije
- **Host / Shared / Client-Server modeli** - fleksibilna arhitektura sesije

---
## 🆚 Poređenje sa alternativnim rešenjima

| Rešenje            | Prednosti                                    | Nedostaci                        |
|--------------------|----------------------------------------------|----------------------------------|
| Photon Fusion      | Brzina, determinističnost, RPC, lag comp     | Veća kompleksnost u startu       |
| Photon PUN         | Jednostavniji za početnike                   | Lošija skalabilnost              |
| Mirror             | Open-source, fleksibilan                     | Više manuelne logike             |
| Netcode for GO     | Dobra Unity integracija                      | Ograničene napredne funkcije     |

Photon Fusion je izabran jer pruža **najbolji balans performansi i kontrole** za real-time igre.

---

## 🧪 Opis demonstracionog projekta

Projekat predstavlja jednostavnu multiplayer scenu sa dva igrača:
- svaki igrač kontroliše svog karaktera,
- kretanje je sinhronizovano između klijenata,
- igrači mogu da pogode jedan drugog,
- nakon smrti igrač se uklanja iz scene, ali ostaje povezan u sesiji.

Cilj projekata nije kompletna igra, već jasna demonstracija osnovnih koncepara Photon Fusion biblioteke.

---

## 🧱 Arhitektura rešenja

- **BasicSpawner** - upravlja sesijom i mrežnim tick-ovima
- **Player (NetworkBehaviour)** - logika igračevih kontrola kretanja
- **NetworkCharacterControllerFPS** - kopija Photon Fusion clase NetworkCharacterController sa manjim korekcijama radi prilagođavanja FPS načinu kretanaj igrača
- **Weapon (MonoBehaviour)** - upravlja rotacijom oružja (nišanom)
- **WeaponHandler (NetworkBehaviour)** - logika upravljanja pucanjem iz oružja
- **HPHandler (NetworkBehaviour)** - upravlja reakcijom na pogodak za oba slučaja (i kada je igrač pogođen, i kada igrač pogađa)
- **NetworkInputData** - struktura za slanje input-a između servera i klijenta

Logika je podeljena tako da:
- lokalni input obrađuje klijent
- mrežno stanje je pod kontrolom autoriteta,
- svi klijenti dobijaju konzistentne informacije.

---

## ✨ Implementirane funkcionalnosti

- Povezivanje dva igrača u istu sesiju
- Sinhronizovano kretanje igrača
- Hit detekcija
- Ragovanje na pogodak
- RPC poziv za „death event“
- Uklanjanje NetworkObject-a bez diskonekcije

---

## 📡 RPC komunikacija i sinhronizacija stanja

RPC (Remote Procedure Call) se koristi za sinhornizaciju svih klijenata da je igrač pogođen.
Primer:
- lokalni igrač je pogođen i ostaje bez energije (health-a),
- RPC se šalje svim klijentima,
- svaki klijent uklanja odgovarajućeg igrača iz scene pozivom Destroy(gameObject), što uništava objekat u sceni, ali ga ne diskonektuje.

Na ovaj način svi klijenti imaju konzistentno stanje, i ostavlja se mogućnost za ponovno stvaranje (respawn) ubijenog igrača.

---

## 🚀 Pokretanje projekta

### Preduslovi
- Unity Hub
- Unity vezija **2022 LTS** (ili novija)
- Photon Fusion 2 paket
- Photon nalog (besplatan plan)

### Koraci

1. Klonirati repozitorijum:
   ```bash
   git clone https://github.com/Milos-Sstanojevic/PhotonFusion-Demo.git
2. Otovriti projekat u Unity-ju preko Unity Hub-a
3. Uneti Photon App ID
   <image>
4. Pokrenuti dva build-a projekta, ili jedan build i play mode unutar Unity editora

---

## ✅ Zaključak

Ovaj tutorijal demonstrira osnovnu upotrebu Photon Fusion 2 biblioteke u Unity-ju kroz praktičan mupltiplayer primer. 
Prikazani su ključni koncepti mrežne sinhronizacije, autoriteta i RPC komunikacije, uz fokus na jasnoću i mogućnost lakog proširenja.

Photon Fusion se pokazuje kao moćno rešenje za real-time multiplayer igre, posebno u scenarijima gde su performanse i konzistentnost od ključnog značaja.

---

## 🔮 Mogućnosti daljeg razvoja

- Vizuelni health bar sistem
- Respawn mehanika
- Matchmaking sistem
- Host migration

