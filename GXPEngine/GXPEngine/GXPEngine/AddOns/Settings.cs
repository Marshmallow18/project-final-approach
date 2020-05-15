using System.Collections.Generic;
using System;
using System.IO;
using System.Reflection;

namespace GXPEngine
{
    /// <summary>
    /// Static class that contains various settings, such as screen resolution and player controls. 
    /// In your Main method, you can Call the Settings.Load() method to initialize these settings from a text file 
    /// (typically settings.txt, which should be present in the bin/Debug and/or bin/Release folder).
    /// 
    /// The purpose is that you can easily change certain settings this way, without recompiling the code, 
    /// and that during development different people can use different settings while working with the same build.
    /// 
    /// Feel free to add all kinds of other useful properties to this class. They will be initialized from the text file as long
    /// as they are of one of the following types:
    ///   public static int
    ///   public static float
    ///   public static bool
    ///   public static string
    ///   public static string[]
    /// </summary>
    class Settings
    {
        // Settings that are related to this class and the parsing process:
        public static string
            SettingsFileName =
                "settings.txt"; // should be in bin/Debug or bin/Release. Use "MySubFolder/settings.txt" for subfolders.

        public static bool ShowSettingsParsing = true; // If true, settings parsing progress is printed to console
        public static bool ThrowExceptionOnMissingSetting = true;

        // Resolution values - use these when creating a new MyGame instance:
        // (Note: for the arcade machine, use ScreenResolutionX,Y = 1600,1200)
        public static int Width = 1920;
        public static int Height = 1080;
        public static int ScreenResolutionX = 1920;
        public static int ScreenResolutionY = 1080;
        public static bool FullScreen = true;

        // In your Player class, you can use these keys for player controls:
        // (Default: arcade machine setup:)
        // PLAYER 1:
        // WASD:
        public static int P1Up = 87;
        public static int P1Left = 65;
        public static int P1Down = 83;

        public static int P1Right = 68;

        // FGHVBN:
        public static int P1Fire1 = 70;
        public static int P1Fire2 = 71;
        public static int P1Fire3 = 72;
        public static int P1Fire4 = 86;
        public static int P1Fire5 = 66;
        public static int P1Fire6 = 78;

        // PLAYER 2:
        // IJKL:
        public static int P2Up = 73;
        public static int P2Left = 74;
        public static int P2Down = 75;

        public static int P2Right = 76;

        // numpad 1-6:
        public static int P2Fire1 = 306;
        public static int P2Fire2 = 307;
        public static int P2Fire3 = 308;
        public static int P2Fire4 = 303;
        public static int P2Fire5 = 304;
        public static int P2Fire6 = 305;

        // GENERAL BUTTONS:
        // one,two:
        public static int Start1P = 49;

        public static int Start2P = 50;

        // enter:
        public static int Menu = 294;

        //TextBox
        public static int Flashbacks_TextBoxTweenSpeed = 80;

        public static int Default_AlphaTween_Duration = 600;

        public static string Textbox_Font = "Roboto";

        public static string Message_To_Player_After_Collect_Flashbacks = "Message to player...";
        public static float Textbox_Margin_Bottom = 30;

        public static string Flashback_Triggers_Collected = "0";
        public static string Flashback_Pickups_Collected = "0";
        public static string Flashback_Pickups_Incorrect_Order_Message = "You get the incorrect order";

        public static bool Flashback_Triggers_Allow_Skip_With_Esc_Key = true;
        public static string Regex_Audio_Music_Pattern = @"(\[audio_\d+\])|(\[music_\d+\])";
        public static float Flashbacks_Music_Volume = 1f;
        public static int Flashbacks_Music_Fadein_Duration = 800;
        public static float Background_Music_Volume = 0.4f;
        public static float SFX_Default_Volume = 0.7f;

        public static string Base_Music = "data/audios/musics/base.ogg";
        
        public static string HUD_Flashback_Counter_Format = "{0} of {1} flashbacks";

        public static string History_Pickedup_SFX = "data/audios/non-organic_sfx/pickup_2_reverb.wav";
        public static float History_Pickedup_SFX_Volume = 0.2f;
        
        public static string Footstep1_SFX = "data/audios/sfx/step_one.wav";
        public static string Footstep2_SFX = "data/audios/sfx/step_two.wav";
        public static float Footsteps_Volume = 0.4f;
        
        public static string Cave_Background_Ambient_Sound = "data/audios/sfx/ambiant_background.wav";
        public static float Cave_Background_Ambient_Sound_Volume = 0.4f;

        public static string Hidden_Room_Revealed_SFX = "data/audios/non-organic_sfx/final_cave_reveal.ogg";

        public static string Door0_Open_Sound = "data/audios/sfx/261109__jaklocke__door-opening-and-closing-8.wav";
        public static float Door0_Open_Sound_Volume = 0.2f;

