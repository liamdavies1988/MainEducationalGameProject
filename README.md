# Grow & Know — Gamified Key Stage 1 Learning Game
Grow & Know
<img width="1020" height="574" alt="picture1" src="https://github.com/user-attachments/assets/b05d2c04-6eff-4f9a-87b9-277bdec3d2eb" />
A gamified learning tool for ages 5–7 (Maths & Literacy), built as my BSc final-year major project.

## Tech Stack
C# · Unity 2022.3 LTS · .NET · LINQ · JSON Serialization · CSV Parsing · Custom ShaderLab

## Key Technical Achievements
- **CSV → ScriptableObject pipeline** — custom C# parser automates curriculum data import, no manual data entry
- **JSON save system** — multi-slot persistence with master profile caching; preserves progress across sessions
- **Accessibility-first design** — 6 skin tones, 61 hairstyles, toggleable hearing aids/glasses/crutches, wheelchair body types, non-verbal feedback for lower-literacy users
- **Bilingual (English/Welsh)** localisation
- **Custom screen-space → world-space raycasting** for dual-camera viewport interaction
- **Systematic testing & debugging** — resolved rendering bugs and improved UI responsiveness

## What I Learned
Building this taught me full software lifecycle discipline: requirements → architecture → implementation → testing → polish. The CSV/JSON data layer and accessibility features were the hardest parts — and the ones I'm most proud of.


AUTHOR:     Liam Davies (lid37)

SUPERVISOR: Helen Miles (hem23)

MODULE:     CS39440 - Major Project (40 Credits)

UNIVERSITY: Aberystwyth University

STATUS:     Technical Feature-Complete (Mid-Project Mark: 80%)

--------------------------------------------------------------------------------
1. PROJECT OVERVIEW
--------------------------------------------------------------------------------
A gamified tool for Key Stage 1 (Ages 5-7) designed to bridge the gap between 
curriculum learning (Maths/Literacy) and interactive play.

KEY CHALLENGES ADDRESSED:
* Dynamic Question Engine: Automated CSV-to-ScriptableObject content delivery.
* Data Persistence: Multi-slot JSON save system with Master Profile Caching.
* Reward Simulation: AI-driven farm world with deep character customization.

--------------------------------------------------------------------------------
2. TECHNICAL SPECIFICATIONS
--------------------------------------------------------------------------------
* ENGINE: Unity 2022.3.x LTS (Standardized for lab compatibility)
* LOGIC:  C# (.NET 4.x) using LINQ for data sorting
* ART:    Custom assets created in ibis Paint X
* DATA:   JSON Serialization for student profiles
* TOOLS:  VS Code + Google Gemini API for logic debugging

SYSTEM ARCHITECTURE:
* MASTER CACHING: Prevents data truncation during active learning sessions.
* VIEWPORT RENDERING: Dual-camera setup projecting a 2D world into a 1024x1024 
  UI Render-Texture.
* COORDINATE TRANSLATOR: Custom Screen-Space to World-Space raycasting for 
  interacting with the remote farm environment.

--------------------------------------------------------------------------------
3. INCLUSIVE DESIGN & ACCESSIBILITY
--------------------------------------------------------------------------------
The project prioritizes authentic representation for all students:
* VISUALS: 6 diverse skin tones and 61 unique hairstyles.
* MEDICAL AIDS: Toggleable assets for hearing aids, glasses, and crutches.
* MOBILITY: Wheelchair-specific body types and pose-adaptive meshes.
* FEEDBACK: Non-verbal, affective indicators (Emoji icons/Red-flashing UI) 
  to support students with lower literacy levels.

--------------------------------------------------------------------------------
4. GAMEPLAY LOGIC (THE LEARNING LOOP)
--------------------------------------------------------------------------------
[START] -> [AVATAR CREATION] -> [MAIN HUB]

A. LEARNING PATH:
   Choose Subject -> Question Display -> Logic Check
   - IF CORRECT: Coin Awarded + Auto-Save
   - IF INCORRECT: Non-punitive visual feedback -> Retry
   - SUCCESS: Session Summary -> Return to Hub

B. REWARD PATH:
   Visit Farm -> Open Shop -> Spend Coins -> Place/Interact with AI Animals

--------------------------------------------------------------------------------
5. SETUP & INSTALLATION
--------------------------------------------------------------------------------
1. Prerequisite: Unity 2022.3 LTS installed.
2. Deployment: Clone repo to local C: drive (Avoid OneDrive sync issues).
3. Build Scenes (In order):
   - TitleScreen
   - SaveSelection
   - PlayerCreation
   - FarmSelection
   - PlayerAndFarm
   - MultipleChoiceGame
4. Save Data Path: 
   %userprofile%\AppData\LocalLow\DefaultCompany\MainEducationalGameProject\Saves

--------------------------------------------------------------------------------
6. DEVELOPMENT LOGS (SUMMARY)
--------------------------------------------------------------------------------
* LOG 1: Initiation, Target Demographic analysis, Unity 2022.3 stabilization.
* LOG 2: CSV-to-ScriptableObject automation and JSON persistence architecture.
* LOG 3: MMP Milestone (80% score) and UI refactoring into MenuController.
* LOG 4: Accessibility expansion (Medical/Mobility aids) and asset mapping.
* LOG 5: Farm Shop LINQ integration and ViewportToFarmTranslator development.
* STATUS: Technical Freeze (Final focus on dissertation).

--------------------------------------------------------------------------------
7. CREDITS & THIRD-PARTY ASSETS
--------------------------------------------------------------------------------
* UI/Icons: Kenney.nl, graphic51, djvstock.
* Sprites:  emiltimplaru, vectoartic.
* Audio:    Pixabay (RibhavAgrawal, GeoffHarvey).
* Symbols:  Donchu & Universal Icons via The Noun Project.

Full attributions are listed in Appendix B of the Project Report.
================================================================================
