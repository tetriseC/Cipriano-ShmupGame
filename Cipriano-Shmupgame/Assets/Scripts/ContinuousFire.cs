/************************************************************
- COPYRIGHT:  2026
- PROJECT:    Cipriano-ShmupGame
- FILE NAME:  ContinuousFire.cs
- DESCRIPTION: Firing piece: always wants to fire, so the Shooter's own
              cooldown sets the rhythm.
*
- REVISION HISTORY:
- Date [YYYY/MM/DD] | Author | Comments
- ------------------------------------------------------------
- 2026/11/13 | David Cipriano | Created Script

************************************************************/

public class ContinuousFire : FiringBehavior
{
    public override bool WantsToFire(float deltaTime)
    {
        return true;
    }
}
