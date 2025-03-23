using System;
using System.Drawing;
using Robocode.TankRoyale.BotApi;
using Robocode.TankRoyale.BotApi.Events;


// Kamikaze

// Bot akan menabrakan diri ke tank lawan jika energi kurang dari 70

public class Kamikaze : Bot
{
    
    static void Main(string[] args)
    {
        new Kamikaze().Start();
    }

    Kamikaze() : base(BotInfo.FromFile("Kamikaze.json")) {

    }

    public override void Run()
    {
        BodyColor = Color.FromArgb(255, 255, 255, 0);  
        TurretColor = Color.FromArgb(255, 0, 0, 255);  
        RadarColor = Color.FromArgb(255, 0, 255, 0);   
        ScanColor = Color.FromArgb(255, 0, 0, 0);    
        BulletColor = Color.FromArgb(255, 255, 255, 255);

        while (IsRunning)
        {
            TurnGunLeft(360);
        }
    }

    public override void OnScannedBot(ScannedBotEvent e)
    {
        if (Energy >= 70){
            TurnToFaceTarget(e.X, e.Y);
            TurnGunLeft(GunBearingTo(e.X, e.Y));
            SetFire(5);
            Rescan();
        } else {
            TurnToFaceTarget(e.X, e.Y);
            TurnGunLeft(BearingTo(e.X, e.Y));
            Forward(500);
            SetFire(5);
            Back(20);
        }
    }

    public override void OnHitBot(HitBotEvent e)
    {
        if (e.IsRammed)
        {
            TurnToFaceTarget(e.X, e.Y);
            SetFire(5);
            Back(30);
            Forward(20);
            SetFire(5);
        }
        else
        {
            TurnToFaceTarget(e.X, e.Y);
        }
    }

    private void TurnToFaceTarget(double x, double y)
    {
        var angleToEnemy = BearingTo(x, y);
        TurnLeft(angleToEnemy);
    }

    public override void OnWonRound(WonRoundEvent e)
    {
        for (int i = 0; i < 10; i++)
        {
        BodyColor = Color.FromArgb(255, 255, 0, 0);  
        TurretColor = Color.FromArgb(255, 255, 165, 0); 
        RadarColor = Color.FromArgb(255, 255, 255, 0);  
        ScanColor = Color.FromArgb(255, 0, 128, 0);  
        BulletColor = Color.FromArgb(255, 0, 0, 255); 
        TurnLeft(360);

        BodyColor = Color.FromArgb(255, 128, 0, 128);  
        TurretColor = Color.FromArgb(255, 0, 255, 255); 
        RadarColor = Color.FromArgb(255, 255, 0, 255); 
        ScanColor = Color.FromArgb(255, 255, 255, 255); 
        BulletColor = Color.FromArgb(255, 0, 0, 0); 
        TurnRight(360);
        }
    }
}