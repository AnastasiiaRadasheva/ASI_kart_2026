# ASI Kart 2026 — Mario Kart-laadne mäng (Unity)

**Projekt:** ASI Karika 2026 koduvoor — “ASI Kart”  
**Engine:** Unity (C#)  
**Projekti kaust:** `Mario kart`  

## Lühikirjeldus

Mario Kart stiilis võidusõidumäng, kus mängija sõidab kartiga ringrajal ning võistleb arvuti juhitavate vastastega.  
Võitja selgub ringide arvu ja finišeerimise järjekorra põhjal.

---

## Nõuete katvus

### 1) Mängu põhiloogika
- Rada, millel saab ringe sõita
- Mängija juhitav kart
- Vähemalt 2 AI vastast (arvuti juhitavad kartid)
- Ringide loendamine ja võitja määramine

### 2) Kasutajaliides
- Ekraanil kuvatakse: koht (position) ja ringide arv (laps)
- Olemas on minimap (minikaart)

### 3) Mängitavus
- Loogiline liikumine, pööramine ja kiirus
- Checkpointid ja respawn viimases checkpointis
- Reset ja Pause funktsioon

---

## Kiirkäivitus (Windows build)

Kui Unity installimine ei ole vajalik, saab valmis Windows buildi alla laadida siit:

**Download (Windows):**  
https://drive.google.com/file/d/1qRN5KkBxE3u1cW3Wmh6MPqMeaPtN2yyV/view

---

## Arenduskeskkond (Unity)

### Vajalikud programmid

- **Unity Hub**
- (Valikuline) Visual Studio või Rider C# skriptide jaoks

---

### Projekti avamine

1. Ava **Unity Hub**
2. Vajuta **Add project**
3. Vali kaust **`Mario kart`**
4. Unity laeb vajalikud paketid automaatselt

---

## Käivitamine

### Variant A — Valmis mäng (Build)

1. Lae alla ZIP-fail
2. Paki lahti
3. Käivita fail **Mario-Kart.exe**

---

### Variant B — Unity Editoris

1. Ava projekt Unitys
2. Ava stseen **MainMenu**
3. Vajuta **Play**

---

## Juhtimine (Controls)

**Klaviatuur:**

- **W / ↑** — Kiirendus
- **S / ↓** — Pidur / tagurdus
- **A / ←** — Vasakule pööramine
- **D / →** — Paremale pööramine
- **T / L** — Drift
- **ESC** — Pause
- **R** — Reset (respawn viimases checkpointis)

---

## Mängureeglid

- Võitmiseks tuleb läbida **3 ringi** ja lõpetada enne AI vastaseid.
- Respawn süsteem kasutab viimast läbitud checkpointi.
- Pause peatab mängu.
- Reset viib mängija tagasi viimasesse checkpointi.

---

## Projekti struktuur

Olulisemad kaustad:

- `Mario kart/Assets/` — kood, stseenid, mudelid, helid, UI
- `ProjectVersion.txt` — Unity versioon

---

## Autoriõigused ja kasutatud assetid

Kõik kasutatud assetid pärinevad Kenney.nl tasuta varade kogust ja on CC0 litsentsiga.

---

### ASSET PACK 1 — Hexagon Kit

Source: https://kenney.nl/assets/hexagon-kit  
Author: Kenney (Kenney.nl)  
License: Public Domain (CC0)  

Used files:
- Assets/allmodels/Models(landscape)

---

### ASSET PACK 2 — Racing Kit

Source: https://kenney.nl/assets/racing-kit  
Author: Kenney (Kenney.nl)  
License: Public Domain (CC0)  

Used files:
- Assets/allmodels/Models

---

### ASSET PACK 3 — Mini Dungeon

Source: https://kenney.nl/assets/mini-dungeon  
Author: Kenney (Kenney.nl)  
License: Public Domain (CC0)  

Used files:
- Assets/allmodels/Models

---

### ASSET PACK 4 — City Kit Roads

Source: https://kenney.nl/assets/city-kit-roads  
Author: Kenney (Kenney.nl)  
License: Public Domain (CC0)  

Used files:
- Assets/allmodels/Models(3kart)
---
### ASSET PACK 5 — Modular-buildings

Source: https://kenney.nl/assets/modular-buildings
Author: Kenney (Kenney.nl)  
License: Public Domain (CC0)  

Used files:
- Assets/allmodels/Models(3new)

All assets were used in scenes:

Assets/Scenes/1player/cart1
Assets/Scenes/1player/cart2
Assets/Scenes/1player/cart3
Assets/Scenes/2player/cart1
Assets/Scenes/2player/cart2
Assets/Scenes/2player/cart3


---
## Autorid

- Anastasiia Radasheva  
- Oleksandra Ryshniak  
- Adriana Pikaljov  
- Mariia Posvystak  
