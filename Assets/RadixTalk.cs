using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;

public class RadixTalk : MonoBehaviour {

   public KMBombInfo Bomb;
   public KMAudio Audio;

   public OutlinedText outlinedText;
   public AudioClip[] sounds;

   private int number, base1, radix, base2;
   private string alteredNumber, alteredRadix;
   private string answer;
   private string input = "";

   private char[] digits = {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};
   private KeyCode[] TypableKeys = {KeyCode.Alpha0, KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.Keypad0, KeyCode.Keypad1, KeyCode.Keypad2, KeyCode.Keypad3, KeyCode.Keypad4, KeyCode.Keypad5, KeyCode.Keypad6, KeyCode.Keypad7, KeyCode.Keypad8, KeyCode.Keypad9, KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E, KeyCode.F};
   private KeyCode[] ExtraKeys = {KeyCode.Backspace, KeyCode.Return};
   private char[] subscripts = {'₀', '₁', '₂', '₃', '₄', '₅', '₆', '₇', '₈', '₉'};
   private int[] allowedBinaryNumbers = {79, 87, 91, 93, 94, 95, 103, 107, 109, 110, 111, 115, 117, 118, 119, 121, 122, 123, 124, 125, 126};

   private bool focused, activated, animationFinished;

   static int ModuleIdCounter = 1;
   int ModuleId;

   void Awake() {
      ModuleId = ModuleIdCounter++;
      GetComponent<KMBombModule>().OnActivate += Activate;
      GetComponent<KMSelectable>().OnFocus += delegate () { focused = true; };
      GetComponent<KMSelectable>().OnDefocus += delegate () { focused = false; };
   }

   void Activate() {
      activated = true;
   }

   void Start() {
      outlinedText = GetComponentInChildren<OutlinedText>(true);
      for (int i = 0; i < 5; i++) {
         outlinedText.SetText(i, "");
      }
      Generate();
   }

   void Generate() {
      base1 = Rnd.Range(2, 17);
      base2 = Rnd.Range(2, 17);
      number = Rnd.Range(0, 128);
      if (base1 == 2 && number > 63 && !allowedBinaryNumbers.Contains(number))
         number = Rnd.Range(0, 64);
      do radix = Rnd.Range(2, 17); while (radix == base1);
      Debug.LogFormat("[Radix Talk #{0}] In decimal, the module is asking to convert {1} into base {2}.", ModuleId, number, radix);
      alteredNumber = ConvertToBase(number, base1, true);
      alteredRadix = ConvertToBase(radix, base2, true);
      answer = ConvertToBase(number, radix, false);
      Debug.LogFormat("[Radix Talk #{0}] However, the module is displaying this as converting {1} into base {2}.", ModuleId, alteredNumber, alteredRadix);
      Debug.LogFormat("[Radix Talk #{0}] The answer to submit is {1}.", ModuleId, answer);
      StartCoroutine(InitialAnimation());
   }

