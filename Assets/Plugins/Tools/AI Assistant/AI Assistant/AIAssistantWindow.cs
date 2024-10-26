using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;

using UnityEngine;
using UnityEditor;

using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

namespace AIAssistant
{

    public class AIAssistantWindow : EditorWindow
    {
        private const float FORCE_TIMEOUT_FREE_USER = 20f;

        private const float FORCE_TIMEOUT_PAID_USER = 1f;

        private const float FAILSAFE = 0.2f;

        private const int PARALLEL_THREAD_COUNT = 8;

        const string ApiKeyErrorText =
            "API Key hasn't been set. Please check the project settings " +
            "(Edit > Project Settings > AI Command > API Key).";

        bool IsApiKeyOk
            => !string.IsNullOrEmpty(AIAssistantSettings.instance.apiKey);

        /*
        private const string PROMPT_ID_PREFIX = "This prompt's ID is: "; //"Remember this prompt as: "; //"This prompt's ID is: ";
        */

        private const string PROVIDE_DOCUMENTATION_PROMPT_PREFIX =
            "I need you to cover the following C# script with XML comments.\n"
            + "- Do NOT erase the parts of code\n" //THIS IS MOSTLY DUE TO MESSAGE LENGTH BUT HAPPENS NEVERTHELESS
            + "- DO NOT ERASE \"//\" COMMENTS\n"
            + "- Do NOT change the amount of tabs, spaces or empty lines in the code\n" //PAIN
            + "- Do NOT change indentation of code\n" //PAIN
            + "- Add XML comments ONLY. Not single-line or multi-line comments above methods/classes\n" //PAIN
            + "- DO NOT USE \"<inheritdoc/>\"\n" //PAIN
            + "- DO NOT USE inheritdoc\n" //MORE PAIN
            + "- Don't give me ```xml files, only ```cs ones\n" //PAIN
            + "- I only need the script body. Don’t add any explanation\n"
            + "- Don't add any trailing messages like \"take note\"\n"; //PAIN

        /*
        private const string RESEND_PROMPT_PREFIX =
            "I received a response with the \"finish_reason: length\" error for one of my previous prompts. The response was too long to fit within the token limit. Could you please resend the answer? I remember mentioning ";
        */

        /*
        private const string RESEND_PROMPT_CONDITIONS_POSTFIX =
            "- I need the response to one of MY prompts only.\n" //LOL
            + "- I only need the script body. Don’t add any explanation or prefix like \"Certainly!...\".\n"
            + "- Don't add any trailing messages like \"take note\".\n";
        */
        
        private static readonly string[] UNWANTED_PREFIXES = new[]
        {
            "```csharp",
            "```C#",
            "```c#",
            "```cs",
            "```xml", //PAIN
            "```",
            "\n"
        };

        private static readonly string[] UNWANTED_POSTFIXES = new[]
        {
            "```", 
            "\n"
        };

        
        private bool forceTimeout = true;

        private bool freeUser = true;

        private bool runInParallel = false;

        private bool running = false;

        
        private List<PromptCommand> promptCommands = new List<PromptCommand>();

        private ConcurrentQueue<WriteToFileCommand> writeCommands = new ConcurrentQueue<WriteToFileCommand>();

        private SourceDirectoryContext directoryContext;

        
        private SemaphoreSlim semaphore = new SemaphoreSlim(PARALLEL_THREAD_COUNT, PARALLEL_THREAD_COUNT);

        private CancellationTokenSource cancellationTokenSource;

        private DateTime lastRequestTime = default(DateTime);

        
        private ILogger logger = null;
        
        private Vector2 scrollPos;

        [MenuItem("Window/AI Assistant")]
        static void Init() => GetWindow<AIAssistantWindow>(true, "AI Assistant");

