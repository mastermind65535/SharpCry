# SharpCry
> C# Intelligent Ransomware PoC

## Screenshots
<img src="screenshot.png">

## Videos

# Disclaimer
> This software is intended ***exclusively for educational purposes and ethical cybersecurity research***. It is designed to help users understand potential vulnerabilities in systems so they can improve their security.
>
> By using this software, you agree to use it in compliance with all applicable laws and regulations. Unauthorized use, distribution, or deployment of this software against any system without the explicit permission of the system owner is strictly prohibited and may result in criminal and civil penalties.
>
> The creator(s) of this software assume no liability and are not responsible for any misuse or damages arising from the use of this software. Always obtain proper authorization before testing or analyzing systems using this software.

# Warning
> Be sure to read the license before using the project.

# Purposes
> This project is a **Proof of Concept (PoC) malware designed to evaluate <ins>the ability of ransomware detection of anti-virus software</ins>** and assess their effectiveness in responding to and minimizing the impact of already executed malicious code.
> **<del>(No malicious actions are included in the source code.)</del>**

# Usage
> **It is a cmd Batch (.bat) file. You can use to execute certain executable files instead using the following command:**<br>
> start /min "YOUR_EXECUTABLE_FILE_PATH"<br>
> ## Manual Compilation
> 1. Load the **Fake-MS-Updater.sln** in [Visual Studio 2022](https://visualstudio.microsoft.com/vs/).<br>
> 2. From the menu located above, go to Debug drop bow next to the green build button and select **Release**.<br>
> 3. Click the green button says **MS Emergency Update**.<br>
> 4. Go to project directory and find **bin/Release/net8.0-windows/**.<br>
> 5. Use both .exe & .dll file to run the result.<br>
> ## Prebuilt Executable
> **!WARNING! Using the prebuilt executable will not run any code. (It prints "complete" on a minimized cmd window frame and pauses for exit)**<br>
> 1. Go to project directory and find **bin\Release\net8.0-windows\**.<br>
> 2. Use both .exe & .dll file to run the result.<br>
