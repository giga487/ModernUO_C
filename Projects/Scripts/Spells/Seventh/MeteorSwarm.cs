using System.Collections.Generic;
using System.Linq;
using Server.Targeting;

namespace Server.Spells.Seventh
{
  public class MeteorSwarmSpell : MagerySpell, ISpellTargetingPoint3D
  {
    private static SpellInfo m_Info = new SpellInfo(
      "Meteor Swarm", "Flam Kal Des Ylem",
      233,
      9042,
      false,
      Reagent.Bloodmoss,
      Reagent.MandrakeRoot,
      Reagent.SulfurousAsh,
      Reagent.SpidersSilk
    );

    public MeteorSwarmSpell(Mobile caster, Item scroll = null) : base(caster, scroll, m_Info)
    {
    }

    public override SpellCircle Circle => SpellCircle.Seventh;

    public override bool DelayedDamage => true;

    public override void OnCast()
    {
      Caster.Target = new SpellTargetPoint3D(this, TargetFlags.None, Core.ML ? 10 : 12);
    }

    public void Target(IPoint3D p)
    {
      if (!Caster.CanSee(p))
      {
        Caster.SendLocalizedMessage(500237); // Target can not be seen.
      }
      else if (SpellHelper.CheckTown(p, Caster) && CheckSequence())
      {
        SpellHelper.Turn(Caster, p);

        if (p is Item item)
          p = item.GetWorldLocation();

        List<Mobile> targets;

        Map map = Caster.Map;

        bool playerVsPlayer = false;

        if (map != null)
        {
          IPooledEnumerable<Mobile> eable = map.GetMobilesInRange(new Point3D(p), 2);

          targets = eable.Where(m =>
          {
            if (Caster == m || !SpellHelper.ValidIndirectTarget(Caster, m) || !Caster.CanBeHarmful(m, false) ||
                Core.AOS && !Caster.InLOS(m))
              return false;

            if (m.Player)
              playerVsPlayer = true;

            return true;
          }).ToList();

          eable.Free();
        }
        else
        {
          targets = new List<Mobile>();
        }

        double damage;

        damage = Core.AOS ? GetNewAosDamage(51, 1, 5, playerVsPlayer)
          : Utility.Random(27, 22);

        if (targets.Count > 0)
        {
          Effects.PlaySound(p, Caster.Map, 0x160);

          if (Core.AOS && targets.Count > 2)
            damage = damage * 2 / targets.Count;
          else if (!Core.AOS)
            damage /= targets.Count;

          for (int i = 0; i < targets.Count; ++i)
          {
            Mobile m = targets[i];

            double toDeal = damage;

            if (!Core.AOS && CheckResisted(m))
            {
              damage *= 0.5;

              m.SendLocalizedMessage(501783); // You feel yourself resisting magical energy.
            }

            toDeal *= GetDamageScalar(m);
            Caster.DoHarmful(m);
            SpellHelper.Damage(this, m, toDeal, 0, 100, 0, 0, 0);

            Caster.MovingParticles(m, 0x36D4, 7, 0, false, true, 9501, 1, 0, 0x100);
          }
        }
      }

      FinishSequence();
    }
  }
}
