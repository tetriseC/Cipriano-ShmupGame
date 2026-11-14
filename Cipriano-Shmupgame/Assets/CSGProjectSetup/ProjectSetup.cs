/************************************************************
 * COPYRIGHT:  2026
 * PROJECT:    CSG Unity Utilities
 * FILE NAME:  ProjectSetup.cs
 * DESCRIPTION: Unity Editor menu script to automatically generate 
 *              a standardized project folder structure under Assets,
 *              configure Player Settings (Company Name, App Icon, Splash Screen),
 *              install default packages via UPM, and generate colored
 *              Hierarchy Folders via the SimpleUnityHierarchyFolder plugin.
 * REVISION HISTORY:
 * Date [YYYY/MM/DD] | Author | Comments
 * ------------------------------------------------------------
 * 2026/10/15 | Akram Taghavi-Burris | Created class
 * 2026/10/26 | Akram Taghavi-Burris | Expanded PlayerSettings automation,
 *                                    added branding asset handling, resolved
 *                                    obsolete APIs, and fixed color serialization.
 *
 ************************************************************/

using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace CSGProjectSetup
{
    #region HierarchyFolderSetup
    /// <summary>
    /// Encapsulates structural metadata for scene hierarchy organizational folders,
    /// pairing a category display name with a predefined plugin color identifier.
    /// </summary>
    public struct HierarchyFolderDefinition
    {
        /// <summary>
        /// Gets or sets the visual display name of the hierarchy folder object.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the predefined color string corresponding to the 
        /// <c>SimpleUnityHierarchyFolder</c> plugin inspector dropdown selection.
        /// </summary>
        public string ColorName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchyFolderDefinition"/> struct.
        /// </summary>
        /// <param name="name">The visual label display name for the hierarchy folder.</param>
        /// <param name="colorName">The predefined color string (e.g., "Red", "Orange", "Green").</param>
        public HierarchyFolderDefinition(string name, string colorName)
        {
            Name = name;
            ColorName = colorName;
        } // end HierarchyFolderDefinition()
        
    } // end struct HierarchyFolderDefinition
    
    #endregion

    /// <summary>
    /// Provides automated Unity Editor menu commands under <c>Tools > CSG > Setup</c> to streamline
    /// new project initialization, folder structure generation, project metadata configuration,
    /// core package installation, and standard scene hierarchy assembly.
    /// </summary>
    public static class ProjectSetup
    {
        #region Fields
        /// <summary>
        /// Holds reference to the active Package Manager installation request instance.
        /// </summary>
        private static AddRequest packageAddRequest;

        /// <summary>
        /// Defines the standard company name assigned in Player Settings.
        /// </summary>
        private const string DefaultCompanyName = "University of Tulsa";

        /// <summary>
        /// Defines the default product title assigned in Player Settings.
        /// </summary>
        private const string DefaultProductName = "CSG Unity Project";

        /// <summary>
        /// Absolute asset path to the custom splash screen logo image file.
        /// </summary>
        private const string LogoTexturePath = "Assets/CSGProjectSetup/CSG-Utulsa-Logo.png";

        /// <summary>
        /// Absolute asset path to the primary application build icon texture file.
        /// </summary>
        private const string AppIconPath = "Assets/CSGProjectSetup/game-icon.png";

        /// <summary>
        /// Array of default subdirectories to generate sequentially within <c>Assets/</c>.
        /// </summary>
        private static readonly string[] DefaultFolders = 
        {
            "Audio",
            "Audio/Music",
            "Audio/SoundFX",
            "InputActions",
            "Materials",
            "Materials/Textures",
            "Models",
            "Prefabs",
            "Scripts",
            "Sprites",
            "UI",
            "UI/Fonts"
        };

        /// <summary>
        /// Array of package identifiers (npm registry keys or Git URLs) to automatically install.
        /// </summary>
        private static readonly string[] DefaultPackages = 
        {
            "com.unity.inputsystem",
            "com.unity.cinemachine",
            "https://github.com/ProfessorAkram/SimpleUnityHierarchyFolder.git"
        };

        /// <summary>
        /// Array of default scene hierarchy folder configurations mapping category roles to predefined plugin colors.
        /// </summary>
        private static readonly HierarchyFolderDefinition[] DefaultHierarchyFolders = 
        {
            new HierarchyFolderDefinition("Testing",     "Red"),
            new HierarchyFolderDefinition("Actors",      "Orange"),
            new HierarchyFolderDefinition("Interactables", "Pink"),
            new HierarchyFolderDefinition("Environment", "Green"),
            new HierarchyFolderDefinition("Lights",      "Yellow"),
            new HierarchyFolderDefinition("UI",          "Blue"),
            new HierarchyFolderDefinition("Cameras",     "Cyan"),
            new HierarchyFolderDefinition("Audio",       "Purple"),
            new HierarchyFolderDefinition("Managers",    "Silver")
        };
        #endregion
        
        #region Menu Commands 

        /// <summary>
        /// Batch command that sequentially executes all initialization steps: directory creation,
        /// project settings configuration, package installation, and scene hierarchy folder creation.
        /// </summary>
        [MenuItem("Tools/CSG/Setup/Run Full Project Setup", false, 0)]
        public static void RunFullSetup()
        {
            CreateDefaultFolders();
            ConfigureProjectSettings();
            InstallDefaultPackages();
            CreateSceneHierarchyFolders();
            
        } // end RunFullSetup()

        /// <summary>
        /// Generates the standard folder hierarchy inside <c>Assets/</c>, injects <c>.gitkeep</c> files 
        /// to ensure Git tracks empty directories, and forces an AssetDatabase refresh.
        /// </summary>
        [MenuItem("Tools/CSG/Setup/Create Default Folders", false, 1)]
        public static void CreateDefaultFolders()
        {
            string rootPath = Application.dataPath;

            foreach (string folderPath in DefaultFolders)
            {
                string fullPath = Path.Combine(rootPath, folderPath);

                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);

                    // Create .gitkeep file so empty directories are preserved in Git repositories
                    string gitkeepPath = Path.Combine(fullPath, ".gitkeep");
                    if (!File.Exists(gitkeepPath))
                    {
                        File.Create(gitkeepPath).Dispose();
                    } // end if (!File.Exists(gitkeepPath))

                    Debug.Log($"[ProjectSetup] Created Directory: Assets/{folderPath}");
                } // end if (!Directory.Exists(fullPath))
                
            } // end foreach (string folderPath in DefaultFolders)

            AssetDatabase.Refresh();
        } // end CreateDefaultFolders()

        /// <summary>
        /// Configures project-wide Player Settings including Company Name, Product Name, 
        /// application build icons, and custom splash screen logos and background colors.
        /// </summary>
        [MenuItem("Tools/CSG/Setup/Configure Project Settings", false, 2)]
        public static void ConfigureProjectSettings()
        {
            // Assign basic Player metadata
            PlayerSettings.companyName = DefaultCompanyName;
            PlayerSettings.productName = DefaultProductName;

            // Configure Application Build Icon using non-obsolete SetIcons API
            EnsureTextureImporterSettings(AppIconPath, TextureImporterType.Default, 512);
            Texture2D iconTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(AppIconPath);

            if (iconTexture != null)
            {
                PlayerSettings.SetIcons(NamedBuildTarget.Unknown, new Texture2D[] { iconTexture }, IconKind.Application);
                Debug.Log($"[ProjectSetup] Applied Application Build Icon from: {AppIconPath}");
            } // end if (iconTexture != null)
            else
            {
                Debug.LogWarning($"[ProjectSetup] Application Icon asset not found at path: {AppIconPath}. Please ensure the texture file exists.");
            } // end else

            // Configure Splash Screen Logo & Background Color
            EnsureTextureImporterSettings(LogoTexturePath, TextureImporterType.Sprite, 2048);
            Sprite logoSprite = AssetDatabase.LoadAssetAtPath<Sprite>(LogoTexturePath);

            // Enable splash screen and set background color (#003595)
            PlayerSettings.SplashScreen.show = true;
            PlayerSettings.SplashScreen.drawMode = PlayerSettings.SplashScreen.DrawMode.AllSequential;
            
            if (ColorUtility.TryParseHtmlString("#003595", out Color backgroundColor))
            {
                PlayerSettings.SplashScreen.backgroundColor = backgroundColor;
                Debug.Log("[ProjectSetup] Applied Splash Screen Background Color: #003595");
            } // end if (ColorUtility.TryParseHtmlString("#003595", out Color backgroundColor))

            if (logoSprite != null)
            {
                // Instantiate SplashScreenLogo struct and set logo & duration properties directly
                PlayerSettings.SplashScreenLogo customLogo = new PlayerSettings.SplashScreenLogo();
                customLogo.logo = logoSprite;
                customLogo.duration = 2.5f;

                PlayerSettings.SplashScreen.logos = new PlayerSettings.SplashScreenLogo[] { customLogo };

                Debug.Log($"[ProjectSetup] Applied Splash Screen Logo from: {LogoTexturePath}");
            } // end if (logoSprite != null)
            else
            {
                Debug.LogWarning($"[ProjectSetup] Splash Logo asset not found at path: {LogoTexturePath}. Please ensure the sprite file exists.");
            } // end else

            Debug.Log($"[ProjectSetup] Project Settings configured: Company = '{PlayerSettings.companyName}'");
        } // end ConfigureProjectSettings()

        /// <summary>
        /// Triggers asynchronous installation of predefined default packages via Unity's Package Manager API.
        /// </summary>
        [MenuItem("Tools/CSG/Setup/Install Default Packages", false, 3)]
        public static void InstallDefaultPackages()
        {
            InstallPackagesSequentially(0);
        } // end InstallDefaultPackages()

        /// <summary>
        /// Generates categorical Hierarchy Folder objects in the active scene using the native menu item 
        /// from the <c>SimpleUnityHierarchyFolder</c> plugin. If the package is not installed, it automatically 
        /// triggers installation first before building the folders.
        /// </summary>
        [MenuItem("Tools/CSG/Setup/Create Scene Hierarchy Folders", false, 4)]
        public static void CreateSceneHierarchyFolders()
        {
            // Verify if the package type is already available in the loaded assemblies
            Type folderType = Type.GetType("ProfessorAkram.SimpleUnityHierarchyFolder.HierarchyFolder, ProfessorAkram.SimpleUnityHierarchyFolder")
                           ?? Type.GetType("ProfessorAkram.SimpleUnityHierarchyFolder.HierarchyFolder");

            if (folderType == null)
            {
                Debug.Log("[ProjectSetup] Hierarchy Folder package missing. Initiating automatic installation...");
                EnsurePackageInstalledAndRun("https://github.com/ProfessorAkram/SimpleUnityHierarchyFolder.git", BuildSceneHierarchyFoldersInternal);
            } // end if (folderType == null)
            else
            {
                BuildSceneHierarchyFoldersInternal();
            } // end else
        } // end CreateSceneHierarchyFolders()
        
        /// <summary>
        /// Creates the configured hierarchy folders and assigns each folder its color.
        /// </summary>
        private static void BuildSceneHierarchyFoldersInternal()
        {
            foreach (var def in DefaultHierarchyFolders)
            {
                // Skip folders that already exist.
                if (GameObject.Find(def.Name) != null)
                {
                    continue;
                }

                // Create one folder.
                GameObject folderObj = CreateHierarchyFolder(def.Name);

                if (folderObj == null)
                {
                    Debug.LogError(
                        $"[ProjectSetup] Failed to create folder '{def.Name}'.");
                    continue;
                }

                // Assign the color before moving to the next folder.
                SetHierarchyFolderColor(folderObj, def.ColorName);

                Undo.RegisterCreatedObjectUndo(
                    folderObj,
                    $"Create Hierarchy Folder {def.Name}");
            }

            EditorSceneManager.MarkSceneDirty(
                EditorSceneManager.GetActiveScene());

            EditorApplication.RepaintHierarchyWindow();

            Debug.Log(
                "[ProjectSetup] Created structured hierarchy folders using SimpleUnityHierarchyFolder.");
        } // end BuildSceneHierarchyFoldersInternal()


        /// <summary>
        /// Creates a single hierarchy folder using the plugin's menu command.
        /// Falls back to creating a GameObject if the menu command fails.
        /// </summary>
        /// <param name="folderName">The name assigned to the new folder.</param>
        /// <returns>The created folder GameObject.</returns>
        private static GameObject CreateHierarchyFolder(string folderName)
        {
            bool menuExecuted =
                EditorApplication.ExecuteMenuItem(
                    "GameObject/Create Hierarchy Folder");

            GameObject folderObj = null;

            if (menuExecuted && Selection.activeGameObject != null)
            {
                folderObj = Selection.activeGameObject;
                folderObj.name = folderName;
            }
            else
            {
                folderObj = new GameObject(folderName);

                // Preserve the existing fallback behavior.
                Type folderType =
                    Type.GetType(
                        "ProfessorAkram.SimpleUnityHierarchyFolder.HierarchyFolder, ProfessorAkram.SimpleUnityHierarchyFolder")
                    ?? Type.GetType(
                        "ProfessorAkram.SimpleUnityHierarchyFolder.HierarchyFolder");

                if (folderType != null)
                {
                    folderObj.AddComponent(folderType);
                }
            }

            Debug.Log(
                $"[ProjectSetup] Created folder '{folderObj?.name}'.");

            return folderObj;
        } // end CreateHierarchyFolder()


        /// <summary>
        /// Assigns a color to an existing plugin hierarchy folder.
        /// </summary>
        /// <param name="folderObj">The folder GameObject to update.</param>
        /// <param name="colorName">The configured plugin color name.</param>
        private static void SetHierarchyFolderColor(
            GameObject folderObj,
            string colorName)
        {
            if (folderObj == null)
            {
                return;
            }

            // Find the plugin component directly.
            Component folderComp = folderObj
                .GetComponents<Component>()
                .FirstOrDefault(component =>
                    component != null &&
                    component.GetType().FullName ==
                    "ProfessorAkram.SimpleUnityHierarchyFolder.HierarchyFolder");

            if (folderComp == null)
            {
                Debug.LogError(
                    $"[ProjectSetup] HierarchyFolder component not found on '{folderObj.name}'.");
                return;
            }

            // Find the serialized color property.
            SerializedObject serializedComp = new SerializedObject(folderComp);
            SerializedProperty colorProperty =
                serializedComp.FindProperty("folderColor");

            if (colorProperty == null)
            {
                Debug.LogError(
                    $"[ProjectSetup] folderColor property not found on '{folderObj.name}'.");
                return;
            }

            // Match the configured name against Unity's actual enum names.
            int colorIndex = Array.IndexOf(
                colorProperty.enumNames,
                colorName);

            if (colorIndex < 0)
            {
                Debug.LogError(
                    $"[ProjectSetup] Color '{colorName}' is not a valid plugin color.");
                return;
            }

            // Apply the color.
            colorProperty.enumValueIndex = colorIndex;
            serializedComp.ApplyModifiedProperties();

            EditorUtility.SetDirty(folderComp);
            EditorUtility.SetDirty(folderObj);

            // Verify the serialized value after assignment.
            serializedComp.Update();

            string assignedColor =
                colorProperty.enumNames[colorProperty.enumValueIndex];

            Debug.Log(
                $"[ProjectSetup] Folder '{folderObj.name}': " +
                $"requested '{colorName}', assigned '{assignedColor}'.");
        } // end SetHierarchyFolderColor()

        
        #endregion

        #region Helpers
        /// <summary>
        /// Recursively initiates package installation requests for each entry in the <see cref="DefaultPackages"/> collection.
        /// </summary>
        /// <param name="index">The zero-based index of the target package in the installation sequence.</param>
        private static void InstallPackagesSequentially(int index)
        {
            if (index >= DefaultPackages.Length)
            {
                Debug.Log("[ProjectSetup] Package installation sequence complete.");
                return;
            } // end if (index >= DefaultPackages.Length)

            string packageId = DefaultPackages[index];
            Debug.Log($"[ProjectSetup] Requesting package installation: {packageId}...");

            packageAddRequest = Client.Add(packageId);

            EditorApplication.CallbackFunction checkProgress = null;
            checkProgress = () =>
            {
                if (packageAddRequest.IsCompleted)
                {
                    EditorApplication.update -= checkProgress;

                    if (packageAddRequest.Status == StatusCode.Success)
                    {
                        Debug.Log($"[ProjectSetup] Successfully installed: {packageAddRequest.Result.packageId}");
                    } // end if (packageAddRequest.Status == StatusCode.Success)
                    else if (packageAddRequest.Status >= StatusCode.Failure)
                    {
                        Debug.LogError($"[ProjectSetup] Failed to install {packageId}: {packageAddRequest.Error.message}");
                    } // end else if (packageAddRequest.Status >= StatusCode.Failure)

                    // Advance to the next package in sequence
                    InstallPackagesSequentially(index + 1);
                } // end if (packageAddRequest.IsCompleted)
            }; // end checkProgress delegate

            EditorApplication.update += checkProgress;
        } // end InstallPackagesSequentially()
        
        /// <summary>
        /// Checks if a package is installed via UPM and downloads it if missing, executing a completion action once ready.
        /// </summary>
        /// <param name="packageUrl">The Git URL or package identifier to check and install.</param>
        /// <param name="onComplete">Callback action executed after successful package installation.</param>
        private static void EnsurePackageInstalledAndRun(string packageUrl, Action onComplete)
        {
            packageAddRequest = Client.Add(packageUrl);

            EditorApplication.CallbackFunction checkProgress = null;
            checkProgress = () =>
            {
                if (packageAddRequest.IsCompleted)
                {
                    EditorApplication.update -= checkProgress;

                    if (packageAddRequest.Status == StatusCode.Success)
                    {
                        Debug.Log($"[ProjectSetup] Package installed successfully: {packageAddRequest.Result.packageId}");
                        
                        // Force Unity to compile and load the new package assembly before invoking the callback
                        AssetDatabase.Refresh();
                        onComplete?.Invoke();
                    } // end if (packageAddRequest.Status == StatusCode.Success)
                    else if (packageAddRequest.Status >= StatusCode.Failure)
                    {
                        Debug.LogError($"[ProjectSetup] Auto-installation failed for package '{packageUrl}': {packageAddRequest.Error.message}");
                    } // end else if (packageAddRequest.Status >= StatusCode.Failure)
                } // end if (packageAddRequest.IsCompleted)
            }; // end checkProgress delegate

            EditorApplication.update += checkProgress;
        } // end EnsurePackageInstalledAndRun()

        /// <summary>
        /// Checks and updates the Unity <see cref="TextureImporter"/> settings for a target image asset 
        /// to ensure proper texture type classification and max texture sizing prior to assignment.
        /// </summary>
        /// <param name="assetPath">Relative project path to the texture asset file.</param>
        /// <param name="targetType">Target <see cref="TextureImporterType"/> setting (e.g., Sprite or Default).</param>
        /// <param name="maxSize">Maximum permitted texture dimension size in pixels.</param>
        private static void EnsureTextureImporterSettings(string assetPath, TextureImporterType targetType, int maxSize)
        {
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;

            if (importer != null)
            {
                bool needsReimport = false;

                if (importer.textureType != targetType)
                {
                    importer.textureType = targetType;
                    needsReimport = true;
                } // end if (importer.textureType != targetType)

                if (importer.maxTextureSize < maxSize)
                {
                    importer.maxTextureSize = maxSize;
                    needsReimport = true;
                } // end if (importer.maxTextureSize < maxSize)

                if (needsReimport)
                {
                    importer.SaveAndReimport();
                    Debug.Log($"[ProjectSetup] Reconfigured import settings for texture asset at: {assetPath}");
                } // end if (needsReimport)
            } // end if (importer != null)
        } // end EnsureTextureImporterSettings()
        
        #endregion
        
    } // end class ProjectSetup
} // end namespace CSGProjectSetup