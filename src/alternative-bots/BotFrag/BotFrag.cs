using System;
using System.Drawing;
using Robocode.TankRoyale.BotApi;
using Robocode.TankRoyale.BotApi.Events;

public class BotFrag : Bot
{
    int enemies; 
    double moveAmount; 
    int turnDirection = 1;
    bool lowEnergyMode = false;

    double lastEnemyX = -1;
    double lastEnemyY = -1;
    int stationaryEnemyCount = 0;
    const int maxStationaryTurns = 100;

    static void Main()
    {
        new BotFrag().Start();
    }

    BotFrag() : base(BotInfo.FromFile("BotFrag.json")) { }

    public override void Run()
    {
        BodyColor = Color.FromArgb(0x00, 0x00, 0x00);
        TurretColor = Color.FromArgb(0xFF, 0x69, 0xB4);
        RadarColor = Color.FromArgb(0x00, 0x64, 0x64);
        BulletColor = Color.FromArgb(0xFF, 0x00, 0x00);
        ScanColor = Color.FromArgb(0xFF, 0xC8, 0xC8);
        TracksColor = Color.FromArgb(0xFF, 0x69, 0xB4);


        moveAmount = Math.Max(ArenaWidth, ArenaHeight);
        TurnRight(Direction % 90);
        Forward(moveAmount);

        enemies = EnemyCount;
 
        TurnGunRight(90);

        while (IsRunning)
        {
            if (Energy < 20) lowEnergyMode = true;

            if (Direction % 90 != 0)
            {
                if(turnDirection == -1)
                    TurnLeft(Direction % 90);
                else
                    TurnRight(Direction % 90);
            }
            else
            {
                if(turnDirection == -1)
                    TurnLeft(90);
                else
                    TurnRight(90);
            }
            Forward(moveAmount);
        }
    }

    public override void OnScannedBot(ScannedBotEvent e)
    {
        // Deteksi musuh diam
        if (Math.Abs(e.X - lastEnemyX) < 1 && Math.Abs(e.Y - lastEnemyY) < 1)
        {
            stationaryEnemyCount++;
        }
        else
        {
            stationaryEnemyCount = 0;
        }

        lastEnemyX = e.X;
        lastEnemyY = e.Y;

        if (stationaryEnemyCount >= maxStationaryTurns)
        {
            Rescan();
            return;
        }

        double distance = DistanceTo(e.X, e.Y);
        if (distance > 400)
        {
            Rescan();
            return;
        }

        var bearingFromGun = GunBearingTo(e.X, e.Y);
        TurnGunLeft(bearingFromGun); 

        double power = (distance < 100) ? 3 : (distance < 300) ? 2 : 1;

        if (Math.Abs(bearingFromGun) <= 4)
        {
            if (GunHeat == 0 && Energy > 0.2)
            {
                double firePower = Math.Min(power, Math.Max(0.1, Energy - 0.1));
                Fire(firePower);

                // // Bergerak setelah menembak
                // TurnRight(30);
                // Forward(80);
            }
            else
            {
                return;
            }
        }

        if (Math.Abs(bearingFromGun) < 1)
        {
            Rescan();
        }

        if (!lowEnergyMode)
        {
            turnDirection *= -1;
        }
    }

    public override void OnHitBot(HitBotEvent e)
    {
        var bearing = GunBearingTo(e.X, e.Y);
        TurnGunLeft(bearing);
        turnDirection *= -1;

        if (bearing > -90 && bearing < 90)
        {
            Back(100);
            TurnLeft(90);
        }
        else
        {
            Forward(100);
            TurnLeft(90);
        }
        Rescan();
    }

    public override void OnHitByBullet(HitByBulletEvent e)
    {
        if (Energy < 20)
        {
            lowEnergyMode = true;
            if(turnDirection == 1)
            {
                TurnRight(90);
                Back(400);
            }
            else
            {
                TurnLeft(90);
                Back(400);
            }
        }
        else
        {
            if(turnDirection == 1)
            {
                TurnRight(90);
                Back(200);
            }
            else
            {
                TurnLeft(90);
                Back(200);
            }
        }

        TurnRight(30);
        Forward(100);
        TurnLeft(30);

        var bearingFromGun = GunBearingTo(e.Bullet.X, e.Bullet.Y);
        TurnGunLeft(bearingFromGun);
        Rescan();
        turnDirection *= -1;
    }
}