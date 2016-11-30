#load "Tank.csx"
#load "Wall.csx"
#load "Fire.csx"

public class CommandRequest
{
    public string matchId { get; set; }

    public int mapWidth { get; set; }

    public int mapHeight { get; set; }

    public int wallDamage { get; set; }

    public int weaponRange { get; set; }

    public int tankDamage { get; set; }

    public int weaponDamage { get; set; }

    public int visibility { get; set; }

    public Tank you { get; set; }

    public Tank[] enemies { get; set; }

    public Wall[] walls { get; set; }

    public int suddenDeath { get; set; }

    public Fire[] fire { get; set; }
}