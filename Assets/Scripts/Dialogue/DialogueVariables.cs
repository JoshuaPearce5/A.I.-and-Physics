using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class DialogueVariables
{
    //Dictionary is publicly readable with this syntax ({ get; private set; })
    public Dictionary<string, Ink.Runtime.Object> variables { get; private set; }

    public DialogueVariables(TextAsset loadGlobalsJSON)
    {
        // Create the story
        Story globalVariablesStory = new Story(loadGlobalsJSON.text);

        // Initialize the dictionary
        variables = new Dictionary<string, Ink.Runtime.Object>();
        // Loop through each global variable name in the Globals file
        foreach (string name in globalVariablesStory.variablesState)
        {
            // For each variable name, get its current value
            Ink.Runtime.Object value = globalVariablesStory.variablesState.GetVariableWithName(name);
            // And add it to the dictionary
            variables.Add(name, value);
            // Debug to see if variables are being added as intended
            //Debug.Log("Initialized global dialogue variable: " + name + " = " + value);
        }
    }

    public void StartListening(Story story)
    {
        //It's important that this call (VariablesToStory) is assigned before the listener (variable observer)
        VariablesToStory(story);
        story.variablesState.variableChangedEvent += VariableChanged;
    }

    public void StopListening(Story story)
    {
        story.variablesState.variableChangedEvent -= VariableChanged;
    }

    private void VariableChanged(string name, Ink.Runtime.Object value)
    {
        //Debug.Log("Variable changed: " + name + " = " + value);

        //Only maintain variables that were firstly initialized from the Globals Ink file
        //Check if the name is contained in the dictionary with "ContainsKey"
        if (variables.ContainsKey(name))
        {
            //If it is, remove the old entry
            variables.Remove(name);
            //Then, add the updated entry with the new value
            variables.Add(name, value);
        }
    }

    //Load the variables into each Ink story
    private void VariablesToStory(Story story)
    {
        //Loop through each entry in the dictionary. call story.VariableState.setGlobal, pass in the Key and Value
        foreach(KeyValuePair<string, Ink.Runtime.Object> variable in variables)
        {
            story.variablesState.SetGlobal(variable.Key, variable.Value);
        }
    }
}
