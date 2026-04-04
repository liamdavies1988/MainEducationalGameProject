# MainEducationalGameProject
main university educational game development 

Educational Farm Game: Project Design
This repository contains the development of a gamified educational tool for primary school students (Ages 5-7). The project focuses on three main coding challenges: a Core Question Engine, a Coin Reward System, and a Farm Mini-game.
1. Game Logic (Flowchart)
The flowchart below maps out the "Learning Loop." It shows how a player moves from setting up their character to answering questions and finally spending their rewards in the farm.
code
Mermaid
graph TD
    %% Setup Phase
    Start([Start Button]) --> PlayerType{New or Existing?}
    PlayerType -- New --> AvatarSelect[Avatar Selection & Name Input]
    AvatarSelect --> FarmSelect[Farm Style Selection]
    FarmSelect --> MainMenu[MAIN MENU HUB]
    PlayerType -- Existing --> MainMenu

    %% Hub
    subgraph Hub [Central Hub]
        MainMenu
        CoinsDisplay[Display Coins]
    end

    MainMenu --> Play[START GAME]
    MainMenu --> Farm[View Farm Layout]
    MainMenu --> Shop[Unlock Animals]

    %% Learning Loop
    Play --> Subj[Choose Subject & Difficulty]
    Subj --> Reset[Reset Counter: 0/10]
    Reset --> QLoop[Display Question]
    QLoop --> Logic{Check Answer}
    
    Logic -- Incorrect --> Sad[Avatar Face: SAD]
    Sad --> QLoop

    Logic -- Correct --> Happy[Avatar Face: HAPPY + 1 Coin]
    Happy --> Counter{Answered 10?}
    Counter -- No --> QLoop
    Counter -- Yes --> Summary[Session Summary]
    Summary --> MainMenu

    %% Farm Loop
    Farm --> Interaction[Drag/Drop Animals & Play Sounds]
    Interaction --> MainMenu
Description:
This diagram shows the step-by-step path a student takes. It highlights the non-punitive feedback loop where a wrong answer triggers a "sad face" and allows a retry, while a correct answer awards a coin and progresses the player toward their 10-question goal.
2. System Architecture (UML Class Diagram)
The UML Class Diagram acts as the technical blueprint for the C# scripts I am building in Unity.
code
Mermaid
classDiagram
    class GameManager {
        <<Singleton>>
        +PlayerData activeProfile
        +int questionsCorrect
        +ProcessAnswer(bool correct)
        +UpdateCoinTotal(int amount)
        +SaveAndExit()
    }

    class PlayerData {
        +string studentName
        +int avatarChoiceID
        +int totalCoins
        +List unlockedAnimals
        +SaveToDisk()
    }

    class QuestionManager {
        +List mathsQuestions
        +List spellingQuestions
        +GetNextQuestion(int difficulty)
        +bool VerifyChoice(int index)
    }

    class UIManager {
        +Text coinCounterDisplay
        +Text questionTextField
        +Image avatarExpression
        +SetFace(string mood)
        +ShowSummaryScreen()
    }

    class FarmManager {
        +int selectedLayoutID
        +GameObject[] animalPrefabs
        +TryPlaceAnimal(int animalID)
    }

    class AnimalAI {
        +AudioClip clickSound
        +float walkRange
        +MoveAnimal()
        +PlayAnimalSound()
    }

    %% Relationships
    GameManager "1" -- "1" PlayerData : manages
    GameManager "1" -- "1" QuestionManager : controls
    GameManager "1" -- "1" UIManager : updates
    GameManager "1" -- "1" FarmManager : triggers
    FarmManager "1" -- "*" AnimalAI : spawns
Description:
This diagram outlines the modular structure of the game. The GameManager acts as the central brain, connecting the QuestionManager (Learning) to the FarmManager (Rewards), while PlayerData ensures all progress is saved correctly.
3. Design Process & Methodology
How I designed these diagrams:
"I wanted to make sure I had a clear roadmap before I started writing any code. Having children of my own, I know that kids in the 5-7 age group need a very simple and clear loop, so I created the flowchart to make sure the game was easy to follow. I decided to use a 10-question session to give the students a sense of completion, and I built the 'Sad Face' logic so they wouldn't feel punished for mistakes but would instead be encouraged to try again.
For the technical side, I created the UML class diagram to organize my thoughts on the three main coding challenges. I realized that a central 'GameManager' was the best way to link the questions to the farm. I also made sure that every animal uses the same 'AnimalAI' script so that the walking and sound logic stays consistent as the player unlocks more of their farm. Planning this out now means I have a solid architecture to follow as I build the prototype in Unity."


