namespace HereticalSolutions.Entities
{
    public enum EEntityAuthoringPresets
    {
        DEFAULT,                    //For single player
        
        NONE,                       //For lobbies

        NETWORKING_HOST,            //For host players with screen

        NETWORKING_HOST_HEADLESS,   //For servers without screen

        NETWORKING_CLIENT           //For clients
    }
}