        void OnGUI()
        {
            if (IsApiKeyOk)
            {
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
                
                if (directoryContext != null)
                {
                    EditorGUILayout.BeginVertical("Box");
                    
                    EditorGUILayout.LabelField(directoryContext.Path);
                    
                    EditorGUILayout.Space(10f);

                    if (directoryContext.Files == null)
                    {
                        EditorGUILayout.LabelField("/// RETRIEVING FILES ///");
                    }
                    else
                    {
                        float maxWidth = -1f;
                    
                        foreach (var fileContext in directoryContext.Files)
                        {
                            var textDimensions = GUI.skin.label.CalcSize(new GUIContent(fileContext.FilePath));

                            if (textDimensions.x > maxWidth)
                                maxWidth = textDimensions.x;
                        }
                        
                        foreach (var fileContext in directoryContext.Files)
                        {
                            string filePath = fileContext.FilePath;

                            var status = fileContext.Status;

                            
                            EditorGUILayout.BeginHorizontal();
                            
                            
                            EditorGUILayout.BeginHorizontal();
                            
                            EditorGUILayout.LabelField(filePath, GUILayout.Width(maxWidth));
                            
                            EditorGUILayout.EndHorizontal();
                            
                            
                            GUILayout.FlexibleSpace();
                            
                            
                            EditorGUILayout.BeginHorizontal();
                            
                            EditorGUILayout.LabelField(status.ToString());

                            bool userCanInteract = !running;

                            bool statusAllowsRegeneration =
                                status == EFileProcessingStatus.UNCHANGED
                                || status == EFileProcessingStatus.DONE
                                || status == EFileProcessingStatus.ERROR
                                || status == EFileProcessingStatus.EXCEPTION
                                || status == EFileProcessingStatus.ABORTED
                                || status == EFileProcessingStatus.TOO_LONG
                                || status == EFileProcessingStatus.TOKENS_EXCEEDED;
                            
                            if (userCanInteract && statusAllowsRegeneration)
                            {
                                if (GUILayout.Button("REGENERATE"))
                                {
                                    running = true;

                                    cancellationTokenSource?.Dispose();

                                    cancellationTokenSource = new CancellationTokenSource();

                                    var task =
                                        RegenerateXMLCommentsForFile(
                                                fileContext,
                                                cancellationTokenSource.Token)
                                            .ContinueWith(
                                                logExceptionTask =>
                                                {
                                                    throw new Exception(
                                                        logger.FormatException(
                                                            GetType(),
                                                            $"{logExceptionTask.Exception.Message} STACK TRACE:\n{logExceptionTask.Exception.StackTrace}"));

                                                    running = false;
                                                },
                                                TaskContinuationOptions.OnlyOnFaulted)
                                            .ConfigureAwait(false);
                                }
                            }

                            EditorGUILayout.EndHorizontal();
                            
                            
                            EditorGUILayout.EndHorizontal();
                        }
                    }

                    EditorGUILayout.EndVertical();
                }

                if (running)
                {
                    EditorGUILayout.LabelField("/// RUNNING ///");

                    EditorGUI.BeginDisabledGroup(true);
                    
                    EditorGUILayout.Toggle("Force timeout", forceTimeout);

                    EditorGUILayout.Toggle("Free user", freeUser);

                    EditorGUILayout.Toggle("Run in parallel", runInParallel);

                    EditorGUI.EndDisabledGroup();
                    
                    if (GUILayout.Button("ABORT"))
                    {
                        cancellationTokenSource?.Cancel();
                    }
                }
                else
                {
                    forceTimeout = EditorGUILayout.Toggle("Force timeout", forceTimeout);

                    freeUser = EditorGUILayout.Toggle("Free user", freeUser);

                    runInParallel = EditorGUILayout.Toggle("Run in parallel", runInParallel);

                    if (GUILayout.Button("Find scripts in folder"))
                    {
                        if (forceTimeout && runInParallel)
                        {
                            logger.LogError(
                                GetType(),
                                "CANNOT RUN IN PARALLEL WITH FORCE TIMEOUT");

                            return;
                        }

                        string path = EditorUtility.OpenFolderPanel(
                            "Select folder to provide documentation",
                            Application.dataPath,
                            "");

                        if (string.IsNullOrEmpty(path))
                        {
                            logger.LogError(
                                GetType(),
                                "OPERATION CANCELLED");

                            return;
                        }

                        running = true;

                        cancellationTokenSource?.Dispose();

                        cancellationTokenSource = new CancellationTokenSource();

                        directoryContext = new SourceDirectoryContext(path);

                        var task =
                            FindScriptsInFolder(
                                    path,
                                    cancellationTokenSource.Token)
                                .ContinueWith(
                                    logExceptionTask =>
                                    {
                                        throw new Exception(
                                            logger.FormatException(
                                                GetType(),
                                                $"{logExceptionTask.Exception.Message} STACK TRACE:\n{logExceptionTask.Exception.StackTrace}"));

                                        running = false;
                                    },
                                    TaskContinuationOptions.OnlyOnFaulted)
                                .ConfigureAwait(false);
                    }

                    if (GUILayout.Button("Create documentation"))
                    {
                        if (forceTimeout && runInParallel)
                        {
                            logger.LogError(
                                GetType(),
                                "CANNOT RUN IN PARALLEL WITH FORCE TIMEOUT");

                            return;
                        }

                        string path = EditorUtility.OpenFolderPanel(
                            "Select folder to provide documentation",
                            Application.dataPath,
                            "");

                        if (string.IsNullOrEmpty(path))
                        {
                            logger.LogError(
                                GetType(),
                                "OPERATION CANCELLED");

                            return;
                        }

                        running = true;

                        cancellationTokenSource?.Dispose();

                        cancellationTokenSource = new CancellationTokenSource();

                        directoryContext = new SourceDirectoryContext(path);

                        var task =
                            CoverScriptsInXMLComments(
                                    path,
                                    cancellationTokenSource.Token)
                                .ContinueWith(
                                    logExceptionTask =>
                                    {
                                        throw new Exception(
                                            logger.FormatException(
                                                GetType(),
                                                $"{logExceptionTask.Exception.Message} STACK TRACE:\n{logExceptionTask.Exception.StackTrace}"));

                                        running = false;
                                    },
                                    TaskContinuationOptions.OnlyOnFaulted)
                                .ConfigureAwait(false);
                    }
                }

                EditorGUILayout.EndScrollView();
            }
            else
            {
                EditorGUILayout.HelpBox(ApiKeyErrorText, MessageType.Error);
            }
        }

