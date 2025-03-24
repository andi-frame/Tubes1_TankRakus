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
  private double lastEnemyX, lastEnemyY, enemySpeed, enemyDirection;
  private int targetId = -1;


  // Movement variables
  private int moveDirection = 1;
  private int wallMargin = 100;

  Bothan() : base(BotInfo.FromFile("Bothan.json")) { }

  private void SetColors()
  {
    BodyColor = Color.FromArgb(0x1E, 0x1E, 0x2E);
    TurretColor = Color.FromArgb(0xFF, 0x45, 0x00);
    RadarColor = Color.FromArgb(0x00, 0xFF, 0xFF);
    BulletColor = Color.FromArgb(0xFF, 0xD7, 0x00);
    ScanColor = Color.FromArgb(0xAD, 0xFF, 0x2F);
    TracksColor = Color.FromArgb(0x80, 0x80, 0x80);
    GunColor = Color.FromArgb(0xDC, 0x14, 0x3C);
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
    TargetSpeed = 5 * moveDirection;

    // MAIN
    while (IsRunning)
    {
      if (IsTooCloseToWall())
      {
        AvoidWall();
      }

      if (isEnemyScanned)
      {
        Console.WriteLine("Locking: " + targetId);
        var radarTurn = RadarBearingTo(lastEnemyX, lastEnemyY);
        SetTurnRadarLeft(radarTurn);

        SetTurnLeft(BearingTo(lastEnemyX, lastEnemyY));

        FirePredict();
        isEnemyScanned = false;
      }
      else
      {
        Console.WriteLine("Not Locking");
        SetTurnRadarRight(20);
      }

      // ExecuteMovement();
      Go();
    }
  }


  // ==== To Maximize Energy for Shooting Only ====
  // Avoid unnecessary action that can reduce energy usage for shooting

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


  // ==== To Maximize Bullet Damage ====
  // Predict enemy movement, higher bullet hit chance
  // On Scanned Function
  public override void OnScannedBot(ScannedBotEvent e)
  {
    isEnemyScanned = true;
    targetId = e.ScannedBotId;
    lastEnemyX = e.X;
    lastEnemyY = e.Y;
    enemySpeed = e.Speed;

    enemyDirection = e.Direction;

    // if (!isLocking || e.ScannedBotId == targetId)
    // {
    //   isLocking = true;
    //   isEnemyScanned = true;
    //   targetId = e.ScannedBotId;

    //   lastEnemyX = e.X;
    //   lastEnemyY = e.Y;
    //   enemySpeed = e.Speed;

    //   enemyDirection = e.Direction;
    // }
    // else
    // {

    // }
  }

  // Predict Fire Bearing and Timing
  private void FirePredict()
  {
    double distance = DistanceTo(lastEnemyX, lastEnemyY);
    Console.WriteLine("Distance: " + distance);
    if (distance > 700)
    {
      return;
    }

    double firePower = 3;
    double bulletSpeed = CalcBulletSpeed(firePower);

    double timeToHit = distance / bulletSpeed;
    double directionRad = enemyDirection * Math.PI / 180;

    double predictedX = lastEnemyX + enemySpeed * Math.Cos(directionRad) * timeToHit;
    double predictedY = lastEnemyY + enemySpeed * Math.Sin(directionRad) * timeToHit;

    double gunTurn = GunBearingTo(predictedX, predictedY);
    SetTurnGunLeft(gunTurn);

    Fire(firePower);
  }

  public override void OnBotDeath(BotDeathEvent e)
  {
    if (e.VictimId == targetId)
    {
      ResetTargeting();
    }
  }

  private void ResetTargeting()
  {
    isLocking = false;
    targetId = -1;
  }
}