        public static bool Cheat_Mode = false;
        
        //Camera Settings
        public static float Camera_Scale = 1;
        public static bool Camera_Scale_Enable_With_E_C_Keys = false;
        
        //Start Screen
        public static string StartScreen_Bg_Image = "data/StartScreenBg.png";
        public static string StartScreen_Music = "data/audio/musics/nosound.wav";

        public static string In_Game_StartScreen_1_Bg_Image = "data/Game_StartScreen_1.png";
        public static string In_Game_StartScreen_1_Music = "data/audios/musics/noaudio.ogg";
        
        public static string In_Game_StartScreen_2_Bg_Image = "data/Game_StartScreen_1.png";
        public static string In_Game_StartScreen_2_Music = "data/audios/musics/noaudio.ogg";

        public static string In_Game_StartScreen_3_Bg_Image = "data/Game_StartScreen_3.png";
        
        /// <summary>
        /// Load new values from the file settings.txt
        /// </summary>
        public static void Load()
        {
            ReadSettingsFromFile();
        }

        private static void Warn(string pWarning, bool alwaysContinue = false)
        {
            string message = "Settings.cs: " + pWarning;
            if (ThrowExceptionOnMissingSetting && !alwaysContinue)
                throw new Exception(message);
            else
                Console.WriteLine("WARNING: " + message);
        }

        private static void ReadSettingsFromFile()
        {
            if (ShowSettingsParsing) Console.WriteLine("Reading settings from file");

            if (!File.Exists(SettingsFileName))
            {
                Warn("No settings file found");
                return;
            }

            StreamReader reader = new StreamReader(SettingsFileName);

            string line = reader.ReadLine();
            while (line != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    line = reader.ReadLine();
                    continue;
                }

                if (line.Length < 2 || line.Substring(0, 2) != "//")
                {
                    if (ShowSettingsParsing) Console.WriteLine("Read a non-comment line: " + line);
                    string[] words = line.Split('=');
                    if (words.Length == 2)
                    {
                        // Remove all white space characters at start and end (but not in between non-white space characters):
                        words[0] = words[0].Trim();
                        words[1] = words[1].Trim();

                        Object value;
                        bool boolValue;
                        float floatValue;
                        int intValue;
                        // InvariantCulture is necessary to override (e.g. Dutch) locale settings when using .NET: the decimal separator is a dot, not a comma.
                        if (int.TryParse(words[1], System.Globalization.NumberStyles.Any,
                            System.Globalization.CultureInfo.InvariantCulture, out intValue))
                        {
                            value = intValue;
                            if (ShowSettingsParsing)
                                Console.WriteLine(" integer argument: Key {0} Value {1}", words[0], value);
                        }
                        else if (float.TryParse(words[1], System.Globalization.NumberStyles.Any,
                            System.Globalization.CultureInfo.InvariantCulture,
                            out floatValue))
                        {
                            value = floatValue;
                            if (ShowSettingsParsing)
                                Console.WriteLine(" float argument: Key {0} Value {1}", words[0], value);
                        }
                        else if (bool.TryParse(words[1], out boolValue))
                        {
                            value = boolValue;
                            if (ShowSettingsParsing)
                                Console.WriteLine(" boolean argument: Key {0} Value {1}", words[0], value);
                        }
                        else
                        {
                            value = words[1];
                            if (ShowSettingsParsing)
                                Console.WriteLine(" string argument: Key {0} Value {1}", words[0], words[1]);
                        }

                        FieldInfo field =
                            typeof(Settings).GetField(words[0], BindingFlags.Static | BindingFlags.Public);
                        if (field != null)
                        {
                            try
                            {
                                // unpacking happens here:
                                // (If you want to load more than the supported types, such as Sounds, add your own
                                //  logic here, similar to the code below for the string[] type:)
                                if (field.FieldType == typeof(string[]))
                                {
                                    string[] valuewords = words[1].Split(',');
                                    for (int i = 0; i < valuewords.Length; i++)
                                    {
                                        valuewords[i] = valuewords[i].Trim();
                                    }

                                    field.SetValue(null, valuewords);
                                }
                                else
                                {
                                    field.SetValue(null, value);
                                }
                            }
                            catch (Exception error)
                            {
                                Warn("Cannot set field " + words[0] + ": type mismatch? " + error.Message);
                            }
                        }
                        else
                        {
                            Warn("No field with name " + words[0] + " exists!");
                        }
                    }
                    else
                    {
                        Warn("Malformed line (expected one '=' character): " + line);
                    }
                }
                else
                {
                    //if (ShowSettingsParsing) Console.WriteLine("Comment line or empty line: " + line);
                }

                line = reader.ReadLine();
            }

            reader.Close();
        }
    }
}