        #region Find scripts in folder

        private async Task FindScriptsInFolder(
            string path,
            CancellationToken cancellationToken)
        {
            #region Initialize
            
            logger.Log(
                GetType(),
                $"INITIALIZING FINDING SOURCE FILES AT DIRECTORY {path}");

            List<SourceFileContext> sourceFiles = new List<SourceFileContext>();

            directoryContext.Status = EFolderProcessingStatus.RETRIEVING_SOURCE_FILES;
            
            #endregion

            #region Retrieve list of scripts to process
            
            await RetrieveListOfScriptsToProcess(
                path,
                sourceFiles,
                cancellationToken);

            if (cancellationToken.IsCancellationRequested)
            {
                directoryContext.Status = EFolderProcessingStatus.ABORTED;
            
                running = false;

                return;
            }

            logger.Log(
                GetType(),
                $"LIST OF SCRIPTS TO PROCESS RETRIEVED. AMOUNT: {promptCommands.Count.ToString()}");

            #endregion

            #region Finalize
            
            promptCommands.Clear();
            
            writeCommands.Clear();
            
            logger.Log(
                GetType(),
                "FINISHED");

            directoryContext.Status = EFolderProcessingStatus.DONE;

            running = false;
            
            #endregion
        }

        #endregion
        
        #region Cover scripts in XML comments
        
        private async Task CoverScriptsInXMLComments(
            string path,
            CancellationToken cancellationToken)
        {
            #region Initialize
            
            logger.Log(
                GetType(),
                $"INITIALIZING COVERING SOURCE FILES AT DIRECTORY {path} WITH XML COMMENTS");

            List<SourceFileContext> sourceFiles = new List<SourceFileContext>();
            
            promptCommands.Clear();
            
            writeCommands.Clear();

            directoryContext.Status = EFolderProcessingStatus.RETRIEVING_SOURCE_FILES;
            
            #endregion

            #region Retrieve list of scripts to process
            
            await RetrieveListOfScriptsToProcess(
                path,
                sourceFiles,
                cancellationToken);

            if (cancellationToken.IsCancellationRequested)
            {
                directoryContext.Status = EFolderProcessingStatus.ABORTED;
            
                running = false;

                return;
            }

            logger.Log(
                GetType(),
                $"LIST OF SCRIPTS TO PROCESS RETRIEVED. AMOUNT: {promptCommands.Count.ToString()}");

            #endregion

            #region Process prompt commands
            
            directoryContext.Status = EFolderProcessingStatus.PROMPTING;

            await ProcessPromptCommands(cancellationToken);

            //Just to ensure that everything that is ready to be written is written
            /*
            if (cancellationToken.IsCancellationRequested)
            {
                directoryContext.Status = EFolderProcessingStatus.ABORTED;
            
                running = false;

                return;
            }
            */
            
            #endregion

            #region Process write commands
            
            logger.Log(
                GetType(),
                $"INVOKING WRITER DELEGATES");

            await ProcessWriteCommands(cancellationToken);
            
            if (cancellationToken.IsCancellationRequested)
            {
                directoryContext.Status = EFolderProcessingStatus.ABORTED;
            
                running = false;

                return;
            }
            
            #endregion

            #region Finalize
            
            promptCommands.Clear();
            
            writeCommands.Clear();

            logger.Log(
                GetType(),
                "FINISHED");

            directoryContext.Status = EFolderProcessingStatus.DONE;

            running = false;
            
            #endregion
        }

