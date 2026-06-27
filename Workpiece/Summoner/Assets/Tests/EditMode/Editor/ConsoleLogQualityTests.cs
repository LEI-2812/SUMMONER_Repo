using System.IO;
using NUnit.Framework;

public class ConsoleLogQualityTests
{
    [Test]
    public void RuntimeScripts_DoNotContainPlaceholderLogMessages()
    {
        string[] scriptFiles = Directory.GetFiles("Assets/Script", "*.cs", SearchOption.AllDirectories);

        foreach (string scriptFile in scriptFiles)
        {
            string source = File.ReadAllText(scriptFile);
            StringAssert.DoesNotContain("Debug.Log(\"Log\")", source, scriptFile);
            StringAssert.DoesNotContain("Debug.LogError(\"Log\")", source, scriptFile);
        }
    }
}
