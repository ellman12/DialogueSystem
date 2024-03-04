namespace DialogueSystem.Data
{
    public enum DialogueType
    {
        ///The node is just for storing text, without presenting the player with any kind of choice.
        Text,
        
        ///The node prompts the user with at least one choice.
        Prompt
    }
}