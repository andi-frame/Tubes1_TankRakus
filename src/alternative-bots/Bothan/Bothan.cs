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
    SetColors();
    AdjustRadarForGunTurn = true;
    AdjustRadarForBodyTurn = true;
    AdjustGunForBodyTurn = true;

    isLocking = false;
    isEnemyScanned = false;

    // Initial movement
    TargetSpeed = 8 * moveDirection;

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

  private void ExecuteMovement()
  {
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
      if (Random(10) == 0)
      {
        SetTurnRight(Random(90) - 45);
      }
    }
  }

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
      lastEnemyX = e.X;
      lastEnemyY = e.Y;
      enemyDirection = BearingTo(e.X, e.Y);
      lastEnemyEnergy = e.Energy;
    }
  }

  private void FirePredict()
  {
    double distance = DistanceTo(lastEnemyX, lastEnemyY);
    if (distance < 5)
    {
      Fire(10);
      return;
    }

    double firePower = Math.Min(3, Math.Max(1, 400 / distance));
    double bulletSpeed = CalcBulletSpeed(firePower);

    double timeToHit = distance / bulletSpeed;
    double directionRad = enemyDirection * Math.PI / 180;

    double predictedX = lastEnemyX + enemySpeed * Math.Cos(directionRad) * timeToHit;
    double predictedY = lastEnemyY + enemySpeed * Math.Sin(directionRad) * timeToHit;

    double gunTurn = GunBearingTo(predictedX, predictedY);
    SetTurnGunLeft(gunTurn);

    if (Math.Abs(gunTurn) < 10)
    {
      Fire(firePower);
    }
  }

  public override void OnHitWall(HitWallEvent e)
  {
    TargetSpeed = -TargetSpeed;
    SetTurnRight(90);
  }

  public override void OnHitByBullet(HitByBulletEvent e)
  {
    TargetSpeed = -TargetSpeed;

    SetTurnRight(90 - NormalizeRelativeAngle(e.Bullet.Direction - Direction));
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