﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos.Build.MSBuild;
using Cosmos.Debug.Common;
using Microsoft.Build.Framework;

namespace Cosmos.Compiler.TestsBase
{
    public class CompilerRunner
    {
        public CompilerRunner()
        {
            References = new List<string>();
        }
        public List<string> References
        {
            get;
            private set;
        }

        public string OutputFile
        {
            get;
            set;
        }

        public void Execute()
        {
            if (String.IsNullOrWhiteSpace(OutputFile))
            {
                throw new InvalidOperationException("No OutputFile specified!");
            }
            if (References.Count == 0)
            {
                throw new InvalidOperationException("No References specified!");
            }
            DebugInfo.mLastGuid = 0;
            var xTask = new IL2CPUTask();
            xTask.DebugEnabled = true;
            xTask.StackCorruptionDetectionEnabled = true;
            xTask.DebugMode = "Source";
            xTask.TraceAssemblies = "User";
            xTask.DebugCom = 1;
            xTask.UseNAsm = true;
            xTask.OutputFilename = OutputFile;
            xTask.EnableLogging = true;
            xTask.EmitDebugSymbols = true;
            xTask.IgnoreDebugStubAttribute = false;
            xTask.References = GetReferences();
            xTask.OnLogError = m =>
                               {
                                   throw new Exception("Error during compilation: " + m);
                               };
            xTask.OnLogWarning = (m) => Console.WriteLine("Warning: {0}", m);
            xTask.OnLogMessage = (m) =>
                                 {
                                     Console.WriteLine("Message: {0}", m);
                                 };
            xTask.OnLogException = (m) => Console.WriteLine("Exception: {0}", m.ToString());

            if (!xTask.Execute())
            {
                throw new Exception("Error occurred while running compiler!");
            }
        }

        private ITaskItem[] GetReferences()
        {
            var xResult = new List<ITaskItem>(References.Count);
            foreach (var xRefFile in References)
            {
                xResult.Add(new TaskItemImpl(xRefFile));
            }
            return xResult.ToArray();
        }

        private class TaskItemImpl : ITaskItem
        {
            private string path;

            public TaskItemImpl(string path)
            {
                this.path = path;
            }

            public System.Collections.IDictionary CloneCustomMetadata()
            {
                throw new NotImplementedException();
            }

            public void CopyMetadataTo(ITaskItem destinationItem)
            {
                throw new NotImplementedException();
            }

            public string GetMetadata(string metadataName)
            {
                if (metadataName == "FullPath")
                {
                    return path;
                }
                throw new NotImplementedException();
            }

            public string ItemSpec
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public int MetadataCount
            {
                get
                {
                    return MetadataNames.Count;
                }
            }

            public System.Collections.ICollection MetadataNames
            {
                get
                {
                    return new String[] { "FullPath" };
                }
            }

            public void RemoveMetadata(string metadataName)
            {
                throw new NotImplementedException();
            }

            public void SetMetadata(string metadataName, string metadataValue)
            {
                throw new NotImplementedException();
            }
        }

    }
}
