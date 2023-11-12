using Engine;
using Microsoft.Xna.Framework;

abstract internal class Enemy : AnimatedGameObject
{
    protected Level level;
    bool alive;
    protected bool IsAlive
    {
        get 
        { 
            return alive; 
        }
        set 
        { 
            alive = value;
            Visible = value;
        }
    }
    protected Enemy(Level level) : base(TickTick.Depth_LevelObjects) 
    {   
        this.level = level;
        IsAlive = true;
    }

    public override void Update(GameTime gameTime)
    {
        if (!IsAlive)
            return;

        base.Update(gameTime);

        //Check for collisions with bullets
        if (level.Player.Gun != null)
        {
            foreach (Projectile projectile in level.Player.Gun.ActiveProjectiles)
            {
                if (HasPixelPreciseCollision(projectile.BoundingBox))
                {
                    projectile.CollideWithEnemy(this);
                }
            }
        }
    }

    public void Die()
    {
        IsAlive = false;
        TickTick.AssetManager.PlaySoundEffect("Sounds/snd_enemy_death");
    }

    public override void Reset()
    {
        IsAlive = true;
    }
}
