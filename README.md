[DOWL game : (Windows)](https://drive.google.com/file/d/1qRN5KkBxE3u1cW3Wmh6MPqMeaPtN2yyV/view?usp=sharing)
# ASI Kart 2026 — Mario Kart-laadne mäng (Unity)

**Projekt:** ASI Karika 2026 koduvoor — “ASI Kart”  
**Repo:** AnastasiiaRadasheva/ASI_kart_2026  
**Engine:** Unity (C#)  
**Projekti kaust:** `Mario kart`

Lühikirjeldus / Описание:  
Mario Kart stiilis võidusõidumäng, kus mängija sõidab kartiga ringrajal ning võistleb arvuti juhitavate vastastega. Võitja selgub ringide arvu ja finišeerimise järjekorra põhjal.

---

## Nõuete katvus / Соответствие требованиям

### 1) Mängu põhiloogika
- Rada, millel saab ringe sõita
- Mängija juhitav kart
- Vähemalt 2 AI vastast (arvuti juhitavad kartid)
- Ringide loendamine ja võitja määramine

### 2) Kasutajaliides
- Ekraanil kuvatakse: koht (position) ja ringide arv (laps)

### 3) Mängitavus
- Loogiline liikumine, pööramine ja kiirus
- Checkpointid + respawn viimases checkpointis
- Rajast väljas kiirus väheneb
- Ohtlikus alas (vesi/laava vms) toimub respawn viimases checkpointis
- Reset ja Pause

> Kui mõni punkt on teil osaliselt tehtud, muutke ülal olevad read “- [x]” / “- [ ]” stiilis.

---

## Kiirkäivitus (Windows mängijale)

Kui te ei soovi Unityt installida, saab Windowsi valmisbuildi alla laadida siit:  
**DOWL game : (Windows)** (link on repo avalehel / README-s).

---

## Arenduskeskkond / Development setup (Unity)

### 1) Vajalikud programmid
- **Unity Hub**
- **Unity Editor versioon**: vaata failist  
  `Mario kart/ProjectSettings/ProjectVersion.txt`  
  (installi Unity Hubis täpselt sama versioon)
- (valikuline) Visual Studio / Rider C# jaoks

### 2) Projekti avamine
1. Ava **Unity Hub**
2. **Add project** → vali kaust **`Mario kart`**
3. Unity laeb paketid automaatselt

### 3) Sõltuvused (Unity “requirements.txt” analoog)
Unity paketid on kirjas failis:  
`Mario kart/Packages/manifest.json`

Kui kasutate mingeid väliseid pluginaid/assette (mitte Unity Package Managerist), lisage nende info ka siia README alla.

---

## Käivitamine / Run

### Variant A — Unity Editoris
1. Ava projekt Unitys
2. Ava stseen: `[SceneNameHere]` (nt `MainMenu` / `Game` — täitke vastavalt oma projektile)
3. Vajuta **Play**

### Variant B — Build (exe)
1. File → Build Settings
2. Lisa stseenid “Scenes In Build” nimekirja
3. Build → käivita `.exe`

---

## Juhtimine / Controls

**Keyboard (näide — täitke vastavalt oma mängule):**
- W / ↑ — gaas
- S / ↓ — pidur / tagurdus
- A / ← — vasakule
- D / → — paremale
- Space — [drift / brake / item]  
- Esc — Pause
- R — Reset (respawn viimases checkpointis)

> Kui teie juhtimine on teistsugune — muutke siit.

---

## Mängureeglid / Gameplay rules

- Võitmiseks tuleb läbida **[X] ringi** ja lõpetada enne AI vastaseid.
- Kui sõidad rajast välja, kart aeglustub.
- Kui satud ohtlikku alasse (vesi/laava), siis kart respawnib viimases checkpointis.
- Respawn kasutab viimast läbitud checkpointi.

---

## Projekti struktuur

Unity standardstruktuur (olulised):
- `Mario kart/Assets/` — kood, stseenid, mudelid, helid, UI
- `Mario kart/ProjectSettings/ProjectVersion.txt` — Unity versioon
- `Mario kart/Packages/manifest.json` — paketid (sõltuvused)

---

## Autoriõigused / Copyright & credits (KOHUSTUSLIK)

Kõik kasutatud graafika, heli, font, mudelid, shaderid ja muu sisu peavad olema:
- ise loodud **või**
- vabalt kasutatavad (open source / free assets)

Kõik allikad on kirjas failis: **ASSET_CREDITS.txt**  
(iga faili kohta: asukoht, link/autor, litsents)

⚠️ Autoriõiguste rikkumine tühistab töö hindamise (0 punkti).

---

## Litsents / License
Kood: [MIT / Apache-2.0 / “All rights reserved”]  
Assedid: vaata **ASSET_CREDITS.txt**

---

## Autorid / Authors
- [Teie nimi 1]
- [Teie nimi 2]
