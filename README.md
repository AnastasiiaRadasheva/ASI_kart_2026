# ASI Kart 2026 — Mario Kart-laadne mäng (Unity)

**Projekt:** ASI Karika 2026 koduvoor — “ASI Kart”
**Engine:** Unity (C#)  
**Projekti kaust:** `Mario kart`

Lühikirjeldus 
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
- On minikaart

### 3) Mängitavus
- Loogiline liikumine, pööramine ja kiirus
- Checkpointid + respawn viimases checkpointis
- Reset ja Pause

---

## Kiirkäivitus (Windows mängijale)

Kui te ei soovi Unityt installida, saab Windowsi valmisbuildi alla laadida siit:  
[DOWL game : (Windows)](https://drive.google.com/file/d/1qRN5KkBxE3u1cW3Wmh6MPqMeaPtN2yyV/view?usp=sharing)

---

## Arenduskeskkond / Development setup (Unity)

### 1) Vajalikud programmid
- **Unity Hub**
- (valikuline) Visual Studio / Rider C# jaoks

### 2) Projekti avamine
1. Ava **Unity Hub**
2. **Add project** → vali kaust **`Mario kart`**
3. Unity laeb paketid automaatselt
---

## Käivitamine / Run

### Variant A — Unity Editoris
1. Lae alla zip-fail
2. Pakkige lahti
3. käivitage fail nimega „Mario-Kart“
### Variant B — Build (exe)
1. Ava projekt Unitys
2. Ava stseen: `[SceneNameHere]` (nt `MainMenu`)
3. Vajuta **Play**

---

## Juhtimine / Controls

**Keyboard (näide — täitke vastavalt oma mängule):**
- W / ↑ — gaas
- S / ↓ — pidur / tagurdus
- A / ← — vasakule
- D / → — paremale
- T / L — [drift]  
- Esc — Pause
- R — Reset (respawn viimases checkpointis)

> Kui teie juhtimine on teistsugune — muutke siit.

---

## Mängureeglid / Gameplay rules
- Võitmiseks tuleb läbida **[X] ringi** ja lõpetada enne AI vastaseid.
- Respawn kasutab viimast läbitud checkpointi.
---

## Projekti struktuur

Unity standardstruktuur (olulised):
- `Mario kart/Assets/` — kood, stseenid, mudelid, helid, UI
- `ProjectVersion.txt` — Unity versioon
---

## Autoriõigused / Copyright & credits (KOHUSTUSLIK)


# ASSET PACK 1 — Hexagon Kit

Source: https://kenney.nl/assets/hexagon-kit  
Author: Kenney (Kenney.nl)  
License: Public Domain (CC0)  
Used Assets:
- Background elements

- Assets/allmodels/Models(landscape)

=================================

# ASSET PACK 2 — Racing Kit

Source: https://kenney.nl/assets/racing-kit  
Author: Kenney (Kenney.nl)  
License: Public Domain (CC0)  
Used Assets:
- Racing track decorations

- Assets/allmodels/Models

=================================

# ASSET PACK 3 — Mini Dungeon

Source: https://kenney.nl/assets/mini-dungeon  
Author: Kenney (Kenney.nl)  
License: Public Domain (CC0)  

- Assets/allmodels/Models
=================================

# ASSET PACK 4 — City Kit Roads

Source: https://kenney.nl/assets/city-kit-roads  
Author: Kenney (Kenney.nl)  
License: Public Domain (CC0)  

- Assets/Models(3kart)


---

## Autorid / Authors
- 
- [Teie nimi 2]
