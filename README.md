# SpawnConvoyAndStockade v2

**Nouveautés**
- BLIP automatique sur le **lead du convoi** + (optionnel) sur les **escortes**
- Fichier **.ini** pour configurer la touche, les chances de convoi, distances, etc.
- Robustesse améliorée (chargement modèles, placement au sol, null-safety)
- Messages de debug optionnels

## Installation (GTA V Solo)
1) Installer **Script Hook V** et **ScriptHookVDotNet** (versions compatibles).
2) Créer/ouvrir le dossier:
```
Grand Theft Auto V/scripts/
```
3) Copier **SpawnConvoyAndStockade_v2.cs** **et** **SpawnConvoyAndStockade.ini** dans `scripts/`.
4) Lancer **GTA V (mode histoire/solo)**.
5) Appuyer sur **F7** (par défaut) pour activer/désactiver.

> ScriptHookVDotNet **compile automatiquement** les `.cs` placés dans `scripts/`.  
> Si vous préférez, vous pouvez **compiler en .dll** (voir ci-dessous), les deux formats sont supportés.

## Contrôles
- **F7** : Toggle du mod (modifiable dans l'INI).

## Options (INI)
- `ToggleKey` : touche d'activation (ex: F7)
- `SpawnDistance` : distance de spawn devant le joueur
- `ExistCheckRadius` : évite les doublons proches
- `StockadeIntervalMs` : fréquence d'apparition des fourgons
- `ConvoyCheckIntervalMs` : fréquence de vérification de convoi
- `ConvoyChancePercent` : probabilité de convoi
- `ConvoyMinLength` / `ConvoyMaxLength` : taille des convois
- `BlipsOnEscort` : BLIP sur les escortes aussi
- `DebugNotifies` : messages de debug

## Compilation en DLL (optionnel)
1) Visual Studio → **Class Library (.NET Framework)**, cible **.NET Framework 4.8**.
2) Ajouter référence à **ScriptHookVDotNet.dll** (depuis votre installation).
3) Ajouter le fichier `SpawnConvoyAndStockade_v2.cs` au projet, **Build**.
4) Placer la DLL obtenue dans `Grand Theft Auto V/scripts/`.

## Avertissement
- Mod **réservé au mode solo**. **N'utilisez pas** ce mod en GTA Online (risque de ban).
"# fourgon-GTA" 