        private async Task RetrieveListOfScriptsToProcess(
            string path,
            List<SourceFileContext> sourceFiles,
            CancellationToken cancellationToken)
        {
            await ProcessFolder(
                path,
                sourceFiles,
                cancellationToken);
            
            directoryContext.SetSourceFiles(sourceFiles.ToArray());
        }

        private async Task ProcessFolder(
            string path,
            List<SourceFileContext> sourceFiles,
            CancellationToken cancellationToken)
        {
            logger.Log(
                GetType(),
                $"PROCESSING FOLDER: {path}");

            string[] filePaths = Directory.GetFiles(path);

            foreach (string filePath in filePaths)
            {
                if (filePath.EndsWith(".cs"))
                {
                    var sourceFileContext = new SourceFileContext(filePath); 
                    
                    sourceFiles.Add(sourceFileContext);
                    
                    promptCommands.Add(
                        new PromptCommand
                        {
                            Context = sourceFileContext
                        });
                }
            }

            string[] subFolderPaths = Directory.GetDirectories(path);

            foreach (var subfolderPath in subFolderPaths)
            {
                await ProcessFolder(
                    subfolderPath,
                    sourceFiles,
                    cancellationToken);

                if (cancellationToken.IsCancellationRequested)
                {
                    directoryContext.Status = EFolderProcessingStatus.ABORTED;
                    
                    running = false;

                    return;
                }
            }
        }

        #endregion

        #region Regenerate XML comments for file

        private async Task RegenerateXMLCommentsForFile(
            SourceFileContext sourceFileContext,
            CancellationToken cancellationToken)
        {
            #region Initialize
            
            logger.Log(
                GetType(),
                $"INITIALIZING REGENERATING XML COMMENTS FOR FILE AT DIRECTORY {sourceFileContext.FilePath}");

            promptCommands.Clear();
            
            writeCommands.Clear();
            
            sourceFileContext.Status = EFileProcessingStatus.UNCHANGED;
            
            directoryContext.Status = EFolderProcessingStatus.RETRIEVING_SOURCE_FILES;
            
            #endregion

            #region Create prompt command for the source file
            
            var promptCommand = new PromptCommand
            {
                Context = sourceFileContext
            };

            promptCommands.Add(promptCommand);

            #endregion

            #region Process prompt commands
            
            directoryContext.Status = EFolderProcessingStatus.PROMPTING;

            await ProcessPromptCommands(cancellationToken);

            //Just to ensure that everything that is ready to be written is written
            /*
            if (cancellationToken.IsCancellationRequested)
            {
                directoryContext.Status = EFolderProcessingStatus.ABORTED;
            
                running = false;

                return;
            }
            */
            
            #endregion

            #region Process write commands
            
            logger.Log(
                GetType(),
                $"INVOKING WRITER DELEGATES");

            await ProcessWriteCommands(cancellationToken);
            
            if (cancellationToken.IsCancellationRequested)
            {
                directoryContext.Status = EFolderProcessingStatus.ABORTED;
            
                running = false;

                return;
            }
            
            #endregion

            #region Finalize
            
            promptCommands.Clear();
            
            writeCommands.Clear();

            logger.Log(
                GetType(),
                "FINISHED");

            directoryContext.Status = EFolderProcessingStatus.DONE;

            running = false;
            
            #endregion
            
        }

