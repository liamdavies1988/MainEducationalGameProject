using UnityEngine;
using UnityEditor;
using System.IO;

public class CSVToSOConverter
{
    // The folder where your CSVs live
    private static string csvFolderPath = "/QuestionCreator/CSVs"; 

    [MenuItem("Tools/Convert ALL CSVs to Questions")]
    public static void GenerateQuestions()
    {
        string inputPath = Application.dataPath + csvFolderPath;
        // Changed name here to avoid conflict
        string baseOutputPath = "Assets/Resources/Questions"; 

        if (!Directory.Exists(inputPath))
        {
            Debug.LogError("Input folder not found at: " + inputPath);
            return;
        }

        // Get all CSV files
        string[] csvFiles = Directory.GetFiles(inputPath, "*.csv");

        if (csvFiles.Length == 0)
        {
            Debug.LogWarning("No CSV files found in " + inputPath);
            return;
        }

        foreach (string filePath in csvFiles)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            
            // Changed name here to 'topicPath' so it's unique
            string topicPath = baseOutputPath + "/" + fileName;

            if (!Directory.Exists(topicPath))
            {
                Directory.CreateDirectory(topicPath);
            }
            
            string[] lines = File.ReadAllLines(filePath);

            for (int i = 1; i < lines.Length; i++)
            {
                string[] data = lines[i].Split(',');
                if (data.Length < 6) continue;

                QuestionData newQuestion = ScriptableObject.CreateInstance<QuestionData>();
                newQuestion.question = data[0].Trim();
                newQuestion.topic = data[1].Trim();
                newQuestion.answers = new string[] 
                { 
                    data[2].Trim(), 
                    data[3].Trim(), 
                    data[4].Trim(), 
                    data[5].Trim() 
                };

                // Use the unique 'topicPath' here
                string assetPath = $"{topicPath}/{fileName}_Q{i}.asset";
                AssetDatabase.CreateAsset(newQuestion, assetPath);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Successfully processed {csvFiles.Length} CSV files into subfolders!");
    }
}