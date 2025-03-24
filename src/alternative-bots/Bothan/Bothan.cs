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
  private double lastEnemyEnergy;


  // Movement variables
  private int moveDirection = 1;
  private int wallMargin = 800;
  private int orbitDirection = 1;
  private int orbitSwitchCounter = 0;

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
    TargetSpeed = 8 * moveDirection;

    // MAIN
    while (IsRunning)
    {
      if (isLocking && isEnemyScanned)
      {
        Console.WriteLine("Locking: " + targetId);
        var radarTurn = RadarBearingTo(lastEnemyX, lastEnemyY);
        SetTurnRadarLeft(radarTurn);

        FirePredict();
        isEnemyScanned = false;
      }
      else
      {
        Console.WriteLine("Not Locking");
        SetTurnRadarRight(20);
      }

      ExecuteMovement();
      Go();
    }
  }


  // ==== To Maximize Energy for Shooting Only ====
  // Avoid unnecessary action that can reduce energy usage for shooting

  // Movement function
  private void ExecuteMovement()
  {
    if (IsTooCloseToWall())
    {
      AvoidWall();
      return;
    }

    if (isLocking)
    {
      double turnAngle = BearingTo(lastEnemyX, lastEnemyY);
      if (turnAngle >= 0 & turnAngle <= 180)
      {
        SetTurnRight(turnAngle);
      }
      else
      {
        SetTurnLeft(turnAngle);
      }
    }
    else
    {
      if (Random(10) == 1)
      {
        SetTurnRight(Random(90) - 45);
      }
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

  public override void OnHitWall(HitWallEvent e)
  {
    // TargetSpeed = -0;
  }

  public override void OnHitByBullet(HitByBulletEvent e)
  {
    // TargetSpeed = -TargetSpeed;
    // SetTurnRight(Direction + 180);
  }


  // ==== To Maximize Bullet Damage ====
  // Predict enemy movement, higher bullet hit chance
  // On Scanned Function
  public override void OnScannedBot(ScannedBotEvent e)
  {
    Console.WriteLine("On Scanned Bot");
    if (!isLocking || e.ScannedBotId == targetId)
    {
      isLocking = true;
      isEnemyScanned = true;
      targetId = e.ScannedBotId;

      lastEnemyX = e.X;
      lastEnemyY = e.Y;

      lastEnemyEnergy = e.Energy;
      enemyDirection = e.Direction;
    }
    else
    {
      targetId = e.ScannedBotId;
      lastEnemyX = e.X + 10;
      lastEnemyY = e.Y + 10;
      enemyDirection = BearingTo(e.X, e.Y);
      lastEnemyEnergy = e.Energy;
    }
  }

  // Predict Fire Bearing and Timing
  private void FirePredict()
  {
    double distance = DistanceTo(lastEnemyX, lastEnemyY);
    if (distance <= 0.1)
    {
      SetFire(10);
      return;
    }

    double firePower = Math.Min(3, Math.Max(1, 400 / distance));
    double bulletSpeed = CalcBulletSpeed(firePower);

    double timeToHit = distance / bulletSpeed;
    double directionRad = enemyDirection * Math.PI / 180;
    // double correction = Math.Atan(Math.Sqrt(4000 / (distance * distance)));
    // double correctionDegree = correction * 180 / Math.PI;

    double predictedX = lastEnemyX + enemySpeed * Math.Cos(directionRad) * timeToHit;
    double predictedY = lastEnemyY + enemySpeed * Math.Sin(directionRad) * timeToHit;

    double gunTurn = GunBearingTo(predictedX, predictedY);
    SetTurnGunLeft(gunTurn);
    // Console.WriteLine(correctionDegree);
    Console.WriteLine(GunDirection + " " + RadarDirection);


    SetFire(firePower);
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

  // Helper
  private int Random(int max)
  {
    return new Random().Next(max);
  }
}