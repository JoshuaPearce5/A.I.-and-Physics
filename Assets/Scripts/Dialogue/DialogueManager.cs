using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Ink.Runtime;
using System.Xml.Serialization;
using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour
{
    [Header("Params")]
    // Change the typing speed of the text here. The smaller the number, the faster the typing
    [SerializeField] private float typingSpeed = 0.04f;

    [Header("Load Globals JSON")]
    [SerializeField] private TextAsset loadGlobalsJSON;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject continueIcon;

    [SerializeField] private TextMeshProUGUI dialogueText;

    [SerializeField] private TextMeshProUGUI displayNameText;
    [SerializeField] private Animator portraitAnimator;
    private Animator layoutAnimator;

    [Header("Audio")]
    [SerializeField] private AudioClip prompt1SoundClip;
    [SerializeField] private AudioClip prompt2SoundClip;

    [SerializeField] private AudioClip[] dialogueTypingSoundClips;

    [Range(1, 5)]
    [SerializeField] private int frequencyLevel = 2;

    [Range(-3, 3)]
    [SerializeField] private float minPitch = 0.5f;
    [Range(-3, 3)]
    [SerializeField] private float maxPitch = 3f;

    [SerializeField] private bool stopAudioSource;

    [SerializeField] private bool makePredictable;

    private AudioSource audioSource;

    /*
    [Header("Choices UI")]
    [SerializeField] private GameObject[] choices;
    private TextMeshProUGUI[] choicesText;
    */

    [SerializeField] PlayerCharacterController controller;

    private Story currentStory;

    public bool dialogueIsPlaying { get; private set; }

    // Used to prevent continuing to the next line before the text typing is done
    private bool canContinueToNextLine = false;

    // Used to make sure the user skipping through the dialogue by clicking doesn't cause issues
    private bool canSkip = false;
    private bool submitSkip = false;

    private Coroutine displayLineCoroutine;

    private static DialogueManager instance;

    //Tags
    private const string SPEAKER_TAG = "speaker";
    private const string PORTRAIT_TAG = "portrait";
    private const string LAYOUT_TAG = "layout";

    private DialogueVariables dialogueVariables;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Warning: More than one Dialogue Manager present in the scene");
        }
        instance = this;

        dialogueVariables = new DialogueVariables(loadGlobalsJSON);

        audioSource = this.gameObject.AddComponent<AudioSource>();
    }

    public static DialogueManager GetInstance()
    {
        return instance;
    }

    private void Start()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);

        //Get the Layout animator
        layoutAnimator = dialoguePanel.GetComponent<Animator>();

        //Get choices text
        /*
        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach (GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }
        */
    }

    private void Update()
    {
        // Make a request to skip
        if (Input.GetButtonDown("Jump"))
        {
            submitSkip = true;
        }

        // return immediately if dialogue isn't playing
        if (!dialogueIsPlaying)
        {
            // Enable controls
            controller.data.controlsDisabled = false;

            controller.data.rb.constraints = RigidbodyConstraints2D.FreezeRotation;

            return;
        }

        if (dialogueIsPlaying)
        {
            // Disable controls
            controller.data.controlsDisabled = true;

            // Freeze both X position and rotation
            controller.data.rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }

        // handles continuing on to the next line in the dialogue when submit is pressed
        // currentStory.currentChoices.Count == 0 makes sure that if there are choices to be made, the "if" block won't run
        // Added the "canContinueToNextLine" condition. This needs to be true to allow the user to continue.

        //if (canContinueToNextLine && currentStory.currentChoices.Count == 0 && Input.GetKeyDown(KeyCode.Space))
        if (canContinueToNextLine && Input.GetButtonDown("Jump"))
        {
            SoundFXManager.instance.PlaySoundFXClip(prompt2SoundClip, transform, 1f);
            ContinueStory();
        }
    }

    public void EnterDialogueMode(TextAsset inkJSON)
    {
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);

        //Start listening when the story is created
        dialogueVariables.StartListening(currentStory);

        //Reset portrait, layout and speaker. These are the default values displayed if the tags have not been defined.
        displayNameText.text = "???";
        portraitAnimator.Play("Default");
        layoutAnimator.Play("right");

        ContinueStory();
    }

    private IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSeconds(0.2f);

        //Stop listening when the story ends
        dialogueVariables.StopListening(currentStory);

        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
    }

    // Continue to a new line of dialogue
    private void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            // Set the text for the current line of dialogue
            // dialogueText.text = currentStory.Continue();

            // Coroutine displays the dialogue, one letter at a time.
            // If statement prevents multiple coroutines attempting to run at the same time, causing letters to be loaded from various lines of dialogue
            // Replaced the line above.
            if (displayLineCoroutine != null)
            {
                StopCoroutine(displayLineCoroutine);
            }

            displayLineCoroutine = StartCoroutine(DisplayLine(currentStory.Continue()));

            // Display choices, if any
            // Moved to the Coroutine, displaying the choices after the line has been typed out
            //DisplayChoices();

            // Handle tags
            HandleTags(currentStory.currentTags);
        }
        else
        {
            StartCoroutine(ExitDialogueMode());
        }
    }

    //Dialogue with no delay after closing the last panel
    /*
    private void ExitDialogueMode()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
    }

    private void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            dialogueText.text = currentStory.Continue();
        }
        else
        {
            ExitDialogueMode();
        }
    }
    */

    private IEnumerator CanSkip()
    {
        // Ensure the variable is false
        canSkip = false;

        yield return new WaitForSeconds(0.05f);

        canSkip = true;
    }

    // Adding typing text effect to dialogue
    private IEnumerator DisplayLine(string line)
    {
        // Clear the dialogue text so the previous line is no longer shown
        dialogueText.text = line;
        dialogueText.maxVisibleCharacters = 0;

        // Hide the continue icon, as the user cannot continue
        continueIcon.SetActive(false);

        // Hide any choices that may be showing currently
        //HideChoices();

        // Prevents the user from skipping past the dialogue
        canContinueToNextLine = false;

        // Prevent skipping until the conditions are met
        submitSkip = false;

        bool isAddingRichTextTag = false;

        StartCoroutine(CanSkip());

        // Making each letter display one at a time
        foreach (char letter in line.ToCharArray())
        {
            // If the user clicks whilst the text is typing, it will skip the typing animation 
            //if (Input.GetKeyDown(KeyCode.Mouse0))

            // This has been changed to work with variables instead of just "Mouse0", ensuring the text skipping works as intended
            if (canSkip && submitSkip)
            {
                submitSkip = false;
                dialogueText.maxVisibleCharacters = line.Length;
                break;
            }

            // Check for the rich text tags. If found, add it without waiting (WaitForSeconds)
            if (letter == '<' || isAddingRichTextTag)
            {
                isAddingRichTextTag = true;
                if (letter == '>')
                {
                    isAddingRichTextTag = false;
                }
            }

            // If rich text is not present, add the next letter normally
            else
            {
                PlayDialogueSound(dialogueText.maxVisibleCharacters, dialogueText.text[dialogueText.maxVisibleCharacters]);
                dialogueText.maxVisibleCharacters++;
                yield return new WaitForSeconds(typingSpeed);
            }
        }

        // Show the continue icon, indicating that the user can continue
        SoundFXManager.instance.PlaySoundFXClip(prompt1SoundClip, transform, 1f);
        continueIcon.SetActive(true);

        // Display the choices, as the user has seen the text type out and can now make a decision
        //DisplayChoices();

        // The text is done typing, so the dialogue can now be skipped past
        canContinueToNextLine = true;

        // Prevent skip
        canSkip = false;
    }

    private void PlayDialogueSound(int currentDisplayedCharacterCount, char currentCharacter)
    {
        if (currentDisplayedCharacterCount % frequencyLevel  == 0)
        {
            if (stopAudioSource)
            {
                audioSource.Stop();
            }

            AudioClip soundClip = null;

            // Make the dialogue sound predictable through hashing
            if (makePredictable)
            {
                int hashCode = currentCharacter.GetHashCode();

                // Sound Clip
                int predictableIndex = hashCode % dialogueTypingSoundClips.Length;
                soundClip = dialogueTypingSoundClips[predictableIndex];

                // Pitch
                int minPitchInt = (int) (minPitch * 100);
                int maxPitchInt = (int) (minPitch * 100);
                int pitchRangeInt = maxPitchInt - minPitchInt;

                // Cannot divide by 0, so if there is no range, then skip the selection
                if (pitchRangeInt != 0)
                {
                    int predictablePitchInt = (hashCode % pitchRangeInt) + minPitchInt;
                    float predictablePitch = predictablePitchInt / 100f;
                    audioSource.pitch = predictablePitch;
                }
                else
                {
                    audioSource.pitch = minPitch;
                }
            }
            // Randomize the audio
            else
            {
                // Sound Clip
                int randomIndex = Random.Range(0, dialogueTypingSoundClips.Length);
                soundClip = dialogueTypingSoundClips[randomIndex];

                // Random Pitch
                audioSource.pitch = Random.Range(minPitch, maxPitch);
            }

            // Play Sound
            audioSource.PlayOneShot(soundClip);
        }
    }

    /*
    private void HideChoices()
    {
        foreach (GameObject choiceButton in choices)
        {
            choiceButton.SetActive(false);
        }
    }
    */

    private void HandleTags(List<string> currentTags)
    {
        // Loop through each tag respectively and handle it accordingly
        foreach (string tag in currentTags)
        {
            //Parse the tag
            string[] splitTag = tag.Split(':');
            if (splitTag.Length != 2)
            {
                Debug.LogError("Following tag could not be parsed: " + tag);
            }
            string tagKey = splitTag[0].Trim();
            string tagValue = splitTag[1].Trim();

            //Handle the tag
            switch (tagKey)
            {
                case SPEAKER_TAG:
                    displayNameText.text = tagValue;
                    break;
                case PORTRAIT_TAG:
                    portraitAnimator.Play(tagValue);
                    break;
                case LAYOUT_TAG:
                    layoutAnimator.Play(tagValue);
                    break;
                default:
                    Debug.LogWarning("Tag came in, but is not currently being handled: " + tag);
                    break;
            }
        }
    }

    /*
    private void DisplayChoices()
    {
        //Retrieve the current choices from the current story
        List<Choice> currentChoices = currentStory.currentChoices;

        //Can the current UI actually support the amount of choices coming in?
        if (currentChoices.Count > choices.Length)
        {
            Debug.LogError("More choices were given than the UI can support. Number of choices given: " + currentChoices.Count);
        }

        int index = 0;
        //Enable and initialize the choices up to the amount of choices for this line of dialogue
        foreach(Choice choice in currentChoices)
        {
            choices[index].gameObject.SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }
        //Go through the remaining choices that the UI can support and make sure they are hidden
        for (int i = index; i < choices.Length; i++)
        {
            choices[i].gameObject.SetActive(false);
        }

        StartCoroutine(SelectFirstChoice());
    }


    private IEnumerator SelectFirstChoice()
    {
        //Event System requires we clear it first, then wait for at least one frame before we set the current selected object

        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        //EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
    }


    public void MakeChoice(int choiceIndex)
    {
        if (canContinueToNextLine)
        {
        currentStory.ChooseChoiceIndex(choiceIndex);

        //This line is necessary specifically for the InputManager code being used
        //Input Manager has been removed
        //InputManager.GetInstance().RegisterSubmitPressed();

        ContinueStory();
        }
    }
    */

    // Takes in a string for the variable's name
    public Ink.Runtime.Object GetVariableState(string variableName)
    {
        Ink.Runtime.Object variableValue = null;
        // Reference the dictionary in the dialogueVariables object
        dialogueVariables.variables.TryGetValue(variableName, out variableValue);
        if (variableValue == null)
        {
            // If the variable doesn't exist, there is a debug message
            Debug.LogWarning("Ink Variable was found to be null: " + variableName);
        }
        // Assuming the variable exists, return it
        return variableValue;
    }
}
