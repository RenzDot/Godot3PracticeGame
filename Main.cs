using Godot;
using System;
using System.Numerics;

public class Main : Node
{
    // Don't forget to rebuild the project so the editor knows about the new export variable.
    // We assign this in the editor, so we don't need the warning about not being assigned.
    [Export]
    public PackedScene MobScene;

    public int Score;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Randomize();
    }

    public void GameOver() {
        GetNode<Timer>("MobTimer").Stop();
        GetNode<Timer>("ScoreTimer").Stop();
        GetNode<HUD>("Hud").ShowGameOver();
        GetNode<Timer>("StartTimer").Stop();
        GetNode<AudioStreamPlayer>("DeathSound").Play();

        GetTree().CallGroup("mobs", "queue_free");
    }

    public void NewGame() {
        Score = 0;

        GetNode<AudioStreamPlayer>("MyMusic").Play();

        var player = GetNode<Player>("Player");
        var startPosition = GetNode<Position2D>("StartPosition");
        player.Start(startPosition.Position);

        GetNode<Timer>("StartTimer").Start();

        var hud = GetNode<HUD>("Hud");
        hud.UpdateScore(Score);
        hud.ShowMessage("Get Ready!");

    }

    public void OnScoreTimerTimeout() {
        Score++;
        GetNode<HUD>("Hud").UpdateScore(Score);
    }

    public void OnStartTimerTimeout() {
        GetNode<Timer>("MobTimer").Start();
        GetNode<Timer>("ScoreTimer").Start();
    }

    public void OnMobTimerTimeout() {
        // Note: Normally it is best to use explicit types rather than the `var`
        // keyword. However, var is acceptable to use here because the types are
        // obviously Mob and PathFollow2D, since they appear later on the line.

        // Create a new instance of the Mob scene.
        var mob = (RigidBody2D)MobScene.Instance();

        // Choose a random location on Path2D.
        var mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");
        mobSpawnLocation.Offset = GD.Randi();

        // Set the mob's direction perpendicular to the path direction.
        float direction = mobSpawnLocation.Rotation + Mathf.Pi / 2;

        // Set the mob's position to a random location.
        mob.Position = mobSpawnLocation.Position;

        // Add some randomness to the direction.
        direction += (float)GD.RandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
        mob.Rotation = direction;

        // Choose the velocity.
        var velocity = new Godot.Vector2((float)GD.RandRange(150.0, 250.0), 0);
        mob.LinearVelocity = velocity.Rotated(direction);

        // Spawn the mob by adding it to the Main scene.
        AddChild(mob);
    }
}