        #endregion

        private async Task ProcessPromptCommands(
            CancellationToken cancellationToken)
        {
            if (!runInParallel)
            {
                foreach (var promptCommand in promptCommands)
                {
                    await PromptSourceFile(
                        promptCommand,
                        cancellationToken);

                    if (cancellationToken.IsCancellationRequested)
                    {
                        directoryContext.Status = EFolderProcessingStatus.ABORTED;
                        
                        foreach (var promptCommandInner in promptCommands)
                        {
                            bool abortable =
                                promptCommandInner.Context.Status == EFileProcessingStatus.UNCHANGED
                                || promptCommandInner.Context.Status == EFileProcessingStatus.AWAITING_WRITE
                                || promptCommandInner.Context.Status == EFileProcessingStatus.AWAITING_TIMEOUT
                                || promptCommandInner.Context.Status == EFileProcessingStatus.PROMPTING;

                            if (abortable)
                                promptCommandInner.Context.Status = EFileProcessingStatus.ABORTED;
                        }
                        
                        //running = false;

                        return;
                    }
                }
                
                directoryContext.Status = EFolderProcessingStatus.AWAITING_WRITE;
            }
            else
            {
                Parallel.ForEach(
                    promptCommands,
                    async promptCommand =>
                    {
                        await PromptSourceFileInParallel(
                            promptCommand,
                            cancellationToken);
                    });

                int lastProcessedCount = 0;

                while (directoryContext.Status != EFolderProcessingStatus.AWAITING_WRITE)
                {
                    await Task.Yield();

                    if (cancellationToken.IsCancellationRequested)
                    {
                        directoryContext.Status = EFolderProcessingStatus.ABORTED;

                        foreach (var promptCommand in promptCommands)
                        {
                            bool abortable =
                                promptCommand.Context.Status == EFileProcessingStatus.UNCHANGED
                                || promptCommand.Context.Status == EFileProcessingStatus.AWAITING_WRITE
                                || promptCommand.Context.Status == EFileProcessingStatus.AWAITING_TIMEOUT
                                || promptCommand.Context.Status == EFileProcessingStatus.PROMPTING;

                            if (abortable)
                                promptCommand.Context.Status = EFileProcessingStatus.ABORTED;
                        }

                        //running = false;

                        return;
                    }

                    int promptCommandsProcessed = 0;

                    foreach (var promptCommand in promptCommands)
                    {
                        bool processingFinished =
                            promptCommand.Context.Status == EFileProcessingStatus.AWAITING_WRITE
                            || promptCommand.Context.Status == EFileProcessingStatus.ERROR
                            || promptCommand.Context.Status == EFileProcessingStatus.EXCEPTION
                            || promptCommand.Context.Status == EFileProcessingStatus.TOO_LONG
                            || promptCommand.Context.Status == EFileProcessingStatus.TOKENS_EXCEEDED;

                        if (processingFinished)
                            promptCommandsProcessed++;
                    }

                    if (promptCommandsProcessed != lastProcessedCount)
                    {
                        lastProcessedCount = promptCommandsProcessed;

                        logger.Log(
                            GetType(),
                            $"PROCESSED {promptCommandsProcessed} / {promptCommands.Count} PROMPT REQUESTS");
                    }
                    
                    if (promptCommandsProcessed == promptCommands.Count)
                        directoryContext.Status = EFolderProcessingStatus.AWAITING_WRITE;
                }
            }
        }

        private async Task PromptSourceFileInParallel(
            PromptCommand promptCommand,
            CancellationToken cancellationToken)
        {
            logger.Log(
                GetType(),
                "AWAITING FOR SEMAPHORE");

            await semaphore.WaitAsync(cancellationToken);

            try
            {
                logger.Log(
                    GetType(),
                    "SEMAPHORE SPINNED, PROCESSING FILE");

                await PromptSourceFile(
                    promptCommand,
                    cancellationToken);
            }
            finally
            {
                logger.Log(
                    GetType(),
                    "FINISHED, RELEASING SEMAPHORE");

                semaphore.Release();
            }
        }

