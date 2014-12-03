CommandSharp
============

Console Helper for .NET, Written in C#

You can use these helper classes to run console-based command-line utilities and capture their outputs. Currently there are two kinds of helpers, one uses Reactive-Extensions (Rx) to report the output. Other uses async tasks. I am still in the process of improving the functionality. Feel free to make suggestions.


Here are a few examples

 Simple command with output capturing

```csharp
  var outputTask = CmdHelper.RunCommand("c:\\Python27\\Python.exe","-file c:\\path\\to\\myprogram.py");
  Console.WriteLine("Python program started...");
  //Do some work here
  var output = await outputTask;
  Console.WriteLine("Output received");
  Console.WriteLine(output.Output);
  Console.WriteLine(output.Errors);
```
  
 To log to file, similar to ```C:\> Command.exe >> c:\path\to\logfile.log```

```csharp
  var outputTask = await CmdHelper.RunCommand("c:\\Python27\\Python.exe","-file c:\\path\\to\\myprogram.py", "c:\\path\\to\\workingdir", "c:\\path\\to\\logfile.log");
```