Dev Logs

Project Development Logs: Gamifying the Curriculum

Developer: Liam Davies
Project: Educational Farm Game for Key Stage 1

Log 1: February 5th – February 10th

Focus: Project Initiation, Research & Ethics

Requirements Gathering: Designed an anonymous Microsoft Forms survey for primary school teachers to identify curriculum "pain points."

Standardization: Made the strategic decision to use Unity 2022.3 LTS instead of Unity 6 to ensure project stability and compatibility with University hardware.

Administrative Milestones: Finalized and submitted the formal Project Outline and Ethics Form. Defined the target demographic (Ages 5-7) based on personal observations of the "engagement gap" in secondary education.

Version Control: Established the GitHub repository. Resolved an initial hurdle with OneDrive sync conflicts by implementing a local-only development path and a robust .gitignore.

Log 2: February 15th

Focus: Core Question Engine & Automation

Automation Tooling: Developed a custom CSV-to-ScriptableObject converter. This C# Editor script allows for the mass-import of curriculum data from Excel, instantly generating individual "Question Assets."

Question Logic: Implemented the QuestionSetup script. Developed randomization logic to ensure answer buttons shuffle positions every time a question loads.

Economy Foundation: Created the GameManager using the Singleton pattern. Integrated the initial coin reward system where correct answers trigger a currency increase.

UI/UX Design: Created high-fidelity storyboards in ibis Paint X, establishing the "Avatar on Left / Chalkboard in Center" visual hierarchy.

Log 3: February 25th

Focus: UI "Juice" & Visual Feedback

Animation State Machine: Built a complex Animator Controller for the UI buttons.

Logic-Gate Refactoring: Encountered issues with Unity’s default UI triggers "auto-playing" the wrong animations. Resolved this by refactoring the Animator to use Boolean logic-gates and custom C# triggers.

Learning Feedback: Implemented the "Non-Punitive" feedback loop. Correct answers now trigger a "Pop" animation and a coin "Ding," while incorrect answers trigger a "Wobble" and a character facial expression change.

Audio Integration: Sourced and integrated high-quality audio assets from Pixabay (RibhavAgrawal) to provide satisfying auditory rewards.

Log 4: March 5th

Focus: JSON Data Persistence & Save Slots

Data Architecture: Developed a professional JSON-based Save System. Instead of saving raw images, the system serializes "Index IDs," keeping save files tiny and efficient.

Multi-Slot Logic: Implemented a 3-slot save architecture (SaveSlot_1-3). This allows multiple students to maintain independent progress on the same device.

The "Suitcase" Singleton: Enhanced the GameManager with DontDestroyOnLoad logic, ensuring player data (coins and name) persists seamlessly as the student moves between scenes.

Technical Resiliency: Successfully recovered from a corrupted Git "HEAD" reference by performing manual directory surgery and using the Git CLI to resync the repository.

Log 5: March 12th

Focus: CRUD Operations & Post-Meeting Polish

MMP Meeting Success: Presented the project progress to the department and achieved an 80% mark.

Unifying the UI: Refactored the entire UI system into a master MenuController. This master script now handles all navigation, save-slot labels, and curriculum selection across the game.

Data Security: Implemented the "Delete" functionality (System.IO.File.Delete).

UX Safety: Built a Modal Confirmation Pop-up. The system now asks for confirmation before loading or deleting a profile, adding a vital safety layer for the 5-7 age demographic.

Log 6: March 20th

Focus: Onboarding Flow & World Reconstruction

Linear Onboarding: Created a "Step-by-Step" wizard that guides the player from Character Creation to Farm Selection.

Dynamic World Building: Upgraded the CharacterLoader script. The game now reads the farmID from the JSON file and uses a loop to automatically activate the correct Tilemap layout (Sunny, Winter, or Night) upon loading.

Environment Previews: Developed a "What You See Is What You Get" (WYSIWYG) selection screen using JPEG previews of the premade farm layouts.

Rendering Innovation: Implemented a Render-to-Texture pipeline to display a live "Farm Viewport" inside the main UI using a dual-camera setup.

Current Status:

The project has successfully moved through the Research, Architecture, and Integration phases. The game currently features a fully functional automated question engine, a multi-slot persistent save system, and a dynamic world reconstruction engine.

Next Milestone: Developing the Farm Shop and Object Spawning system to allow players to spend their earned currency.