        private async Task PromptSourceFile(
            PromptCommand promptCommand,
            CancellationToken cancellationToken)
        {
            string filePath = promptCommand.Context.FilePath;

            logger.Log(
                GetType(),
                $"PROMPTING SOURCE FILE: {filePath}");

            string sourceCode = await File.ReadAllTextAsync(filePath, cancellationToken);

            //string prompt = $"{PROMPT_ID_PREFIX}{promptID}\n{PROVIDE_DOCUMENTATION_PROMPT_PREFIX}\n{sourceCode}";
            
            string prompt = $"{PROVIDE_DOCUMENTATION_PROMPT_PREFIX}\n{sourceCode}";

            var httpRequest = OpenAIUtil.BuildRequest(
                prompt,
                out byte[] data);

            var gptRequest = new GPTRequest();

            gptRequest.HTTPRequest = httpRequest;

            gptRequest.Data = data;

            gptRequest.Retry = true;

            gptRequest.SourceFileContext = promptCommand.Context;
            
            promptCommand.Context.Status = EFileProcessingStatus.PROMPTING;
            
            await SendRequest(
                gptRequest,
                cancellationToken);

            if (cancellationToken.IsCancellationRequested)
            {
                promptCommand.Context.Status = EFileProcessingStatus.ABORTED;
                
                running = false;

                return;
            }

            var codeWithDocumentation = gptRequest.Response;

            if (string.IsNullOrEmpty(codeWithDocumentation))
            {
                promptCommand.Context.Status = EFileProcessingStatus.EXCEPTION;
                
                throw new Exception(
                    logger.FormatException(
                        GetType(),
                        "PROMPT RESPONSE IS EMPTY"));
            }

            foreach (var unwantedPrefix in UNWANTED_PREFIXES)
            {
                if (codeWithDocumentation.StartsWith(unwantedPrefix))
                    codeWithDocumentation = codeWithDocumentation.Substring(
                        unwantedPrefix.Length,
                        codeWithDocumentation.Length - unwantedPrefix.Length);
            }

            foreach (var unwantedPostfix in UNWANTED_POSTFIXES)
            {
                if (codeWithDocumentation.EndsWith(unwantedPostfix))
                    codeWithDocumentation = codeWithDocumentation.Substring(
                        0,
                        codeWithDocumentation.Length - unwantedPostfix.Length);
            }

            writeCommands.Enqueue(
                new WriteToFileCommand
                {
                    Context = promptCommand.Context,

                    Text = codeWithDocumentation
                });
            
            promptCommand.Context.Status = EFileProcessingStatus.AWAITING_WRITE;
        }

