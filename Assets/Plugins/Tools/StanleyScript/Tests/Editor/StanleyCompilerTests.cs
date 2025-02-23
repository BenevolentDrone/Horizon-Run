using System.Threading.Tasks;

using NUnit.Framework;

using UnityEngine;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.StanleyScript.Tests
{
    [TestFixture]
    public class StanleyCompilerTests
    {
        [Test]
        public void TestCompilerSimple()
        {
            // Arrange
            var compiler = new StanleyCompiler(
                new StanleyASTWalker());
            
            /*
            string[] script = new[]
            {
                "There was a number Counter.",
                "There was a number Max.",
                "There was an event Stop.",
                "",
                "Max was 5.",
                "Counter was 0.",
                "",
                "To increment Counter:",
                "    Counter became Counter + 1.",
                "    When Counter had been greater than Max then Stop.",
                "End.",
                "",
                "Counter was incremented.",
                "Alert was called.",
                "Message was displayed to Screen."
            };
            */
            
            string[] script = new[]
            {
                "There was a number Counter.",
                "",
                "Counter was 0.",
            };

            // Act
            compiler.LoadProgram(script);
            compiler.Compile(out string[] instructions);
            
            // Print results
            Debug.Log("Input script:\n\n" + string.Join("\n", script));
            
            Debug.Log("\nGenerated opcodes:\n\n" + string.Join("\n", 
                instructions/*.Where(i => !string.IsNullOrWhiteSpace(i))*/));
            
            Debug.Log("\n");
        }

        [Test]
        public void TestCompilerMethodsEventsLoops()
        {
            // Arrange
            var compiler = new StanleyCompiler(
                new StanleyASTWalker());

            string[] script = @"
There was an object Button.
There was an object Display.
There was a list Items.
There was a number Count.

There was an action Handler.
There was an action Button's OnClick.
There was an event OnComplete.
There was an event Display's OnUpdate.

There was text Message.

This is how Handler was done:
    Message became 'Handler finished'.
End.

This is how Button's OnClick was handled:
    Message became 'Action completed'.
End.

Once OnComplete happened:
    Message became 'Operation completed'.
End.

Once Display's OnUpdate happened:
    Message became 'Display updated'.
End.

This was repeated 5 times:
    Handler was executed.
End.

OnComplete has happened.
Display's OnUpdate has happened.

Time passed for 100 milliseconds.

It was expected that Message was 'Display updated'.".Split('\n');

            // Act
            compiler.LoadProgram(script);
            compiler.Compile(out string[] instructions);

            // Assert
            Debug.Log("\nGenerated opcodes:\n\n" + string.Join("\n", instructions));
            Debug.Log("\n");
        }
        
        [Test]
        public async Task TestVMMethodsEventsLoopsAsync()
        {
            // Arrange
            var compiler = new StanleyCompiler(
                new StanleyASTWalker());

            string[] script = @"
There was an object Button.
There was an object Display.
There was a list Items.
There was a number Count.

There was an action Handler.
There was an action Button's OnClick.
There was an event OnComplete.
There was an event Display's OnUpdate.

There was text Message.

This is how Handler was done:
    Message became 'Handler finished'.
End.

This is how Button's OnClick was handled:
    Message became 'Action completed'.
End.

Once OnComplete happened:
    Message became 'Operation completed'.
End.

Once Display's OnUpdate happened:
    Message became 'Display updated'.
End.

This was repeated 5 times:
    Handler was executed.
End.

OnComplete has happened.
Display's OnUpdate has happened.

Time passed for 100 milliseconds.

It was expected that Message was 'Display updated'.".Split('\n');

            // Act
            compiler.LoadProgram(script);
            compiler.Compile(out string[] instructions);
            
            StanleyContext context = StanleyFactory.BuildStanleyContext(
                null,
                true,
                false,
                null);

            NUnit.Framework.Assert.That(context.SaveProgramToLibrary(
                "test",
                script));

            NUnit.Framework.Assert.That(context.LoadProgramFromLibrary("test"));

            await context.StartAsync(
                new AsyncExecutionContext
                {
                });

            // Assert
            NUnit.Framework.Assert.That(context.ExecutionStatus, Is.EqualTo(EExecutionStatus.FINISHED));
        }

        [Test]
        public async Task TestVMListsLoops()
        {
            // Arrange
            var compiler = new StanleyCompiler(
                new StanleyASTWalker());

            string[] script = @"
There was a number Count.
There was a list List.
There was anm object called Object.
There was a list called Object's List.

There was an action Increment.
There was an action Squared.

Count was 0.

Value 1 was added to List.
Value 2 was added to List.
Value 3 was added to List.
Value 4 was added to List.
Value 5 was added to List.

This is how Increment was done:
    Count became Count + 1.
End.

This is how Squared was done:
    Count became Count * Count.
End.

This was repeated 5 times:
    Increment was executed.
End.

It was expected that Count was 5.

As long as Count was less than 30:
    Squared was executed.
    Value Count was inserted at position 0 in List.
End.

It was expected that Count was 625.

It was expected that count from List was 7.

It was expected that the first value from List was 625.

It was expected that the value at position 1 from List was 25.

For each Number in List:
    Value Number was inserted at position 0 in Object's List.
End.

It was expected that count from Object's List was 7.

Value at position 1 was removed from Object's List.

It was expected that the value at position 1 from Object's List was 3.

The List became empty.

It was expected that count from List was 0.".Split('\n');

            // Act
            compiler.LoadProgram(script);
            compiler.Compile(out string[] instructions);
            
            Debug.Log("Generated opcodes:\n" + string.Join("\n", 
                instructions));
            
            StanleyContext context = StanleyFactory.BuildStanleyContext(
                null,
                true,
                false,
                null);

            NUnit.Framework.Assert.That(context.SaveProgramToLibrary(
                "test",
                script));

            NUnit.Framework.Assert.That(context.LoadProgramFromLibrary("test"));

            await context.StartAsync(
                new AsyncExecutionContext
                {
                });

            // Assert
            NUnit.Framework.Assert.That(context.ExecutionStatus, Is.EqualTo(EExecutionStatus.FINISHED));
        }
    
        [Test]
        public async Task TestVMBranching()
        {
            // Arrange
            var compiler = new StanleyCompiler(
                new StanleyASTWalker());

            string[] script = @"
There was a number Count.

There was an action IfThenElse.

Count was 0.

This is how IfThenElse was done:
    If Count was 0:
        Count became 1.
    Otherwise if Count was 1:
        Count became 2.
    Otherwise:
        Count became 3.
    End.
End.

This was repeated 3 times:
    IfThenElse was executed.
End.

It was expected that Count was 3.
".Split('\n');

            // Act
            compiler.LoadProgram(script);
            compiler.Compile(out string[] instructions);
            
            Debug.Log("Generated opcodes:\n" + string.Join("\n", 
                instructions));
            
            StanleyContext context = StanleyFactory.BuildStanleyContext(
                null,
                true,
                false,
                null);

            NUnit.Framework.Assert.That(context.SaveProgramToLibrary(
                "test",
                script));

            NUnit.Framework.Assert.That(context.LoadProgramFromLibrary("test"));

            await context.StartAsync(
                new AsyncExecutionContext
                {
                });

            // Assert
            NUnit.Framework.Assert.That(context.ExecutionStatus, Is.EqualTo(EExecutionStatus.FINISHED));
        }
        
        [Test]
        public async Task TestVMSubprograms()
        {
            // Arrange
            var compiler = new StanleyCompiler(
                new StanleyASTWalker());
            
            string[] script1 = @"
There was a program P.

There was an action StartP.

This is how StartP was done:
    P has started.
End.

Program 'script2' was loaded into P.

StartP was executed.

There was a list List.

Value 1 was added to List.
Value 2 was added to List.
Value 3 was added to List.
Value 4 was added to List.
Value 5 was added to List.

It was expected that count from List was 5.
".Split('\n');
            
            string[] script2 = @"
There was a list List.

Value 1 was added to List.
Value 2 was added to List.
Value 3 was added to List.
Value 4 was added to List.
Value 5 was added to List.

It was expected that count from List was 5.
".Split('\n');

            // Act
            compiler.LoadProgram(script1);
            compiler.Compile(out string[] instructions);
            
            Debug.Log("Generated opcodes:\n" + string.Join("\n", 
                instructions));
            
            StanleyContext context = StanleyFactory.BuildStanleyContext(
                null,
                true,
                false,
                null);

            NUnit.Framework.Assert.That(context.SaveProgramToLibrary(
                "script1",
                script1));
            
            NUnit.Framework.Assert.That(context.SaveProgramToLibrary(
                "script2",
                script2));

            NUnit.Framework.Assert.That(context.LoadProgramFromLibrary("script1"));

            await context.StartAsync(
                new AsyncExecutionContext
                {
                });

            // Assert
            NUnit.Framework.Assert.That(context.ExecutionStatus, Is.EqualTo(EExecutionStatus.FINISHED));
        }
        
        [Test]
        public async Task TestVMTypecast()
        {
            // Arrange
            var compiler = new StanleyCompiler(
                new StanleyASTWalker());

            string[] script = @"
There was a number INT.
There was a real number REAL.

There was a number A.
There was a real number B.
There was a real number C.

INT was 5.
REAL was 3.14.

A became REAL.

B became A.

C became INT.

It was expected that A was 3.

It was expected that B was 3.0.

It was expected that C was 5.
".Split('\n');

            // Act
            compiler.LoadProgram(script);
            compiler.Compile(out string[] instructions);
            
            Debug.Log("Generated opcodes:\n" + string.Join("\n", 
                instructions));
            
            StanleyContext context = StanleyFactory.BuildStanleyContext(
                null,
                true,
                false,
                null);

            NUnit.Framework.Assert.That(context.SaveProgramToLibrary(
                "test",
                script));

            NUnit.Framework.Assert.That(context.LoadProgramFromLibrary("test"));

            await context.StartAsync(
                new AsyncExecutionContext
                {
                });

            // Assert
            NUnit.Framework.Assert.That(context.ExecutionStatus, Is.EqualTo(EExecutionStatus.FINISHED));
        }
    }
}
