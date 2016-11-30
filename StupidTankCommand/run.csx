#load "Models\Command.csx"
#load "Models\Tank.csx"
#load "Models\Wall.csx"
#load "Models\Fire.csx"
#load "Models\CommandRequest.csx"

using System.Net;
using System.Threading.Tasks;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    var commandRequest = await ParseRequest(req, log);

    Command command = new Command()
    {
        command = ChooseCommand(commandRequest, log)
    };

    return req.CreateResponse(HttpStatusCode.OK, command);
}

public static async Task<CommandRequest> ParseRequest(HttpRequestMessage req, TraceWriter log)
{
    CommandRequest commandRequest = null;

    try
    {
        commandRequest = await req.Content.ReadAsAsync<CommandRequest>();
    }
    catch (System.Exception ex)
    {
        log.Error(ex.Message);
    }
    
    if (commandRequest != null)
    {
        log.Info($"matchId: {commandRequest.matchId}");
        log.Info($"mapWidth: {commandRequest.mapWidth}");
        log.Info($"mapHeight: {commandRequest.mapHeight}");
        log.Info($"wallDamage: {commandRequest.wallDamage}");
        log.Info($"tankDamage: {commandRequest.tankDamage}");
        log.Info($"weaponDamage: {commandRequest.weaponDamage}");
        log.Info($"weaponRange: {commandRequest.weaponRange}");
        log.Info($"visibility: {commandRequest.visibility}");
        log.Info($"suddenDeath: {commandRequest.suddenDeath}");

        log.Info($"you.direction: {commandRequest.you.direction}");
        log.Info($"you.x: {commandRequest.you.x}");
        log.Info($"you.y: {commandRequest.you.y}");
        log.Info($"you.strength: {commandRequest.you.strength}");
        log.Info($"you.ammo: {commandRequest.you.ammo}");
        log.Info($"you.status: {commandRequest.you.status}");
        log.Info($"you.targetRange: {commandRequest.you.targetRange}");

        if (commandRequest.enemies?.Length > 0)
        {
            log.Info($"Enemy count: {commandRequest.enemies.Length}");
            
            foreach (var enemy in commandRequest.enemies)
            {
                log.Info($"enemy.direction: {enemy.direction}");
                log.Info($"enemy.x: {enemy.x}");
                log.Info($"enemy.y: {enemy.y}");
                log.Info($"enemy.strength: {enemy.strength}");
                log.Info($"enemy.ammo: {enemy.ammo}");
                log.Info($"enemy.status: {enemy.status}");
                log.Info($"enemy.targetRange: {enemy.targetRange}");
            }
        }

        if (commandRequest.walls?.Length > 0)
        {
            log.Info($"Wall count: {commandRequest.walls.Length}");

            foreach (var wall in commandRequest.walls)
            {
                log.Info($"wall.x: {wall.x}");
                log.Info($"wall.y: {wall.y}");
                log.Info($"wall.strength: {wall.strength}");
            }
        }

        if (commandRequest.fire?.Length > 0)
        {
            log.Info($"Fire count: {commandRequest.fire.Length}");

            foreach (var f in commandRequest.fire)
            {
                log.Info($"f.x: {f.x}");
                log.Info($"f.y: {f.y}");
            }
        }
    }
    else
    {
        log.Warning("Command request was invalid");
    }

    return commandRequest;
}

public static bool IsEnemyVisible(Tank[] enemies, TraceWriter log)
{
    foreach (var enemy in enemies)
    {
        if (enemy.x == null && enemy.y == null)
        {
            continue;
        }

        return true;
    }

    return false;
}