        private async Task SendRequest(
            GPTRequest request,
            CancellationToken cancellationToken)
        {
            bool responseReceivedAndValid = false;

            while (!responseReceivedAndValid)
            {
                if (forceTimeout)
                {
                    var currentTime = DateTime.Now;

                    var timeSinceLastRequest = currentTime - lastRequestTime;

                    float timeout = freeUser
                        ? FORCE_TIMEOUT_FREE_USER
                        : FORCE_TIMEOUT_PAID_USER;

                    if (timeSinceLastRequest.TotalSeconds < timeout)
                    {
                        TimeSpan waitDuration =
                            TimeSpan.FromSeconds(timeout - timeSinceLastRequest.TotalSeconds + FAILSAFE);

                        logger.Log(
                            GetType(),
                            $"FORCE TIMEOUT. LAST REQUEST TIME: {lastRequestTime.ToString()} TIMEOUT: {waitDuration.ToString()}");

                        request.SourceFileContext.Status = EFileProcessingStatus.AWAITING_TIMEOUT;
                        
                        await Task.Delay(waitDuration);

                        if (cancellationToken.IsCancellationRequested)
                        {
                            running = false;

                            return;
                        }
                    }
                }

                logger.Log(
                    GetType(),
                    $"SENDING REQUEST");

                request.SourceFileContext.Status = EFileProcessingStatus.PROMPTING;
                
                using (Stream stream = request.HTTPRequest.GetRequestStream())
                {
                    await stream.WriteAsync(
                        request.Data,
                        0,
                        request.Data.Length,
                        cancellationToken);
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    request.SourceFileContext.Status = EFileProcessingStatus.ABORTED;
                    
                    running = false;

                    return;
                }

                logger.Log(
                    GetType(),
                    "AWAITING RESPONSE");

                string responseText;

                var response = (HttpWebResponse)request.HTTPRequest.GetResponse();

                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    responseText = await streamReader.ReadToEndAsync();
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    request.SourceFileContext.Status = EFileProcessingStatus.ABORTED;
                    
                    running = false;

                    return;
                }

                if (forceTimeout)
                    lastRequestTime = DateTime.Now;

                // Response extraction
                var json = responseText;

                logger.Log(
                    GetType(),
                    $"RESPONSE RECEIVED:\n{json}");

                var responseData = JsonUtility.FromJson<OpenAI.Response>(json);

                if (responseData.Equals(default(OpenAI.Response)))
                {
                    logger.LogError(
                        GetType(),
                        "RESPONSE STRUCT IS EMPTY");

                    if (!request.Retry)
                    {
                        request.SourceFileContext.Status = EFileProcessingStatus.ERROR;
                    
                        return;
                    }

                    if (cancellationToken.IsCancellationRequested)
                    {
                        request.SourceFileContext.Status = EFileProcessingStatus.ABORTED;
                        
                        return;
                    }

                    request.SourceFileContext.Status = EFileProcessingStatus.PROMPTING;
                    
                    continue;
                }

                /*
                if (responseData.choices == null
                    || responseData.choices.Length == 0)
                {
                    throw new Exception(
                        GetType(),
                        "RESPONSE CHOICES ARRAY EMPTY");
                }

                if (responseData.choices[0].message.Equals(default(OpenAI.ResponseMessage)))
                {
                    throw new Exception(
                        GetType(),
                        "RESPONSE MESSAGE STRUCT IS EMPTY");
                }

                if (string.IsNullOrEmpty(responseData.choices[0].message.content))
                {
                    throw new Exception(
                        GetType(),
                        "RESPONSE MESSAGE TEXT IS EMPTY");
                }
                */

                switch (responseData.choices[0].finish_reason)
                {
                    case "length":

                        request.SourceFileContext.Status = EFileProcessingStatus.TOKENS_EXCEEDED;
                        
                        throw new Exception(
                            logger.FormatException(
                                GetType(),
                                "TOKEN LENGTH EXCEEDED"));

                        /*
                        logger.LogError(
                            GetType(),
                            "TOKEN LENGTH EXCEEDED, REQUESTING TO RESEND PROMPT RESPONSE");

                        var promptID = request.ID;

                        if (promptID.Equals(default(Guid)))
                        {
                            throw new Exception(
                                GetType(),
                                "THE RESPONSE HAS EXCEEDED TOKEN LENGTH LIMIT TWICE");
                        }

                        string resendPrompt = $"{RESEND_PROMPT_PREFIX}\"{promptID}\"\n{RESEND_PROMPT_CONDITIONS_POSTFIX}";

                        var resendHttpRequest = OpenAIUtil.BuildRequest(
                            resendPrompt,
                            out byte[] data);

                        var resendRequest = new GPTRequest();

                        resendRequest.ID = default(Guid);

                        resendRequest.HTTPRequest = resendHttpRequest;

                        resendRequest.Data = data;

                        resendRequest.Retry = true;

                        await SendRequest(
                            resendRequest,
                            cancellationToken);

                        if (cancellationToken.IsCancellationRequested)
                        {
                            running = false;

                            return;
                        }

                        request.Response = resendRequest.Response;

                        responseReceivedAndValid = true;
                        */
                        
                        break;

                    case "stop":
                        
                        request.Response = responseData.choices[0].message.content;

                        responseReceivedAndValid = true;

                        request.SourceFileContext.Status = EFileProcessingStatus.AWAITING_WRITE;
                        
                        break;
                    
                    default:
                        
                        request.SourceFileContext.Status = EFileProcessingStatus.EXCEPTION;
                        
                        throw new Exception(
                            logger.FormatException(
                                GetType(),
                                $"UNKNOWN FINISH REASON: {responseData.choices[0].finish_reason}"));

                        break;
                }
            }
        }

        private async Task ProcessWriteCommands(
            CancellationToken cancellationToken)
        {
            while (writeCommands.TryDequeue(out var writeCommand))
            {
                File.WriteAllText(
                    writeCommand.Context.FilePath,
                    writeCommand.Text);
                
                writeCommand.Context.Status = EFileProcessingStatus.DONE;
            }
        }
    }
}