// =================================================================================================
// File: CSVtoSCRIPTABLEOBJECS.cs
// Author: Liam Davies (lid37)
// Supervisor: Helen Miles (hem23)
// Project: Gamifying the Curriculum: An Educational Application for Primary Education
// Date Created: February 15, 2026
// Last Modified: April 20, 2026
//
// Description:
// Editor utility script that parses CSV files from persistent data storage and 
// automatically generates categorized QuestionData ScriptableObjects for the game.
//
// Third-Party Assets / Code:
// - Logic assistance and structural debugging provided by Google Gemini API.
// - UI Assets sourced from Kenney.nl and Vecteezy (see Appendix B of report).
// - Sound assets sourced from Pixabay.
// =================================================================================================

using UnityEngine;
using UnityEditor;
using System.IO;

public class CSVToSOConverter
{
    private static string csvFolderPath = "/QuestionCreator/CSVs"; 

    // --- Editor Tools ---

    [MenuItem("Tools/Convert ALL CSVs to Questions")]
    public static void GenerateQuestions()
    {
        string inputPath = Application.persistentDataPath + csvFolderPath;
        string baseOutputPath = "Assets/Resources/Questions"; 
        // Define the source directory for CSVs and the target for ScriptableObjects

        if (!Directory.Exists(inputPath)) {
            Debug.LogError("Input folder not found at: " + inputPath);
            return;
        }

        string[] csvFiles = Directory.GetFiles(inputPath, "*.csv");

        foreach (string filePath in csvFiles)
        {
            string[] lines = File.ReadAllLines(filePath);

            for (int i = 1; i < lines.Length; i++)
            {
                string[] data = lines[i].Split(',');

                // Ensure the row contains the minimum required data columns
                if (data.Length < 7) continue;

                string questionText = data[0].Trim();
                string topicName = data[1].Trim();
                string difficultyLevel = data[2].Trim();

                // Construct the dynamic directory structure based on topic and difficulty
                string folderPath = $"{baseOutputPath}/{topicName}/{difficultyLevel}";

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // Instantiate and populate the QuestionData ScriptableObject
                QuestionData newQuestion = ScriptableObject.CreateInstance<QuestionData>();
                newQuestion.question = questionText;
                newQuestion.topic = topicName;
                newQuestion.answers = new string[] 
                { 
                    data[3].Trim(), // Index 0: Correct Answer
                    data[4].Trim(), // Index 1: Wrong Answer 1
                    data[5].Trim(), // Index 2: Wrong Answer 2
                    data[6].Trim()  // Index 3: Wrong Answer 3
                };

                // Generate a unique asset name and save it to the database
                string assetName = $"{topicName}_{difficultyLevel}_Line{i}.asset";
                string fullAssetPath = $"{folderPath}/{assetName}";
                AssetDatabase.CreateAsset(newQuestion, fullAssetPath);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("CSV Import Complete! Questions sorted by Topic and Difficulty.");
    }
}