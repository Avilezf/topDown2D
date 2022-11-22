internal class PlayConfiguration
{
    private int tower;
    private int player;

    public PlayConfiguration(int tower, int player)
    {
        this.tower = tower;
        this.player = player;
    }

    public int towers 
    {
        get { return tower; }   // get method
        set { tower = value; }  // set method
    }

    public int players 
    {
        get { return player; }   // get method
        set { player = value; }  // set method
    }
}