public static string SiegeMode(Tank myTank, Tank[] enemies, int weaponRange, TraceWriter log)
{
    string command = "pass";

    foreach (var enemy in enemies)
    {
        if (enemy.x == null && enemy.y == null)
        {
            continue;
        }

        var xdiff = myTank.x.Value - enemy.x.Value;
        var ydiff = myTank.y.Value - enemy.y.Value;

        if (xdiff == 0)
        {
            if (ydiff < 0)
            {
                // Unten

                if (myTank.direction == "bottom")
                {
                    // Schiessposition

                    if (Math.Abs(ydiff) <= weaponRange)
                    {
                        command = "fire";
                    }
                }
                else
                {
                    command = RotationCommand(myTank.direction, "bottom", log);
                }
            }
            else
            {
                // Oben

                if (myTank.direction == "top")
                {
                    // Schiessposition

                    if (Math.Abs(ydiff) <= weaponRange)
                    {
                        command = "fire";
                    }
                }
                else
                {
                    command = RotationCommand(myTank.direction, "top", log);
                }
            }
        }
        else if (xdiff < 0)
        {
            // Rechts

            if (ydiff == 0)
            {
                if (myTank.direction == "right")
                {
                    // Schiessposition

                    if (Math.Abs(xdiff) <= weaponRange)
                    {
                        command = "fire";
                    }
                }
                else
                {
                    command = RotationCommand(myTank.direction, "right", log);
                }
            }
            else if (ydiff < 0)
            {
                // Rechts Unten

                if (Math.Abs(xdiff) < Math.Abs(ydiff))
                {
                    if (myTank.direction == "bottom")
                    {
                        // Schiessposition

                        if (Math.Abs(ydiff) <= weaponRange)
                        {
                            if (xdiff == 0)
                            {
                                command = "fire";
                            }
                        }
                    }
                    else
                    {
                        command = RotationCommand(myTank.direction, "bottom", log);
                    }
                }
                else
                {
                    if (myTank.direction == "right")
                    {
                        // Schiessposition

                        if (Math.Abs(xdiff) <= weaponRange)
                        {
                            if (ydiff == 0)
                            {
                                command = "fire";
                            }
                        }
                    }
                    else
                    {
                        command = RotationCommand(myTank.direction, "right", log);
                    }
                }
            }
            else
            {
                // Rechts Oben

                if (Math.Abs(xdiff) < Math.Abs(ydiff))
                {
                    if (myTank.direction == "top")
                    {
                        // Schiessposition

                        if (Math.Abs(ydiff) <= weaponRange)
                        {
                            if (xdiff == 0)
                            {
                                command = "fire";
                            }
                        }
                    }
                    else
                    {
                        command = RotationCommand(myTank.direction, "top", log);
                    }
                }
                else
                {
                    if (myTank.direction == "right")
                    {
                        // Schiessposition

                        if (Math.Abs(xdiff) <= weaponRange)
                        {
                            if (ydiff == 0)
                            {
                                command = "fire";
                            }
                        }
                    }
                    else
                    {
                        command = RotationCommand(myTank.direction, "right", log);
                    }
                }
            }
        }
        else
        {
            // Links

            if (ydiff == 0)
            {
                if (myTank.direction == "left")
                {
                    // Schiessposition

                    if (Math.Abs(xdiff) <= weaponRange)
                    {
                        command = "fire";
                    }
                }
                else
                {
                    command = RotationCommand(myTank.direction, "left", log);
                }
            }
            else if (ydiff < 0)
            {
                // Links Unten

                if (Math.Abs(xdiff) < Math.Abs(ydiff))
                {
                    if (myTank.direction == "bottom")
                    {
                        // Schiessposition

                        if (Math.Abs(ydiff) <= weaponRange)
                        {
                            if (xdiff == 0)
                            {
                                command = "fire";
                            }
                        }
                    }
                    else
                    {
                        command = RotationCommand(myTank.direction, "bottom", log);
                    }
                }
                else
                {
                    if (myTank.direction == "left")
                    {
                        // Schiessposition

                        if (Math.Abs(xdiff) <= weaponRange)
                        {
                            if (ydiff == 0)
                            {
                                command = "fire";
                            }
                        }
                    }
                    else
                    {
                        command = RotationCommand(myTank.direction, "left", log);
                    }
                }
            }
            else
            {
                // Links Oben

                if (Math.Abs(xdiff) < Math.Abs(ydiff))
                {
                    if (myTank.direction == "top")
                    {
                        // Schiessposition

                        if (Math.Abs(ydiff) <= weaponRange)
                        {
                            if (xdiff == 0)
                            {
                                command = "fire";
                            }
                        }
                    }
                    else
                    {
                        command = RotationCommand(myTank.direction, "top", log);
                    }
                }
                else
                {
                    if (myTank.direction == "left")
                    {
                        // Schiessposition

                        if (Math.Abs(xdiff) <= weaponRange)
                        {
                            if (ydiff == 0)
                            {
                                command = "fire";
                            }
                        }
                    }
                    else
                    {
                        command = RotationCommand(myTank.direction, "left", log);
                    }
                }
            }
        }
    }

    return command;
}