   void Update() {
      if (focused && activated && animationFinished) {
         for (int i = 0; i < TypableKeys.Length; i++) {
            if (Input.GetKeyDown(TypableKeys[i]) && input.Length < 7) {
               if (i < 20)
                  input += (i % 10);
               else
                  input += digits[i - 10];
               Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.TypewriterKey, transform);
               outlinedText.SetText(4, input);
            }
         }
         if (Input.GetKeyDown(ExtraKeys[0])) {
            if (input.Length != 0) {
               input = input.Substring(0, input.Length - 1);
               Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.TypewriterKey, transform);
               outlinedText.SetText(4, input);
            }
         }
         if (Input.GetKeyDown(ExtraKeys[1])) {
            animationFinished = false;
            StartCoroutine(CheckInput());
         }
      }
   }

   string ConvertToBase(int number, int modulus, bool appendBase) {
      string result = "";
      while ((number / modulus) > 0) {
         result = digits[(number % modulus)] + result;
         number /= modulus;
      }
      result = digits[(number % modulus)] + result;
      if (!appendBase)
         return result;
      if (modulus < 10) {
         result += subscripts[modulus];
      } else {
         result += subscripts[modulus / 10];
         result += subscripts[modulus % 10];
      }
      return result;
   }

   void Solve() {
      GetComponent<KMBombModule>().HandlePass();
   }

   void Strike() {
      GetComponent<KMBombModule>().HandleStrike();
   }

   IEnumerator InitialAnimation() {
      for (int i = 0; i < alteredNumber.Length; i++) {
         Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.TypewriterKey, transform);
         outlinedText.SetText(0, alteredNumber.Substring(0, i + 1));
         yield return new WaitForSeconds(0.25f / alteredNumber.Length);
      }
      for (int i = 0; i < 17; i++) {
         Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.TypewriterKey, transform);
         outlinedText.SetText(1, "converted to base".Substring(0, i + 1));
         yield return new WaitForSeconds(0.25f / 17);
      }
      for (int i = 0; i < alteredRadix.Length; i++) {
         Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.TypewriterKey, transform);
         outlinedText.SetText(2, alteredRadix.Substring(0, i + 1));
         yield return new WaitForSeconds(0.25f / alteredRadix.Length);
      }
      for (int i = 0; i < 2; i++) {
         Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.TypewriterKey, transform);
         outlinedText.SetText(3, "is".Substring(0, i + 1));
         yield return new WaitForSeconds(0.25f / 2);
      }
      animationFinished = true;
   }

   IEnumerator CheckInput() {
      if (input == answer) {
         outlinedText.SetText(0, number.ToString());
         outlinedText.SetText(2, radix.ToString());
         Audio.PlaySoundAtTransform("Correct", transform);
         Debug.LogFormat("[Radix Talk #{0}] Inputted the correct answer. Module solved!", ModuleId);
         Solve();
         yield return null;
      } else {
         for (int i = 0; i < 5; i++)
            outlinedText.SetColors(i, new Color(1f, 0f, 0f, 1f), new Color(.6f, 0f, 0f, 1f));
         outlinedText.SetText(3, "isn't");
         Audio.PlaySoundAtTransform("Incorrect", transform);
         if (input == "") {
            outlinedText.SetText(4, "nothing..");
            Debug.LogFormat("[Radix Talk #{0}] Submitted nothing, which... is incorrect. Striking and resetting the module...", ModuleId);
         } else {
            Debug.LogFormat("[Radix Talk #{0}] Submitted {1}, which is incorrect. Striking and resetting the module...", ModuleId, input);
         }
         yield return new WaitForSeconds(1f);
         Strike();
         yield return new WaitForSeconds(1f);
         for (int i = 0; i < 5; i++)
            outlinedText.SetColors(i, new Color(0f, 1f, 0f, 1f), new Color(0f, .6f, 0f, 1f));
         input = "";
         for (int i = 0; i < 5; i++)
            outlinedText.SetText(i, "");
         Generate();
      }
   }

#pragma warning disable 414
   private readonly string TwitchHelpMessage = @"Use !{0} submit (number) to submit an answer.";
#pragma warning restore 414

   IEnumerator ProcessTwitchCommand(string Command) {
      Command = Command.Trim().ToUpperInvariant();
      yield return null;
      if (!activated || !animationFinished) {
         yield return "sendtochaterror The module is in the middle of an animation.";
         yield break;
      }
      if (!Regex.IsMatch(Command, @"^\s*SUBMIT\s+[0123456789ABCDEF]{1,7}\s*$")) {
         yield return "Invalid command/submission.";
         yield break;
      }
      string[] twitchInput = Command.Split(' ');
      for (int i = 0; i < twitchInput[1].Length; i++) {
         input += twitchInput[1][i];
         Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.TypewriterKey, transform);
         outlinedText.SetText(4, input);
         yield return new WaitForSeconds(0.1f);
      }
      animationFinished = false;
      StartCoroutine(CheckInput());
      if (input != answer) {
         yield return "strike";
      }
   }

   IEnumerator TwitchHandleForcedSolve() {
      while (input.Length != 0) {
         input = input.Substring(0, input.Length - 1);
         Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.TypewriterKey, transform);
         outlinedText.SetText(4, input);
         yield return new WaitForSeconds(0.1f);
      }
      for (int i = 0; i < answer.Length; i++) {
         input += answer[i];
         Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.TypewriterKey, transform);
         outlinedText.SetText(4, input);
         yield return new WaitForSeconds(0.1f);
      }
      animationFinished = false;
      StartCoroutine(CheckInput());
   }
}
