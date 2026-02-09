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