public static string ChooseCommand(CommandRequest commandRequest, TraceWriter log)
{
    string command = "pass";

    var myTank = commandRequest.you;

    if (IsEnemyVisible(commandRequest.enemies, log))
    {
        command = SiegeMode(myTank, commandRequest.enemies, commandRequest.weaponRange, log);
    }
    else
    {
        // Search

        // Ist eine Wall vor uns in weapon range
        command = Search(commandRequest.walls, myTank, commandRequest.weaponRange, log);

    }
    

    return command;
}

public static string Search(Wall[] walls, Tank myTank, int weaponRange, TraceWriter log)
{
    string command = "pass";

    if (myTank.x > 6)
    {
        if (myTank.direction != "left")
        {
            command = RotationCommand(myTank.direction, "left", log);
        }
        else
        {
            command = HandleWall(walls, myTank);
        }
    }
    else if (myTank.x < 6)
    {
        if (myTank.direction != "right")
        {
            command = RotationCommand(myTank.direction, "right", log);
        }
        else
        {
            command = HandleWall(walls, myTank);
        }
    }
    else if (myTank.y > 6)
    {
        if (myTank.direction != "top")
        {
            command = RotationCommand(myTank.direction, "top", log);
        }
        else
        {
            command = HandleWall(walls, myTank);
        }
    }
    else if (myTank.y < 6)
    {
        if (myTank.direction != "bottom")
        {
            command = RotationCommand(myTank.direction, "bottom", log);
        }
        else
        {
            command = HandleWall(walls, myTank);
        }
    }

    return command;
}

public static string HandleWall(Wall[] walls, Tank myTank)
{
    bool wallIsInFront = false;

    foreach (var wall in walls)
    {
        if (myTank.x - 1 == wall.x && myTank.y == wall.y && myTank.direction == "left")
        {
            wallIsInFront = true;
            break;
        }
        else if (myTank.y - 1 == wall.y && myTank.x == wall.x && myTank.direction == "top")
        {
            wallIsInFront = true;
            break;
        }
        else if (myTank.x + 1 == wall.x && myTank.y == wall.y && myTank.direction == "right")
        {
            wallIsInFront = true;
            break;
        }
        else if (myTank.y + 1 == wall.y && myTank.x == wall.x && myTank.direction == "bottom")
        {
            wallIsInFront = true;
            break;
        }
    }

    if (wallIsInFront)
    {
        return "fire";
    }
    else
    {
        return "forward";
    }
}

public static string RotationCommand(string direction, string targetDirection, TraceWriter log)
{
    var command = "pass";

    if (targetDirection == "right")
    {
        switch (direction)
        {
            case "top":
                command = "turn-right";
                break;
            case "bottom":
                command = "turn-left";
                break;
            case "left":
                command = "turn-left";
                break;
            case "right":
            default:
                log.Error($"Invalid direction {direction}");
                break;
        }
    }
    else if (targetDirection == "left")
    {
        switch (direction)
        {
            case "top":
                command = "turn-left";
                break;
            case "bottom":
                command = "turn-right";
                break;
            case "right":
                command = "turn-left";
                break;
            case "left":
            default:
                log.Error($"Invalid direction {direction}");
                break;
        }
    }
    else if (targetDirection == "top")
    {
        switch (direction)
        {
            case "left":
                command = "turn-right";
                break;
            case "bottom":
                command = "turn-left";
                break;
            case "right":
                command = "turn-left";
                break;
            case "top":
            default:
                log.Error($"Invalid direction {direction}");
                break;
        }
    }
    else if (targetDirection == "bottom")
    {
        switch (direction)
        {
            case "top":
                command = "turn-left";
                break;
            case "left":
                command = "turn-left";
                break;
            case "right":
                command = "turn-right";
                break;
            case "bottom":
            default:
                log.Error($"Invalid direction {direction}");
                break;
        }
    }
    else
    {
        log.Error($"Invalid targetDirection {targetDirection}");
    }

    return command;
}