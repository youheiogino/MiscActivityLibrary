using System;
using System.Activities;
using System.Activities.Expressions;
using System.Activities.Statements;
using System.Activities.XamlIntegration;
using System.Linq;
using Microsoft.CSharp.Activities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiscActivityLibrary;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Variable<int> n = new Variable<int>
            {
                Name = "n"
            };

            Activity wf = new Sequence()
            {
                Variables = { n },
                Activities =
                {
                    new Assign<int>
                    {
                        To = new CSharpReference<int>("n"),
                        Value = new CSharpValue<int>("new Random().Next(1, 101)")
                    },
                    new ShowMessageBox
                    {
                        Message = new CSharpValue<string>("\"The number is \" + n")
                    }
                }
            };

            CompileExpressions(wf);

            WorkflowInvoker.Invoke(wf);
        }

        [TestMethod]
        public void TestMethod2()
        {
            Activity wf = new Activity1();
            WorkflowInvoker.Invoke(wf);
        }

        // https://docs.microsoft.com/ja-jp/dotnet/framework/windows-workflow-foundation/csharp-expressions
        static void CompileExpressions(Activity activity)
        {
            // activityName is the Namespace.Type of the activity that contains the
            // C# expressions.
            string activityName = activity.GetType().ToString();

            // Split activityName into Namespace and Type.Append _CompiledExpressionRoot to the type name
            // to represent the new type that represents the compiled expressions.
            // Take everything after the last . for the type name.
            string activityType = activityName.Split('.').Last() + "_CompiledExpressionRoot";
            // Take everything before the last . for the namespace.
            string activityNamespace = string.Join(".", activityName.Split('.').Reverse().Skip(1).Reverse());

            // Create a TextExpressionCompilerSettings.
            TextExpressionCompilerSettings settings = new TextExpressionCompilerSettings
            {
                Activity = activity,
                Language = "C#",
                ActivityName = activityType,
                ActivityNamespace = activityNamespace,
                RootNamespace = null,
                GenerateAsPartialClass = false,
                AlwaysGenerateSource = true,
                ForImplementation = false
            };

            // Compile the C# expression.
            TextExpressionCompilerResults results =
                new TextExpressionCompiler(settings).Compile();

            // Any compilation errors are contained in the CompilerMessages.
            if (results.HasErrors)
            {
                throw new Exception("Compilation failed.");
            }

            // Create an instance of the new compiled expression type.
            ICompiledExpressionRoot compiledExpressionRoot =
                Activator.CreateInstance(results.ResultType,
                    new object[] { activity }) as ICompiledExpressionRoot;

            // Attach it to the activity.
            CompiledExpressionInvoker.SetCompiledExpressionRoot(
                activity, compiledExpressionRoot);
        }
    }
}
