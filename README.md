# 🚛 SpawnConvoyAndStockade
### Un mod GTA V Solo — braquez des fourgons blindés et interceptez des convois !

![GTA V Gruppe 6](https://i.imgur.com/GJkV8JQ.jpeg)

## 🧠 Principe du mod

**SpawnConvoyAndStockade** est un script **ScriptHookVDotNet (C#)** pour **GTA V (mode solo uniquement)**.  
Il ajoute dynamiquement dans le monde du jeu des **fourgons blindés "Gruppe 6"** et parfois des **convois entiers escortés** de véhicules de sécurité.

Le but : recréer l’ambiance des braquages de camions blindés de manière plus vivante, variée et fun.

### ⚙️ Fonctionnement général

- Le script tourne en tâche de fond une fois activé (touche **F7**).
- Il fait apparaître périodiquement un **camion blindé "Stockade"** dans les environs du joueur.
- De temps en temps, il déclenche un **convoi complet** de véhicules :
  - 1 camion blindé principal (Gruppe 6),
  - 1 à 3 véhicules d’escorte aléatoires (SUV, Sentinel, Sheriff, Rumpo…),
  - des conducteurs PNJ armés,
  - parfois des gardes qui sortent du véhicule pour défendre le convoi.
- Certains convois s’arrêtent ou ouvrent leurs portes : à vous d’en profiter 💰

Le tout se veut **immersif, non intrusif** et idéal pour du jeu solo libre.

---

## 🕹️ Contrôles

| Touche | Action |
|--------|---------|
| **F7** | Activer / désactiver le spawn automatique |

---

## 🧩 Détails techniques

- Le script est codé en **C#** pour **ScriptHookVDotNet**.
- Il utilise les classes natives `World`, `Vehicle`, `Ped`, et les natives `TASK_VEHICLE_DRIVE_*` pour simuler des patrouilles ou des arrêts.
- Les véhicules et PNJ générés sont persistants, donc tu peux interagir librement (tirer, voler, détruire...).
- Des **blips** s’ajoutent parfois sur la mini-carte pour repérer les cibles.

### Paramètres par défaut (modifiables dans le code)
| Variable | Valeur | Description |
|-----------|---------|-------------|
| `spawnDistance` | 30.0f | Distance devant le joueur où spawn le camion |
| `stockadeIntervalMs` | 4500 | Délai entre deux tentatives de spawn |
| `convoyChancePercent` | 15 | Pourcentage de chance de faire apparaître un convoi |
| `convoyMaxLength` | 4 | Nombre max de véhicules dans un convoi |

---

## 🧱 Installation

### Prérequis
Assure-toi d’avoir installé les trois éléments suivants :
1. [Script Hook V](http://www.dev-c.com/gtav/scripthookv/)  
2. [ScriptHookVDotNet](https://github.com/crosire/scripthookvdotnet/releases)
3. Un dossier `scripts/` dans ton répertoire GTA V (crée-le si besoin)

### Étapes d’installation
1. Télécharge le fichier `SpawnConvoyAndStockade.cs` (ou le `.dll` si tu compiles toi-même).  
2. Place-le dans le dossier : Grand Theft Auto V/scripts/
3. Lance **GTA V en mode solo**.
4. Appuie sur **F7** pour activer le mod.
5. Attends quelques secondes : un fourgon ou un convoi devrait apparaître !

---

## ⚠️ Avertissement

❗ **Ne jamais utiliser ce mod en GTA Online** :  
L’utilisation de mods dans le mode en ligne entraîne un **ban immédiat**.  
Ce mod est 100 % réservé au **mode solo / histoire**.

---

## 🧰 Compilation (optionnel)

Si tu veux ta propre version compilée (.dll) :
1. Ouvre **Visual Studio** → **Créer un projet** → *Bibliothèque de classes (.NET Framework)*  
2. Cible le **.NET Framework 4.8**  
3. Ajoute une **référence à ScriptHookVDotNet.dll** (présente dans ton dossier GTA/scripts ou installation SHVDN).  
4. Colle le code source dans ton projet et **compile**.  
5. Place le fichier `.dll` généré dans `scripts/`.

---

## 📸 Aperçu du gameplay

- Fourgons blindés apparaissant naturellement sur les routes.
- Convois de sécurité se déplaçant ou s’arrêtant.
- Sécurité armée réagissant à ta présence.
- Idéal pour les braquages improvisés ou tests de gameplay RP.

---

## 🧑‍💻 Auteur
**Deax**  
Projet personnel pour enrichir le gameplay libre en solo.  
Libre à toi de le forker, l’améliorer ou d’en publier des variantes.

---

## 📜 Licence
Ce projet est distribué sous licence MIT.  
Tu peux l’utiliser, le modifier et le redistribuer librement, tant que tu respectes la licence et n’en fais pas un usage commercial non autorisé.

---

### 💬 Idées d’améliorations futures :
- Ajout de coffres d’argent ou sacs lootables après braquage  
- Police réagissant aux braquages si trop de bruit  
- Paramétrage via fichier `.ini`  
- Récompenses monétaires RP


