using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;
using KModkit;
using Newtonsoft.Json;

public class FlowchartMadness : MonoBehaviour {

    public KMAudio Audio;
    public KMBombModule Module;
    public KMBombInfo Bomb;
    public KMSelectable[] Buttons;
    public TextMesh screenText;

    static int moduleIdCounter = 1;
    int moduleId = 0;
    System.Random rnd = new System.Random();

    //Initializer variables
    int fmRandomSymbolArray = 0;
    int fmRandomSymbolArrayID = 0;
    string[] displaySymbols = new string[4];
    int[] displaySymbolsNumeric = new int[4];
    string[] displaySymbolsNames = new string[4];
    string[] digitArray = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
    string[] latinArray = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
    string[] greekArray = { "!", "@", "#", "$", "%", "^", "&", "*", "(", "~", ")", "+", "-", "[", "]", "{", "~", "}", "?", "<", ">", "_", "=", ",", ".", "/" };
    string[] smileyArray = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };

    //Step 1 variables
    int startingCellID = 0;
    int equivalentSymbolCounter = 0;
    int greatestEquivalentSymbolCounter = 0;

    //Step 2 variables
    int numOfDigits = 0;
    int numOfLatin = 0;
    int numOfGreek = 0;
    int numOfSmiley = 0;
    int totalMoves = 0;
    int symbolNumeric = 0;
    string tempSymbol = "";

    //Step 3 variables
    bool circle, square, triangle, skull;
    string[] smileyEyesNorm = { "a", "c", "k", "n", "p" };
    string[] smileyEyesAngry = { "e", "h", "r", "w", "x" };
    string[] smileyEyesSad = { "f", "i", "l", "u", "z" };
    string[] smileyEyesQuestion = { "b", "d", "g", "j", "m", "o", "q", "s", "v", "y" };
    string[] smileyMouthSmile = { "a", "g", "h", "l", "o" };
    string[] smileyMouthLaugh = { "k", "q", "r", "y", "z" };
    string[] smileyMouthFrown = { "d", "j", "n", "u", "x" };
    string[] smileyMouthSad = { "b", "c", "e", "i", "s" };
    int happinessScore = 0;
    bool happySymbolDisplayed;
    int[] symbolNumericSums = new int[6];
    int evalSquareCounter = 0;
    bool squareSumExists;
    bool displayedLatinExists;
    int finalCellId;

    //Step 4 variables
    int coinToss = 0;
    int inputSequenceLength = 0;
    int solvingSequenceLength = 0;
    string inputSequenceDebug = "";
    string solvingSequenceDebug = "";
    int[] numOfButtonPresses = new int[8];
    bool inputRuleTwoActive = false;
    int buttonNumberPressed = 0;
    bool numOfButtonsPressedGreaterThanOne;
    bool inputRuleTwoFour = false;
    int randomNumberOfPresses = 0;
    int[] inputRuleSevenCode = new int[4];
    bool inputRuleSeventeenActive;
    int inputRuleSeventeenCorrectButton = 0;
    bool inputRuleEighteenActive;
    int inputRuleEighteenCorrectButton;
    int inputRuleEighteenPositionIndex = 0;
    int smileyGlyphCount = 0;
    int greekLetterCount = 0;
    bool inputRuleThirtyThreeActive;

    bool moduleSolved = false, lightsOn = false;

	// Use this for initialization
	void Start () {
        if (!moduleSolved) {
            Debug.Log("Start");
            //Reseting or Setting up module
            startingCellID = 0;
            equivalentSymbolCounter = 0;
            greatestEquivalentSymbolCounter = 0;
            numOfDigits = 0;
            numOfLatin = 0;
            numOfGreek = 0;
            numOfSmiley = 0;
            totalMoves = 0;
            symbolNumeric = 0;
            tempSymbol = "";
            for (int i = 0; i < 6; i++)
            {
                symbolNumericSums[i] = 0;
            }
            evalSquareCounter = 0;
            finalCellId = 0;
            inputSequenceLength = 0;
            solvingSequenceLength = 0;
            inputSequenceDebug = "";
            solvingSequenceDebug = "";
            for (int i = 0; i < 8; i++)
            {
                numOfButtonPresses[i] = 0;
            }
            inputRuleTwoActive = false;
            numOfButtonsPressedGreaterThanOne = false;
            inputRuleTwoFour = false;
            for (int i = 0; i < 4; i++)
            {
                inputRuleSevenCode[i] = 0;
            }
            inputRuleSeventeenActive = false;
            inputRuleSeventeenCorrectButton = 0;
            inputRuleEighteenActive = false;
            smileyGlyphCount = 0;
            greekLetterCount = 0;
            inputRuleThirtyThreeActive = false;

            //Step 1: Determining Starting Cell ID
            if (checkSymbolEquivalence() == 3)
            {
                startingCellID = Bomb.GetBatteryCount();
            }
            else if (checkSymbolEquivalence() == 2)
            {
                startingCellID = Bomb.GetPortCount();
            }
            else
            {
                startingCellID = (displaySymbolsNumeric[0] + displaySymbolsNumeric[1] + displaySymbolsNumeric[2] + displaySymbolsNumeric[3]) % 10;
            }
            Debug.LogFormat("[Flowchart Madness #{0}] The Starting Cell ID is {1}.", moduleId, startingCellID);

            //Step 2: Calculating the Number of Moves
            totalMoves = 0;
            foreach (char thing in Bomb.GetSerialNumber())
            {
                tempSymbol = thing.ToString();
                Debug.Log("Adding " + getSymbolNumeric(tempSymbol) + " to total moves.");
                totalMoves += getSymbolNumeric(tempSymbol);
            }
            totalMoves = totalMoves % 44;
            Debug.LogFormat("[Flowchart Madness #{0}] The number of moves to make in the flowchart is {1}.", moduleId, totalMoves);

            //Step 3: Performing the moves on the flowchart
            circle = evalCircle();
            Debug.Log("Circles are " + circle);
            square = evalSquare();
            Debug.Log("Squares are " + square);
            triangle = evalTriangle();
            Debug.Log("Triangles are " + triangle);
            finalCellId = performMove(startingCellID);
            Debug.Log("You've moved to " + finalCellId);
            for (int i = 0; i < totalMoves - 1; i++)
            {
                finalCellId = performMove(finalCellId);
                Debug.Log("You've moved to " + finalCellId);
            }
            Debug.LogFormat("[Flowchart Madness #{0}] Your Final Cell ID and Input Rule is {1}.", moduleId, finalCellId);

            //Step 4: Finding/Performing the Input Rule
            calculateInputSequence(finalCellId);
            Debug.Log("The input is " + inputSequenceDebug);
        }
    }

    //Lights come on in the room (timer starts)
    void Awake () {
        Debug.Log("Awake");
        //Index module ID
        moduleId = moduleIdCounter++;

        foreach (KMSelectable button in Buttons)
        {
            KMSelectable pressedButton = button;
            button.OnInteract += delegate () { ButtonPress(pressedButton); return false; };
        }

        //Set up initially displayed symbols
        mainUpdateDisplay();
    }

    // Update is called once per frame
    void Update () {
        if (!moduleSolved) {
            // Evaluate skulls all the time
            if (Bomb.GetStrikes() > 0)
            {
                skull = true;
            }
            else
            {
                skull = false;
            }

            //Check General Input Rule
            if (solvingSequenceLength == inputSequenceLength && !inputRuleEighteenActive && !inputRuleThirtyThreeActive && !inputRuleSeventeenActive)
            {
                if (solvingSequenceDebug.Equals(inputSequenceDebug) && !moduleSolved)
                {
                    GetComponent<KMBombModule>().HandlePass();
                    Audio.PlaySoundAtTransform("FlowchartMadnessVictory", transform);
                    moduleSolved = true;
                }
                else
                {
                    GetComponent<KMBombModule>().HandleStrike();
                    Start();
                }
            }
        }
	}

    void ButtonPress(KMSelectable button) {
        button.AddInteractionPunch();
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.TypewriterKey, transform);
        Debug.Log("You've pressed button " + button.GetComponentInChildren<TextMesh>().text);
        if (button.GetComponentInChildren<TextMesh>().text.Equals("1")) {
            buttonNumberPressed = 1;
            solvingSequenceDebug += buttonNumberPressed.ToString();
            solvingSequenceLength++;
            numOfButtonPresses[buttonNumberPressed - 1] += 1;
            Debug.LogFormat("[Flowchart Madness #{0}] You pressed number {1}", moduleId, buttonNumberPressed);
        }
        else if (button.GetComponentInChildren<TextMesh>().text.Equals("2")) {
            buttonNumberPressed = 2;
            solvingSequenceDebug += buttonNumberPressed.ToString();
            solvingSequenceLength++;
            numOfButtonPresses[buttonNumberPressed - 1] += 1;
            Debug.LogFormat("[Flowchart Madness #{0}] You pressed number {1}", moduleId, buttonNumberPressed);
        }
        else if (button.GetComponentInChildren<TextMesh>().text.Equals("3")) {
            buttonNumberPressed = 3;
            solvingSequenceDebug += buttonNumberPressed.ToString();
            solvingSequenceLength++;
            numOfButtonPresses[buttonNumberPressed - 1] += 1;
            Debug.LogFormat("[Flowchart Madness #{0}] You pressed number {1}", moduleId, buttonNumberPressed);
        }
        else if (button.GetComponentInChildren<TextMesh>().text.Equals("4")) {
            buttonNumberPressed = 4;
            solvingSequenceDebug += buttonNumberPressed.ToString();
            solvingSequenceLength++;
            numOfButtonPresses[buttonNumberPressed - 1] += 1;
            Debug.LogFormat("[Flowchart Madness #{0}] You pressed number {1}", moduleId, buttonNumberPressed);
        }
        else if (button.GetComponentInChildren<TextMesh>().text.Equals("5")) {
            buttonNumberPressed = 5;
            solvingSequenceDebug += buttonNumberPressed.ToString();
            solvingSequenceLength++;
            numOfButtonPresses[buttonNumberPressed - 1] += 1;
            Debug.LogFormat("[Flowchart Madness #{0}] You pressed number {1}", moduleId, buttonNumberPressed);
        }
        else if (button.GetComponentInChildren<TextMesh>().text.Equals("6")) {
            buttonNumberPressed = 6;
            solvingSequenceDebug += buttonNumberPressed.ToString();
            solvingSequenceLength++;
            numOfButtonPresses[buttonNumberPressed - 1] += 1;
            Debug.LogFormat("[Flowchart Madness #{0}] You pressed number {1}", moduleId, buttonNumberPressed);
        }
        else if (button.GetComponentInChildren<TextMesh>().text.Equals("7")) {
            buttonNumberPressed = 7;
            solvingSequenceDebug += buttonNumberPressed.ToString();
            solvingSequenceLength++;
            numOfButtonPresses[buttonNumberPressed - 1] += 1;
            Debug.LogFormat("[Flowchart Madness #{0}] You pressed number {1}", moduleId, buttonNumberPressed);
        }
        else if (button.GetComponentInChildren<TextMesh>().text.Equals("8")) {
            buttonNumberPressed = 8;
            solvingSequenceDebug += buttonNumberPressed.ToString();
            solvingSequenceLength++;
            numOfButtonPresses[buttonNumberPressed - 1] += 1;
            Debug.LogFormat("[Flowchart Madness #{0}] You pressed number {1}", moduleId, buttonNumberPressed);
        }
        else if (button.GetComponentInChildren<TextMesh>().text.Equals("RESET")) {
            Debug.LogFormat("[Flowchart Madness #{0}] You pressed RESET", moduleId);
            solvingSequenceDebug = "";
            solvingSequenceLength = 0;
            for (int i = 0; i < 8; i++) {
                numOfButtonPresses[i] = 0;
            }
        }
        //Input Rule 17
        if (inputRuleSeventeenActive && !moduleSolved) {
            if (buttonNumberPressed == inputRuleSeventeenCorrectButton && inputRuleSeventeenCorrectButton == 6) {
                Audio.PlaySoundAtTransform("BeachBoysSong", transform);
                GetComponent<KMBombModule>().HandlePass();
                moduleSolved = true;
            }
            else if (buttonNumberPressed == inputRuleSeventeenCorrectButton && inputRuleSeventeenCorrectButton == 7) {
                Audio.PlaySoundAtTransform("ChristmasSong", transform);
                GetComponent<KMBombModule>().HandlePass();
                moduleSolved = true;
            }
            else {
                GetComponent<KMBombModule>().HandleStrike();
                Start();
            }
        }

        //Input Rule 18
        if (inputRuleEighteenActive) {
            if (buttonNumberPressed == inputRuleEighteenCorrectButton) {
                mainUpdateDisplay();
                if (solvingSequenceLength < 10) {
                    inputRuleEighteenPositionIndex = 0;
                }
                else {
                    inputRuleEighteenPositionIndex = 3;
                }
                inputRuleEighteenCorrectButton = (displaySymbolsNumeric[inputRuleEighteenPositionIndex] % 8) + 1;
                Debug.Log("The correct button to press is " + inputRuleEighteenCorrectButton);
                Debug.Log("Solving Sequence is " + solvingSequenceLength + "long");
                Debug.Log("Input Sequence is " + inputSequenceLength + "long");
            }
            else {
                GetComponent<KMBombModule>().HandleStrike();
                mainUpdateDisplay();
                Start();
            }
            if (solvingSequenceLength == inputSequenceLength && !moduleSolved) {
                GetComponent<KMBombModule>().HandlePass();
                Audio.PlaySoundAtTransform("FlowchartMadnessVictory", transform);
                moduleSolved = true;
            }
        }

        //Input Rule 33
        if (inputRuleThirtyThreeActive && !moduleSolved) {
            if (buttonNumberPressed == 1) {
                GetComponent<KMBombModule>().HandlePass();
                moduleSolved = true;
            }
            else {
                for (int i = 0; i < 4; i++)
                {
                    fmRandomSymbolArray = i;
                    if (fmRandomSymbolArray == 1)
                    {
                        fmRandomSymbolArrayID = rnd.Next(0, 10);
                        displaySymbols[i] = digitArray[fmRandomSymbolArrayID];
                        displaySymbolsNumeric[i] = fmRandomSymbolArrayID + 1;
                        displaySymbolsNames[i] = "D";
                    }
                    else if (fmRandomSymbolArray == 0)
                    {
                        fmRandomSymbolArrayID = rnd.Next(0, 26);
                        displaySymbols[i] = latinArray[fmRandomSymbolArrayID];
                        displaySymbolsNumeric[i] = fmRandomSymbolArrayID + 1;
                        displaySymbolsNames[i] = "L";
                    }
                    else if (fmRandomSymbolArray == 2 || fmRandomSymbolArray == 3)
                    {
                        fmRandomSymbolArrayID = rnd.Next(0, 26);
                        if (fmRandomSymbolArrayID == 9 || fmRandomSymbolArrayID == 16)
                        {
                            fmRandomSymbolArrayID++;
                        }
                        displaySymbols[i] = greekArray[fmRandomSymbolArrayID];
                        displaySymbolsNumeric[i] = fmRandomSymbolArrayID + 1;
                        displaySymbolsNames[i] = "G";
                    }
                }
                updateDisplay();
                if (solvingSequenceLength == inputSequenceLength && !moduleSolved) {
                    for (int i = 0; i < 4; i++) {
                        fmRandomSymbolArrayID = 17;
                        displaySymbols[i] = smileyArray[fmRandomSymbolArrayID];
                        displaySymbolsNumeric[i] = fmRandomSymbolArrayID + 1;
                        displaySymbolsNames[i] = "S";
                    }
                    updateDisplay();
                    GetComponent<KMBombModule>().HandlePass();
                    Audio.PlaySoundAtTransform("FlowchartMadnessVictory", transform);
                    moduleSolved = true;
                }
            }
        }
    }

    object getSymbol(string[] symbolArray, int charID) {
        return symbolArray[charID];
    }

    void updateDisplay() {
        screenText.text = displaySymbols[0] + displaySymbols[1] + displaySymbols[2] + displaySymbols[3];
        Debug.Log(displaySymbolsNumeric[0] + " " + displaySymbolsNumeric[1] + " " + displaySymbolsNumeric[2] + " " + displaySymbolsNumeric[3]);
    }

    int getSymbolNumeric(string symbol) {
        symbolNumeric = 0;
        if (digitArray.Contains(symbol)) {
            symbolNumeric = findSymbolInArray(symbol, digitArray);
        }
        else if (latinArray.Contains(symbol))
        {
            symbolNumeric = findSymbolInArray(symbol, latinArray);
        }
        else if (greekArray.Contains(symbol))
        {
            symbolNumeric = findSymbolInArray(symbol, greekArray);
        }
        else if (smileyArray.Contains(symbol))
        {
            symbolNumeric = findSymbolInArray(symbol, smileyArray);
        }
        return symbolNumeric + 1;
    }

    int findSymbolInArray(string symbol, string[] array) {
        for (int i = 0; i < array.Length; i++) {
            if (symbol.Equals(array[i])) {
                return i;
            }
        }
        return 0;
    }

    int checkSymbolEquivalence() {
        greatestEquivalentSymbolCounter = 1;
        for (int i = 0; i < 3; i++) {
            equivalentSymbolCounter = 1;
            for (int j = i + 1; j < 4; j++) {
                if (displaySymbolsNumeric[i] == displaySymbolsNumeric[j]) {
                    equivalentSymbolCounter++;
                }
            }
            if (equivalentSymbolCounter > greatestEquivalentSymbolCounter) {
                greatestEquivalentSymbolCounter = equivalentSymbolCounter;
            }
        }
        return greatestEquivalentSymbolCounter;
    }

    bool isPrime(int number)
    {
        if (number <= 1) return false;
        if (number == 2 || number == 3 || number == 5) return true;
        if (number % 2 == 0 || number % 3 == 0 || number % 5 == 0) return false;

        var boundary = (int)System.Math.Floor(System.Math.Sqrt(number));

        // You can do less work by observing that at this point, all primes 
        // other than 2 and 3 leave a remainder of either 1 or 5 when divided by 6. 
        // The other possible remainders have been taken care of.
        int i = 6; // start from 6, since others below have been handled.
        while (i <= boundary)
        {
            if (number % (i + 1) == 0 || number % (i + 5) == 0)
                return false;

            i += 6;
        }

        return true;
    }

    bool evalCircle() {
        happySymbolDisplayed = false;
        if (displaySymbolsNames.Contains("S")) {
            for (int i = 0; i < 4; i++) {
                if (displaySymbolsNames[i].Equals("S")) {
                    happinessScore = 0;
                    tempSymbol = displaySymbols[i];
                    if (tempSymbol.Equals("t")) {
                        return false;
                    }
                    //Calculate Happiness score
                    if (smileyEyesNorm.Contains(tempSymbol)) {
                        happinessScore = -1;
                    }
                    else if (smileyEyesAngry.Contains(tempSymbol)) {
                        if (Bomb.GetIndicators().Count() == 0) {
                            happinessScore = 3;
                        }
                        else {
                            happinessScore = -3;
                        }
                    }
                    else if (smileyEyesSad.Contains(tempSymbol)) {
                        if (Bomb.GetBatteryCount() % 2 == 0) {
                            happinessScore = 1;
                        }
                        else {
                            happinessScore = -6;
                        }
                    }
                    else if (smileyEyesQuestion.Contains(tempSymbol)) {
                        happinessScore = -5;
                    }

                    if (smileyMouthSmile.Contains(tempSymbol)) {
                        happinessScore += 4;
                    }
                    else if (smileyMouthFrown.Contains(tempSymbol)) {
                        happinessScore -= 4;
                    }
                    else if (smileyMouthLaugh.Contains(tempSymbol)) {
                        happinessScore *= -1;
                    }
                    else if (smileyMouthSad.Contains(tempSymbol)) {
                        happinessScore -= 100;
                    }

                    if (happinessScore >= 0) {
                        happySymbolDisplayed = true;
                    }
                }
            }
        }
        return happySymbolDisplayed;
    }

    bool evalSquare() {
        evalSquareCounter = 0;
        squareSumExists = false;
        for (int i = 0; i < 3; i++) {
            for (int j = i + 1; j < 4; j++) {
                symbolNumericSums[evalSquareCounter] = displaySymbolsNumeric[i] + displaySymbolsNumeric[j];
                evalSquareCounter++;
            }
        }
        foreach (int sum in symbolNumericSums) {
            for (int i = 2; i < 8; i++) {
                if (sum == i * i) {
                    squareSumExists = true;
                }
            }
        }
        return squareSumExists;
    }

    bool evalTriangle() {
        displayedLatinExists = false;
        if (displaySymbolsNames.Contains("L")) {
            for (int i = 0; i < 4; i++) {
                foreach (string indicator in Bomb.GetIndicators()) {
                    if (indicator.ContainsIgnoreCase(displaySymbols[i].ToLower())) {
                        displayedLatinExists = true;
                    }
                }
            }
        }
        return displayedLatinExists;
    }

    bool red(bool s1, bool s2) {
        if (s1 == false && s2 == false) {
            return true;
        }
        else {
            return false;
        }
    }

    bool red(bool s1, bool s2, bool s3) {
        if (s1 == false && s2 == false && s3 == false) {
            return true;
        }
        else {
            return false;
        }
    }

    bool red(bool s1, bool s2, bool s3, bool s4) {
        if (s1 == false && s2 == false && s3 == false && s4 == false) {
            return true;
        }
        else {
            return false;
        }
    }

    bool green(bool s1, bool s2) {
        if (s1 == true || s2 == true) {
            return true;
        }
        else {
            return false;
        }
    }

    bool green(bool s1, bool s2, bool s3) {
        if (s1 == true || s2 == true || s3 == true) {
            return true;
        }
        else {
            return false;
        }
    }

    bool green(bool s1, bool s2, bool s3, bool s4) {
        if (s1 == true || s2 == true || s3 == true || s4 == true) {
            return true;
        }
        else {
            return false;
        }
    }

    bool blue(bool s1, bool s2) {
        if (s1 == s2) {
            return true;
        }
        else {
            return false;
        }
    }

    bool blue(bool s1, bool s2, bool s3) {
        if (s1 == s2 && s1 == s3) {
            return true;
        }
        else {
            return false;
        }
    }

    bool blue(bool s1, bool s2, bool s3, bool s4) {
        if (s1 == s2 && s3 == s4 && s1 == s3) {
            return true;
        }
        else {
            return false;
        }
    }

    void mainUpdateDisplay() {
        for (int i = 0; i < 4; i++)
        {
            fmRandomSymbolArray = rnd.Next(1, 5);
            if (fmRandomSymbolArray == 1)
            {
                fmRandomSymbolArrayID = rnd.Next(0, 10);
                displaySymbols[i] = digitArray[fmRandomSymbolArrayID];
                displaySymbolsNumeric[i] = fmRandomSymbolArrayID + 1;
                displaySymbolsNames[i] = "D";
            }
            else if (fmRandomSymbolArray == 2)
            {
                fmRandomSymbolArrayID = rnd.Next(0, 26);
                displaySymbols[i] = latinArray[fmRandomSymbolArrayID];
                displaySymbolsNumeric[i] = fmRandomSymbolArrayID + 1;
                displaySymbolsNames[i] = "L";
            }
            else if (fmRandomSymbolArray == 3)
            {
                fmRandomSymbolArrayID = rnd.Next(0, 26);
                if (fmRandomSymbolArrayID == 9 || fmRandomSymbolArrayID == 16)
                {
                    fmRandomSymbolArrayID++;
                }
                displaySymbols[i] = greekArray[fmRandomSymbolArrayID];
                displaySymbolsNumeric[i] = fmRandomSymbolArrayID + 1;
                displaySymbolsNames[i] = "G";
            }
            else
            {
                fmRandomSymbolArrayID = rnd.Next(0, 26);
                displaySymbols[i] = smileyArray[fmRandomSymbolArrayID];
                displaySymbolsNumeric[i] = fmRandomSymbolArrayID + 1;
                displaySymbolsNames[i] = "S";
            }
        }
        updateDisplay();
    }

    int performMove(int cellID)
    {
        switch (cellID)
        {
            case 0:
                if (blue(triangle, circle) && red(square, skull)) return 5;
                else return 2;
            case 1:
                if (green(circle, skull)) return 7;
                else return 12;
            case 2:
                if (!skull) return 41;
                else return 19;
            case 3:
                if (blue(triangle, skull)) return 28;
                else return 20;
            case 4:
                if (blue(triangle, square)) return 8;
                else return 14;
            case 5:
                if (blue(square, circle, skull)) return 25;
                else return 17;
            case 6:
                if (!circle) return 31;
                else return 30;
            case 7:
                if (red(circle, square)) return 18;
                else return 27;
            case 8:
                if (blue(square, skull) && green(square, skull)) return 1;
                else return 10;
            case 9:
                if (!square) return 23;
                else return 42;
            case 10:
                if (!circle) return 43;
                else return 15;
            case 11:
                if (blue(!triangle, circle) && triangle) return 26;
                else return 24;
            case 12:
                if (green(!skull, square, triangle)) return 38;
                else return 3;
            case 13:
                if (blue(circle, triangle)) return 32;
                else return 16;
            case 14:
                if (red(circle, skull)) return 0;
                else return 40;
            case 15:
                if (green(circle, triangle) && circle) return 33;
                else return 21;
            case 16:
                if (green(circle, triangle)) return 37;
                else return 34;
            case 17:
                if (red(circle, square, triangle)) return 22;
                else return 39;
            case 18:
                if (square) return 36;
                else return 29;
            case 19:
                if (red(circle, triangle) && !square) return 4;
                else return 35;
            case 20:
                if (green(circle, square, triangle, skull)) return 13;
                else return 6;
            case 21:
                if (circle) return 9;
                else return 11;
            case 22:
                if (red(!square, !triangle)) return 30;
                else return 10;
            case 23:
                if (green(circle, square)) return 37;
                else return 29;
            case 24:
                if (!square) return 22;
                else return 6;
            case 25:
                if (blue(circle, square)) return 9;
                else return 38;
            case 26:
                if (green(circle, square) && red(skull, triangle)) return 17;
                else return 40;
            case 27:
                if (red(triangle, circle)) return 14;
                else return 3;
            case 28:
                if (green(triangle, square)) return 16;
                else return 25;
            case 29:
                if (green(skull, !square, triangle)) return 32;
                else return 4;
            case 30:
                if (green(triangle, !circle) && !triangle) return 1;
                else return 41;
            case 31:
                if (blue(skull, square) && !circle) return 39;
                else return 20;
            case 32:
                if (blue(triangle, square, skull, circle)) return 43;
                else return 33;
            case 33:
                if (red(triangle, skull)) return 24;
                else return 2;
            case 34:
                if (triangle) return 0;
                else return 5;
            case 35:
                if (green(skull, triangle) && skull && triangle) return 27;
                else return 18;
            case 36:
                if (red(triangle, square)) return 7;
                else return 15;
            case 37:
                if (red(circle, square, skull, triangle)) return 42;
                else return 13;
            case 38:
                if (!triangle) return 31;
                else return 28;
            case 39:
                if (!skull) return 8;
                else return 21;
            case 40:
                if (blue(circle, skull)) return 36;
                else return 11;
            case 41:
                if (circle && square && triangle && skull) return 23;
                else return 26;
            case 42:
                if (skull) return 12;
                else return 19;
            case 43:
                if (green(skull, triangle)) return 34;
                else return 35;
            default:
                Debug.Log("You've moved to a space that doesn't exist. This should not have happened.");
                return 100;
        }
    }

    void appendInputSequence(int num) {
        inputSequenceDebug += num.ToString();
        inputSequenceLength += num.ToString().Length;
    }

    void calculateInputSequence(int rule) {
        coinToss = rnd.Next(0, 2);
        if (coinToss == 1) {
            Debug.Log("Additional Rule in place!");
        }

        switch (rule) {
            case 0:
                for (int i = 1; i < 9; i++) {
                    appendInputSequence(i);
                }
                break;
            case 1:
                for (int i = 8; i > 0; i--) {
                    appendInputSequence(i);
                }
                if (coinToss == 1) {
                    for (int i = 1; i < 9; i++) {
                        appendInputSequence(i);
                    }
                }
                break;
            case 2:
                for (int i = 0; i < 10; i++) {
                    appendInputSequence((int)System.Math.Pow(2,i));
                }
                break;
            case 3:
                for (int i = 1; i < 5; i++) {
                    appendInputSequence(i);
                    appendInputSequence(i);
                }
                if (coinToss == 1) {
                    appendInputSequence(55);
                }
                break;
            case 4:
                randomNumberOfPresses = rnd.Next(10, 31);
                for (int i = 0; i < randomNumberOfPresses; i++) {
                    appendInputSequence(1);
                }
                break;
            case 5:
                for (int i = 1; i < 5; i++) {
                    appendInputSequence(i);
                }
                if (coinToss == 1) {
                    for (int i = 4; i > 0; i--) {
                        appendInputSequence(i);
                    }
                }
                break;
            case 6:
                if (!displaySymbolsNames.Contains("G")) {
                    appendInputSequence(7482);
                }
                else {
                    for (int i = 0; i < 4; i++) {
                        if (displaySymbolsNames[i].Equals("G")) {
                            appendInputSequence((displaySymbolsNumeric[i] % 8) + 1);
                        }
                    }
                }
                if (coinToss == 1) {
                    appendInputSequence(7482);
                }
                break;
            case 7:
                for (int i = 0; i < 4; i++) {
                    inputRuleSevenCode[i] = (displaySymbolsNumeric[i] % 8) + 1;
                    appendInputSequence(inputRuleSevenCode[i]);
                }
                if (coinToss == 1) {
                    for (int i = 0; i < 4; i++) {
                        if (inputRuleSevenCode[i] == 8) {
                            inputRuleSevenCode[i] = 1;
                        }
                        else {
                            inputRuleSevenCode[i] = inputRuleSevenCode[i] + 1;
                        }
                        appendInputSequence(inputRuleSevenCode[i]);
                    }
                }
                break;
            case 8:
                appendInputSequence(8);
                break;
            case 9:
                if (displaySymbolsNumeric.Contains(9)) {
                    for (int i = 0; i < 4; i++) {
                        if (displaySymbolsNumeric[i] == 9) {
                            appendInputSequence(i + 1);
                        }
                    }
                }
                else {
                    appendInputSequence(8);
                }
                if (coinToss == 1) {
                    for (int i = 0; i < 9; i++) {
                        appendInputSequence(8);
                    }
                }
                break;
            case 10:
                randomNumberOfPresses = rnd.Next(10, 31);
                for (int i = 0; i < randomNumberOfPresses; i++) {
                    appendInputSequence(7);
                }
                break;
            case 11:
                appendInputSequence(1);
                appendInputSequence(1);
                if (coinToss == 1) {
                    appendInputSequence(725582);
                    appendInputSequence(43811);
                }
                break;
            case 12:
                appendInputSequence(2);
                break;
            case 13:
                appendInputSequence(71858312);
                if (coinToss == 1) {
                    appendInputSequence(44218273);
                }
                break;
            case 14:
                for (int i = 0; i < totalMoves; i++) {
                    appendInputSequence(5);
                }
                if (coinToss == 1) {
                    appendInputSequence(5);
                }
                break;
            case 15:
                randomNumberOfPresses = rnd.Next(10, 31);
                for (int i = 0; i < randomNumberOfPresses; i++)
                {
                    appendInputSequence(2);
                }
                break;
            case 16:
                randomNumberOfPresses = rnd.Next(3, 11);
                for (int i = 0; i < randomNumberOfPresses; i++)
                {
                    appendInputSequence(2468);
                }
                break;
            case 17:
                inputRuleSeventeenActive = true;
                inputRuleSeventeenCorrectButton = 7;
                for (int i = 0; i < 4; i++) {
                    if (displaySymbolsNumeric[i] == 19 || displaySymbolsNumeric[i] == 21 || displaySymbolsNumeric[i] == 13 || displaySymbolsNumeric[i] == 5 || displaySymbolsNumeric[i] == 18) {
                        inputRuleSeventeenCorrectButton = 6;
                    }
                }
                appendInputSequence(inputRuleSeventeenCorrectButton);
                break;
            case 18:
                inputRuleEighteenActive = true;
                inputSequenceLength = 10;
                if (coinToss == 1) {
                    inputSequenceLength = 20;
                }
                inputRuleEighteenCorrectButton = (displaySymbolsNumeric[inputRuleEighteenPositionIndex] % 8) + 1;
                Debug.Log("The correct button to press is " + inputRuleEighteenCorrectButton);
                break;
            case 19:
                randomNumberOfPresses = rnd.Next(10, 31);
                for (int i = 0; i < randomNumberOfPresses; i++) {
                    appendInputSequence(8);
                }
                break;
            case 20:
                randomNumberOfPresses = rnd.Next(10, 31);
                for (int i = 0; i < randomNumberOfPresses; i++) {
                    appendInputSequence(3);
                }
                break;
            case 21:
                appendInputSequence(8);
                if (coinToss == 1) {
                    calculateInputSequence(22);
                }
                break;
            case 22:
                appendInputSequence(6);
                if (coinToss == 1) {
                    calculateInputSequence(23);
                }
                break;
            case 23:
                appendInputSequence(3);
                if (coinToss == 1) {
                    calculateInputSequence(24);
                }
                break;
            case 24:
                appendInputSequence(4);
                if (coinToss == 1) {
                    calculateInputSequence(25);
                }
                break;
            case 25:
                appendInputSequence(5);
                if (coinToss == 1) {
                    calculateInputSequence(inputSequenceDebug[0]);
                }
                break;
            case 26:
                if (!displaySymbolsNames.Contains("S")) {
                    appendInputSequence(1835);
                }
                else {
                    for (int i = 0; i < 4; i++) {
                        if (displaySymbolsNames[i].Equals("S")) {
                            appendInputSequence((displaySymbolsNumeric[i] % 8) + 1);
                        }
                    }
                }
                if (coinToss == 1) {
                    appendInputSequence(1835);
                }
                break;
            case 27:
                appendInputSequence(7);
                break;
            case 28:
                appendInputSequence(1);
                if (coinToss == 1) {
                    calculateInputSequence((displaySymbolsNumeric[0] + displaySymbolsNumeric[1] + displaySymbolsNumeric[2] + displaySymbolsNumeric[3]) % 44);
                }
                break;
            case 29:
                appendInputSequence(7);
                break;
            case 30:
                randomNumberOfPresses = rnd.Next(10, 31);
                for (int i = 0; i < randomNumberOfPresses; i++) {
                    appendInputSequence(4);
                }
                break;
            case 31:
                greekLetterCount = 0;
                smileyGlyphCount = 0;
                for (int i = 0; i < 4; i++) {
                    if (displaySymbolsNames[i].Equals("G")) {
                        greekLetterCount++;
                        appendInputSequence(6);
                    }
                    else if (displaySymbolsNames[i].Equals("S")) {
                        smileyGlyphCount++;
                    }
                }
                if (inputSequenceLength == 0) {
                    appendInputSequence(6);
                }
                if (coinToss == 1) {
                    if (greekLetterCount > smileyGlyphCount) {
                        appendInputSequence(8);
                    }
                    else if (greekLetterCount < smileyGlyphCount) {
                        appendInputSequence(1);
                    }
                    else {
                        appendInputSequence(365);
                    }
                }
                break;
            case 32:
                appendInputSequence(25);
                break;
            case 33:
                inputRuleThirtyThreeActive = true;
                inputSequenceLength = rnd.Next(6,16);
                break;
            case 34:
                if (!displaySymbolsNames.Contains("D")) {
                    appendInputSequence(3174);
                }
                else {
                    for (int i = 0; i < 4; i++) {
                        if (displaySymbolsNames[i].Equals("D")) {
                            appendInputSequence((displaySymbolsNumeric[i] % 8) + 1);
                        }
                    }
                }
                if (coinToss == 1) {
                    appendInputSequence(3174);
                }
                break;
            case 35:
                for (int i = 0; i < 4; i++) {
                    appendInputSequence((displaySymbolsNumeric[i] % 8) + 1);
                }
                if (coinToss == 1)
                {
                    appendInputSequence((displaySymbolsNumeric[1] % 8) + 1);
                    appendInputSequence((displaySymbolsNumeric[3] % 8) + 1);
                }
                break;
            case 36:
                randomNumberOfPresses = rnd.Next(10, 31);
                for (int i = 0; i < randomNumberOfPresses; i++) {
                    appendInputSequence(5);
                }
                break;
            case 37:
                appendInputSequence(3176533);
                appendInputSequence(1367126);
                if (coinToss == 1) {
                    calculateInputSequence(38);
                }
                break;
            case 38:
                appendInputSequence(1818587);
                appendInputSequence(3761515);
                if (coinToss == 1)
                {
                    calculateInputSequence(37);
                }
                break;
            case 39:
                randomNumberOfPresses = rnd.Next(3, 11);
                for (int i = 0; i < randomNumberOfPresses; i++)
                {
                    appendInputSequence(1357);
                }
                break;
            case 40:
                for (int i = 0; i < 40; i++)
                {
                    appendInputSequence(4);
                }
                if (coinToss == 1) {
                    appendInputSequence(3);
                }
                break;
            case 41:
                appendInputSequence(5);
                break;
            case 42:
                randomNumberOfPresses = rnd.Next(10, 31);
                for (int i = 0; i < randomNumberOfPresses; i++)
                {
                    appendInputSequence(6);
                }
                break;
            case 43:
                for (int i = 8; i > 0; i--) {
                    if (i != displaySymbolsNumeric[0] % 8 + 1 && i != displaySymbolsNumeric[1] % 8 + 1 && i != displaySymbolsNumeric[2] % 8 + 1 && i != displaySymbolsNumeric[3] % 8 + 1) {
                        appendInputSequence(i);
                    }
                }
                if (coinToss == 1) {
                    for (int i = 0; i < 9; i++) {
                        if (i == displaySymbolsNumeric[0] % 8 + 1 || i == displaySymbolsNumeric[1] % 8 + 1 || i == displaySymbolsNumeric[2] % 8 + 1 || i == displaySymbolsNumeric[3] % 8 + 1) {
                            appendInputSequence(i);
                        }
                    }
                }
                break;
        }
    }

}
