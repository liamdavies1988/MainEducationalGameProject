using UnityEngine;
using UnityEditor;
using System.IO;

public class CSVToSOConverter
{
    private static string csvFolderPath = "/QuestionCreator/CSVs"; 

    [MenuItem("Tools/Convert ALL CSVs to Questions")]
    public static void GenerateQuestions()
    {
        string inputPath = Application.dataPath + csvFolderPath;
        string baseOutputPath = "Assets/Resources/Questions"; 

        if (!Directory.Exists(inputPath)) {
            Debug.LogError("Input folder not found at: " + inputPath);
            return;
        }

        string[] csvFiles = Directory.GetFiles(inputPath, "*.csv");

        foreach (string filePath in csvFiles)
        {
            string[] lines = File.ReadAllLines(filePath);

            // Loop through lines (Starting at 1 to skip header row)
            for (int i = 1; i < lines.Length; i++)
            {
                string[] data = lines[i].Split(',');

                // We now need at least 7 columns (Q, Topic, Diff, C, W1, W2, W3)
                if (data.Length < 7) continue;

                string questionText = data[0].Trim();
                string topicName = data[1].Trim();
                string difficultyLevel = data[2].Trim();

                // 1. Build the dynamic path: e.g. "Assets/Resources/Questions/Maths/Easy"
                string folderPath = $"{baseOutputPath}/{topicName}/{difficultyLevel}";

                // 2. Automatically create the folders if they don't exist
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // 3. Create the Question Asset
                QuestionData newQuestion = ScriptableObject.CreateInstance<QuestionData>();
                newQuestion.question = questionText;
                newQuestion.topic = topicName;
                newQuestion.answers = new string[] 
                { 
                    data[3].Trim(), // Correct
                    data[4].Trim(), // Wrong 1
                    data[5].Trim(), // Wrong 2
                    data[6].Trim()  // Wrong 3
                };

                // 4. Save the file: e.g. "Maths_Easy_Q1.asset"
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