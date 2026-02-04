# Photon Fusion 2 u Unity-ju - Osnovni multiplayer FPS demo

**Sadržaj**
- [Uvod](#uvod)
- [Problem koji Photon Fusion rešava](#problem-koji-photon-fusion-rešava)
- [Šta je Photon Fusion 2](#šta-je-photon-fusion-2)
- [Ključne karakteristike](#ključne-karakteristike)
- [Poređenje sa alternativnim rešenjima](#poređenje-sa-alternativnim-rešenjima)
- [Opis demonstracionog projekta](#opis-demonstracionog-projekta)
- [Arhitektura rešenja](#arhitektura-rešenja)
- [Implementirane funkcionalnosti](#implementirane-funkcionalnosti)
- [Implementacioni detalji Photon Fusion-a](#implementacioni-detalji-photon-fusion-a)
- [Pokretanje projekta](#pokretanje-projekta)
- [Zaključak](#zaključak)
- [Mogućnosti daljeg razvoja](#mogućnosti-daljeg-razvoja)

---

## Uvod

U okviru ovog tutorijala prikazana je primena **Photon Fusion 2** mrežne biblioteke u **Unity** okruženju, sa ciljem demonstracije osnovnih principa razvoja real-time multiplayer igara.

Kroz jednostavan demonstracioni projekat prikazano je:
- povezivanje više klijenata u istu sesiju,
- sinhronizacija kretanja dva igrača,
- detekcija hit-a (pogotka),
- reagovanje na hit (pogodak),
- korišćenje **RPC** poziva za obaveštavanje svih klijenata o smrti igrača

Tutorijal je koncipiran kao **praktičan primer** za Photon Fusion, sa fokusom na razumevanje osnovnih koncepata i mogućnosti lakog proširenja.

---

## Problem koji Photon Fusion rešava

Razvoj multiplayer igara nosi niz kompleksnijih problema, među kojima su najvažniji:
- sinhronizacija stanja između više klijenata,
- minimizacija mrežne latencije,
- konzistentnost simulacije,
- autoritet nad podacima (server vs klijent),
- skalabilnost i stabilnost mrežne komunikacije.

Implementacija ovih sistema „od nule“ zahteva veliko iskustvo i značajno povećava kompleksnost projekta. **Photon Fusion** postoji upravo da bi ove probleme apstrahovao i omogućio developerima da se fokusiraju na gameplay logiku.

---

## Šta je Photon Fusion 2

Photon Fusion 2 je moderna **real-time networking** biblioteka za Unity, namenjena razvoju multiplayer igara koje zahtevaju brzu i pouzdanu sinhronizaciju.

Fusion koristi **tick-based** simulaciju i jasno definisane modele autoriteta, čime omogućava determinističko ponašanje i dobru kontrolu nad mrežnim stanjem.

Biblioteka je posebno pogodna za:
- FPS igre,
- akcione igre
- kompetitivne multiplayer scenarije,
- real-time kooperativne igre.

---

## Ključne karakteristike

- **NetworkObject & NetworkBehaviour** - mrežni evivalenti Unity komponenti, GameObject i MonoBehaviour
- **State Authority / Input Authority** - jasan model vlasništva nad objektima
- **Tick-based simulacija** - determinističko ponašanje
- **RPC (Remote Procedure Calls)** - jednostavna komunikacija između klijenata
- **Lag Compensation** - korekcija kašnjenja kod hit detekcije
- **Host / Shared / Client-Server modeli** - fleksibilna arhitektura sesije

---

## Poređenje sa alternativnim rešenjima

| Rešenje            | Prednosti                                    | Nedostaci                        |
|--------------------|----------------------------------------------|----------------------------------|
| Photon Fusion      | Brzina, determinističnost, RPC, lag comp     | Veća kompleksnost u startu       |
| Photon PUN         | Jednostavniji za početnike                   | Lošija skalabilnost              |
| Mirror             | Open-source, fleksibilan                     | Više manuelne logike             |
| Netcode for GO     | Dobra Unity integracija                      | Ograničene napredne funkcije     |

Photon Fusion je izabran jer pruža **najbolji balans performansi i kontrole** za real-time igre.

---

## Opis demonstracionog projekta

Projekat predstavlja jednostavnu multiplayer scenu sa dva igrača:
- svaki igrač kontroliše svog karaktera,
- kretanje je sinhronizovano između klijenata,
- igrači mogu da pogode jedan drugog,
- nakon smrti igrač se uklanja iz scene, ali ostaje povezan u sesiji.

Cilj projekata nije kompletna igra, već jasna demonstracija osnovnih koncepara Photon Fusion biblioteke.

---

## Arhitektura rešenja

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

## Implementirane funkcionalnosti

- Povezivanje dva igrača u istu sesiju
- Sinhronizovano kretanje igrača
- Hit detekcija
- Ragovanje na pogodak
- RPC poziv za „death event“
- Uklanjanje NetworkObject-a bez diskonekcije

---

## Implementacioni detalji Photon Fusion-a

### INetworkRunnerCallbacks

Photon Fusion koristi interfejs **INetworkRunnerCallbacks** kao centralni mehanizam za obaveštavanje aplikacije o svim bitnim mrežnim događajima.

Klasa koja implementira ovaj interfejs dobija callback metode za:
- povezivanje i diskonektovanje igrača,
- kreiranje i gašenje sesije,
- razmenu inputa,
- spawn i despawn mrežnih objekata,
- promene stanja mreže

U ovom projektu klasa `BasicSpawner` implementira `INetworkRunnerCallbacks` i služi kao **glavni mrežni kontroler sesije**.

### NetworkRunner

Photon Fusion koristi **NetworkRunner** kao centralnu komponentu mrežne simulacije.
Runner je odgovoran za:
- pokretanje i održavanje multiplayer sesije,
- tick-based mrežnu simulaciju,
- prikupljanje i distribuciju inputa,
- sinhronizaciju NetworkObject-a,
- lag compensation mehanizam.

### Kreiranje multiplayer sesije

Sesija se pokreće pozivom StartGame() metode.
```csharp
await _runner.StartGame(new StartGameArgs
{
    GameMode = mode,
    SessionName = "TestRoom",
    Scene = scene,
    SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
});
```
Objašnjenje parametara:
- GameMode
  - Host - instanca je server i klijent
  - Client - instanca je samo klijent
- SessionName
  - identifikator sesije
  - svi klijenti sa istim imenom ulaze u istu sesiju
- Scene / SceneManager
  - omogućava sinhroniyovano učitavanje scene
  - svi klijenti imaju isti svet

### OnPlayerJoined

Metoda se izvršava na svim klijentima, ali je spawn dozvoljen samo serveru (StateAuthority).
Svakom igraču se dodeljuje NetworkObject.
PlayerRef predstavlja mrežni identitet igrača i koristi se kao ključ za praćenje igrača.


### NetworkObject i mrežni identitet objekata

Svaki objekat koji treba da postoji i bude sinhronizovan u mreži mora imati **NetworkObject** komponentu.
U ovom projektu svaki igrač je NetworkObject, što znaći da:
- postoji jedan autoritivan izvor istine,
- stanje se replicira svim klijentima,
- Fusion automatski vodi računa o životnom ciklusu objekata

| GameObject | NetworkObject |
|-----------|---------------|
| Lokalan objekat | Mrežni objekat |
| Ne učestvuje u mreži | Sinhronizovan preko Runner-a |
| MonoBehaviour | NetworkBehaviour |
| Nema autoritet | Ima State / Input Authority |

### Struktura Player Prefab-a

<img width="1165" height="982" alt="image" src="https://github.com/user-attachments/assets/c918a370-79cc-480c-9618-9e0aaf8bbd40" />

Player objekat u svojoj hierarhiji ima sledeće objekte:
- Body - objekat koji sadrži MeshRenderer i MeshFilter i generalno služi samo za vizuelni prikaz (renderovanje) objekta
- Gun - ovaj objekat je igračevo oružje koji pored standarnih komponenti za renderovanje sadrži i komponente (skripte) Weapon i WeaponHandler
- PlayerCamera - objekat koji sadrži kameru  kroz koju igrač vidi svet, ali i Canvas kako bi se renderovao nišan
- HitBox - Photon fusion zahteva posebnu komponentu za detekciju kolizije pomoću Raycast-a umesto sandradnog Unity collider-a, tako da ovaj objekat sadrži komponentu HitBox

Komponente na Player objektu:
- NetworkObject
- CharacterController - Unity komponenta koja pruža osnovno kretanje igrača, takođe zahtevano je da ova komponenta postoji na objektu koji na sebi ima NetworkCharacterControllerFPS komponentu
- HitboxRoot - PhotonFusion komponenta koja omogućava preciznu detekciju kolizije
- Player - skripta koja kontrolise igrača
- NetworkCharacterControllerFPS - kopija PhotonFusion skripte NetworkCharacterController koja omogućava osnovno upravljanje igračem, uz prilagođavanje skripte kako bi se kretanje dešavalo po FPS pravilima
- HPHandler - skprita koja omogućava reakciju na hit

### NetwokBehaviour vs MonoBehaviour

**NetworkBehaviour** je mrežna ekstenzija **MonoBehaviour** klase.
Omogućava:
- pristup `Runner` instanci
- korišćenje `[Networked]` property-ja
- korišćenje i override specijalnih Photon Fusion funkcija za mrežnu sinhronizaciju (FixedUpdateNetwork, Spawned, Render ...)

Generalno pravilo za korišćenje ove dve klase je:
- **MonoBehavior** - lokalna logika
- **NetworkBehavior** - sve što utiče na mrežno stanje

### Mrežni Input

Photon Fusion koristi centralizovan input sistem gde se input svakog igrača šalje Runner-u i koristi u mrežnoj simulaciji.
U projektu je definisana sledeća struktura:
```csharp
public struct NetworkInputData : INetworkInput
{
    public Vector2 movementInput;
    public Vector2 rotationInput;
    public NetworkBool isJumpPressed;
    public NetworkBool isFirePressed;
}
```
`NetworkBool` se koristi kao zamena klasićnog `bool` tipa jer je optimizovan za mrežni prenos i determinističku simulaciju. 

### Obrada Input-a

Obrada inputa u Photon Fusion-u se sastoji iz dva jasno odvojena koraka:
1. **Prikupljanje inputa (OnInput)** - lokalno, na klijentu
   - Input se ne čita direktno u Player skripti. Umesto toga, Fusion zahteva da se sav input prikuplja u `OnInput()` callback-u, koji se najčešće nalazi u klasi koja upravlja sesijom (u ovo slučaju `BasicSpawner`).
```csharp
public void OnInput(NetworkRunner runner, NetworkInput input)
{
  NetworkInputData data = new NetworkInputData();
  data.movementInput=new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"));
  data.rotationInput=new Vector2(Input.GetAxisRaw("Mouse X"),Input.GetAxisRaw("Mouse Y")*-1);
  data.isJumpPressed=Input.GetButton("Jump");
  data.isFirePressed=Input.GetButton("Fire1");
  input.Set(data);
}
```
   - Ova funkcija se poziva samo na klijentu koji ima InputAuthority
2. **Korišćenje inputa (GetInput)** - u mrežnoj simulaciji
   - Nakon što Runner prikupi input od svih klijenata, on ga koristi tokom mrežne simulacije.
   - U Player skripti unutar FixedUpdateNetwork funckije proverava se da li je bilo inputa u trenutnom mrežnom tick-u
  ```csharp
  public override void FixedUpdateNetwork()
  {
     if (!GetInput(out NetworkInputData data)) return;
  }
  ```

### Sinhornizacija kretanja igrača

Kretanje igrača se izvršava unutar FixedUpdateNetwork funkcije u Player skripti. 

Nije potrebno proveravati da li ovaj objekat ima StateAuthority jer se interno u NetworkCharacterControllerFPS klasi to već proverava.

Kako bi se igrač zaista kretao na način kao u FPS igrama, potrebno je u kretanje uračunati u pravac pogleda igrača (rotaciju kamera igrača). 
Ovaj deo inputa ne proverava NetworkCharacterControllerFPS pa je zato potrebno proveriti da li objekat koji izvršava ovaj tick ima InputAuthority, i ako da potrebno je sinhornizovati lokalnu rotaciju lokalne kamere na x osi (takozvani Pitch).
Pitch je označen kao Networked kako bi skripta Weapon mogla da sinhornizuje rotaciju oružja sa rotaciom kamere.
Rotacija oko y ose direktno se izvršava na glavnom objektu, objektu koji i sadrži Player skriptu, a koji je takođe sadrži i NetworkObject komponentu. S obzirom da postoji NetworkObject komponenta na ovom objektu, interno postoji i NetworkTransform komponenta,
pa se rotacije ovog objekta direktno sinhornizuje i na svim njegovim child objektima u hijerarhiji.

```csharp
public override void FixedUpdateNetwork()
{
  if (_tr == null) return;
  if (!GetInput(out NetworkInputData data)) return;
  Vector3 moveDirection = _tr.forward * data.movementInput.y + _tr.right * data.movementInput.x;
  moveDirection.Normalize();
  _characterController.Move(moveDirection);
  
  if(Object.HasInputAuthority)
      _playerLocalCameraTr.localRotation=Quaternion.Euler(Pitch,0,0);
  Pitch+= data.rotationInput.y * _characterController.rotationSpeed * Runner.DeltaTime;
  Pitch = Mathf.Clamp(Pitch, -60, 60); 
  
  if (!Object.HasStateAuthority) return;
  
  _yaw += data.rotationInput.x * _characterController.rotationSpeed * Runner.DeltaTime;
  _tr.rotation = Quaternion.Euler(0, _yaw, 0);

 
  
  bool jumpPressed = data.isJumpPressed && !_jumpWasPressed;
  _jumpWasPressed = data.isJumpPressed;
  if (jumpPressed)
      _characterController.Jump();
  
}
```

### StateAuthority vs InputAuthority

Photon Fusion jasno razdvaja dva koncepta autoriteta:
- Input Authority - onaj ko šalje input, odnosno Client
- State Authority - onaj ko menja mrežno stanje, odnosno u ovom slučaju Host

### Networked property i sinhronizacija stanja

Promenljive označene `[Networked]`:
- automatski se repliciraju,
- deo su mrežnog stanje,
- mogu se menjati samo od strane StateAuthority

Photon Fusion omogućava detekciju promene ovakvih property-ja. Primer možemo videti u `WeaponHandler` skripti gde oružje puca samo ako je flag `IsFiring` prešao u stanje `true`. 
```csharp
private ChangeDetector _changeDetector;

public override void Spawned()
{
  _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
}

public override void Render()
{
  if(_changeDetector.DetectChanges(this).Changed("IsFiring"))
      OnFireChanged();
}
```


### LagCompensation i Detekcija Hit-a

Photon Fusion koristi LagCompensation da bi obezbedio fer hit detekciju u multiplayer okruženju. 
Fusion privremeno „vraća“ stanje sveta u prošlost, u trenutak kada je igrač pritisnuo dugme za pucanje, čime se eliminiše usticaj mrežne latencije.
```csharp
Runner.LagCompensation.Raycast(aimPoint.position, aimPoint.forward, 100,Object.InputAuthority, out var hitInfo, playerLayers, HitOptions.IncludePhysX);

if (hitInfo.Hitbox != null && Object.HasStateAuthority)
{
   var player=hitInfo.Hitbox.transform.root.GetComponent<HPHandler>();
   if(player!=null) player.TakeDamage();
}
```

### RPC komunikacija i sinhronizacija stanja

RPC (Remote Procedure Call) se koristi za sinhornizaciju svih klijenata da je igrač pogođen.
Primer:
- lokalni igrač je pogođen i ostaje bez energije (health-a),
- RPC se šalje svim klijentima,
- svaki klijent uklanja odgovarajućeg igrača iz scene pozivom Destroy(gameObject), što uništava objekat u sceni, ali ga ne diskonektuje.

Na ovaj način svi klijenti imaju konzistentno stanje, i ostavlja se mogućnost za ponovno stvaranje (respawn) ubijenog igrača.
Važno je napomenuti da Photon Fusion 2 za razliku od Photon Fusion 1 zahteva da imena RPC funkcija počinju ili se završavaju sa Rpc (RPC).
```csharp
[Rpc(RpcSources.InputAuthority, RpcTargets.All)]
public void RpcOnDeath()
{
    Destroy(gameObject);
}
```

---

##  Pokretanje projekta

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

<img width="1160" height="1092" alt="image" src="https://github.com/user-attachments/assets/30fe5a4d-db1f-419f-9213-2f1435d0149e" />
<img width="800" height="564" alt="image" src="https://github.com/user-attachments/assets/7cf4ee83-ef90-478e-8390-cca442fe99b6" />

4. Otvoriti Network Project Settings i omogućiti LagCompensation

<img width="1165" height="1093" alt="image" src="https://github.com/user-attachments/assets/198026f5-2189-41be-b811-8aa55b93147a" />
<img width="559" height="1015" alt="image" src="https://github.com/user-attachments/assets/9aedbea2-f879-451e-acf6-d385deb139e5" />

5. Pokrenuti dva build-a projekta, ili jedan build i play mode unutar Unity editora

---

## Zaključak

Ovaj tutorijal demonstrira osnovnu upotrebu Photon Fusion 2 biblioteke u Unity-ju kroz praktičan mupltiplayer primer. 
Prikazani su ključni koncepti mrežne sinhronizacije, autoriteta i RPC komunikacije, uz fokus na jasnoću i mogućnost lakog proširenja.

Photon Fusion se pokazuje kao moćno rešenje za real-time multiplayer igre, posebno u scenarijima gde su performanse i konzistentnost od ključnog značaja.

---

## Mogućnosti daljeg razvoja

- Vizuelni health bar sistem
- Respawn mehanika
- Matchmaking sistem
- Host migration

