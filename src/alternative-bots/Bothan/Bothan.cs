using System;
using System.Drawing;
using Robocode.TankRoyale.BotApi;
using Robocode.TankRoyale.BotApi.Events;

public class Bothan : Bot
{
  static void Main(string[] args)
  {
    new Bothan().Start();
  }

  // Targeting variables
  private bool isLocking, isEnemyScanned;
  private double lastEnemyX, lastEnemyY, enemyDirection;
  private int targetId = -1;
  private int lastEnemySeenTurn = -1;
  private int targetLostThreshold = 50;


  // Movement variables
  private int wallMargin = 100;

  Bothan() : base(BotInfo.FromFile("Bothan.json")) { }

  private void SetColors()
  {
    BodyColor = Color.FromArgb(0x00, 0x00, 0x00);
    TurretColor = Color.FromArgb(0xFF, 0x00, 0x00);
    RadarColor = Color.FromArgb(0x00, 0xFF, 0xFF);
    BulletColor = Color.FromArgb(0xFF, 0xFF, 0x00);
    ScanColor = Color.FromArgb(0x00, 0xFF, 0x00);
    TracksColor = Color.FromArgb(0xFF, 0x80, 0x00);
    GunColor = Color.FromArgb(0xFF, 0x00, 0xFF);
  }

  public override void Run()
  {
    // Initial configuration
    SetColors();
    AdjustRadarForGunTurn = true;
    AdjustRadarForBodyTurn = true;
    AdjustGunForBodyTurn = true;

    // Initial boolean value
    isLocking = false;
    isEnemyScanned = false;

    // Initial movement
    TargetSpeed = 5;

    // MAIN
    while (IsRunning)
    {
      if (IsTooCloseToWall())
      {
        AvoidWall();
      }

      if (isLocking && (TurnNumber - lastEnemySeenTurn > targetLostThreshold))
      {
        Console.WriteLine("Target lost - assuming dead or out of range");
        ResetTargeting();
      }

      if (isLocking && isEnemyScanned)
      {
        Console.WriteLine("Locking: " + targetId);
        var radarTurn = RadarBearingTo(lastEnemyX, lastEnemyY);
        SetTurnRadarLeft(radarTurn);

        SetTurnLeft(BearingTo(lastEnemyX, lastEnemyY));
        SetTurnGunLeft(GunBearingTo(lastEnemyX, lastEnemyY));
        Fire(2);

        isEnemyScanned = false;
      }
      else
      {
        Console.WriteLine("Not Locking");
        SetTurnRadarRight(20);
      }

      Go();
    }
  }

  private bool IsTooCloseToWall()
  {
    // Distance to wall
    double distanceToEast = ArenaWidth - X;
    double distanceToWest = X;
    double distanceToNorth = ArenaHeight - Y;
    double distanceToSouth = Y;

    return distanceToNorth < wallMargin || distanceToSouth < wallMargin ||
            distanceToEast < wallMargin || distanceToWest < wallMargin;
  }

  private void AvoidWall()
  {
    double centerX = ArenaWidth / 2;
    double centerY = ArenaWidth / 2;

    // Turn towards center
    double angleToCenter = BearingTo(centerX, centerY);
    SetTurnLeft(angleToCenter);
  }


  public override void OnHitBot(HitBotEvent e)
  {
    // Get enemy potition
    double hitBotX = e.X;
    double hitBotY = e.Y;
    double bearingToEnemyBot = BearingTo(hitBotX, hitBotY);

    if (e.IsRammed)
    {
      SetTurnLeft(NormalizeRelativeAngle(bearingToEnemyBot + 90));
    }
    else
    {
      SetTurnLeft(NormalizeRelativeAngle(bearingToEnemyBot + 180));
    }
  }

  // On Scanned Function
  public override void OnScannedBot(ScannedBotEvent e)
  {
    if (!isLocking || e.ScannedBotId == targetId)
    {
      isLocking = true;
      isEnemyScanned = true;
      targetId = e.ScannedBotId;

      lastEnemySeenTurn = TurnNumber;
      lastEnemyX = e.X;
      lastEnemyY = e.Y;

      enemyDirection = e.Direction;
    }
  }

  // Refresh Targeting
  public override void OnBotDeath(BotDeathEvent e)
  {
    if (e.VictimId == targetId)
    {
      ResetTargeting();
    }
  }

  private void ResetTargeting()
  {
    isEnemyScanned = false;
    isLocking = false;
    targetId = -1;
  }
}

//test