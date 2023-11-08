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
        foreach(Projectile projectile in level.Player.Gun.ActiveProjectiles)
        {
            if(HasPixelPreciseCollision(projectile))
            {
                Die();
            }
        }
    }

    void Die()
    {
        IsAlive = false;
    }
}
