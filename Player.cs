using Godot;
using System;

public partial class Player : Area2D
{
    [Signal]
    public delegate void Hit();

    [Export]
    public int Speed { get; set; } = 400; // How fast the player will move (pixels/sec).
    public Vector2 ScreenSize; // Size of the game window.

    public override void _Ready() {
        ScreenSize = GetViewportRect().Size;
        Hide();
    }

    // We also specified this function name in PascalCase in the editor's connection window.
    private void OnBodyEntered(Node2D body) {
        Hide(); // Player disappears after being hit.
        
        EmitSignal("Hit");
        // Must be deferred as we can't change physics properties on a physics callback.
        GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
    }

    public void Start(Vector2 position) {
        Position = position;
        Show();
        GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
    }

    public override void _Process(float delta) {
        var velocity = Vector2.Zero;

        if (Input.IsActionPressed("moveRight")) {
            velocity.x += 1;
        }
        if (Input.IsActionPressed("moveLeft")) {
            velocity.x -= 1;    
        }
        if (Input.IsActionPressed("moveDown")) {
            velocity.y += 1;
        }
        if (Input.IsActionPressed("moveUp")) {
            velocity.y -= 1;
        }

        var animatedSprite2D = GetNode<AnimatedSprite>("AnimatedSprite2D");
        if (velocity.Length() > 0) {
            velocity = velocity.Normalized() * Speed;
            animatedSprite2D.Play();
        } else {
            animatedSprite2D.Stop();
        }

        Position += velocity * (float)delta;
        Position = new Vector2(
            x: Mathf.Clamp(Position.x, 0, ScreenSize.x),
            y: Mathf.Clamp(Position.y, 0, ScreenSize.y)
        );

        if (velocity.x != 0) {
            animatedSprite2D.Animation = "walk";
            animatedSprite2D.FlipV = false;
            // See the note below about the following boolean assignment.
            animatedSprite2D.FlipH = velocity.x < 0;
        } else if (velocity.y != 0) {
            animatedSprite2D.Animation = "up";
            animatedSprite2D.FlipV = velocity.y > 0;
        }
    }

}
