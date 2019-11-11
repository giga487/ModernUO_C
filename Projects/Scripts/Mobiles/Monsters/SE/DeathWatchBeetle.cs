using Server.Engines.Plants;
using Server.Items;

namespace Server.Mobiles
{
  [TypeAlias("Server.Mobiles.DeathWatchBeetle")]
  public class DeathwatchBeetle : BaseCreature
  {
    [Constructible]
    public DeathwatchBeetle() : base(AIType.AI_Melee, Core.ML ? FightMode.Aggressor : FightMode.Closest, 10, 1, 0.2, 0.4)
    {
      Body = 242;

      SetStr(136, 160);
      SetDex(41, 52);
      SetInt(31, 40);

      SetHits(121, 145);
      SetMana(20);

      SetDamage(5, 10);

      SetDamageType(ResistanceType.Physical, 100);

      SetResistance(ResistanceType.Physical, 35, 40);
      SetResistance(ResistanceType.Fire, 15, 30);
      SetResistance(ResistanceType.Cold, 15, 30);
      SetResistance(ResistanceType.Poison, 50, 80);
      SetResistance(ResistanceType.Energy, 20, 35);

      SetSkill(SkillName.MagicResist, 50.1, 58.0);
      SetSkill(SkillName.Tactics, 67.1, 77.0);
      SetSkill(SkillName.Wrestling, 50.1, 60.0);
      SetSkill(SkillName.Anatomy, 30.1, 34.0);

      Fame = 1400;
      Karma = -1400;

      switch (Utility.Random(12))
      {
        case 0:
          PackItem(new LeatherGorget());
          break;
        case 1:
          PackItem(new LeatherGloves());
          break;
        case 2:
          PackItem(new LeatherArms());
          break;
        case 3:
          PackItem(new LeatherLegs());
          break;
        case 4:
          PackItem(new LeatherCap());
          break;
        case 5:
          PackItem(new LeatherChest());
          break;
      }

      if (Utility.RandomDouble() < .5)
        PackItem(Seed.RandomBonsaiSeed());

      Tamable = true;
      MinTameSkill = 41.1;
      ControlSlots = 1;
    }

    public DeathwatchBeetle(Serial serial) : base(serial)
    {
    }

    public override string CorpseName => "a deathwatchbeetle corpse";

    public override string DefaultName => "a deathwatch beetle";

    public override int Hides => 8;

    public override WeaponAbility GetWeaponAbility() => WeaponAbility.CrushingBlow;

    public override int GetAngerSound() => 0x4F3;

    public override int GetIdleSound() => 0x4F2;

    public override int GetAttackSound() => 0x4F1;

    public override int GetHurtSound() => 0x4F4;

    public override int GetDeathSound() => 0x4F0;

    public override void GenerateLoot()
    {
      AddLoot(LootPack.LowScrolls, 1);
      AddLoot(LootPack.Potions, 1);
    }

    public override void AlterMeleeDamageTo(Mobile to, ref int damage)
    {
      if (Utility.RandomBool() && Mana > 14 && to != null)
      {
        damage = damage + damage / 2;
        to.SendLocalizedMessage(1060091); // You take extra damage from the crushing attack!
        to.PlaySound(0x1E1);
        to.FixedParticles(0x377A, 1, 32, 0x26da, 0, 0, 0);
        Mana -= 15;
      }
    }

    public override void OnDamage(int amount, Mobile from, bool willKill)
    {
      Mobile combatant = Combatant;

      if (combatant?.Deleted != false || combatant.Map != Map || !InRange(combatant, 12) ||
          !CanBeHarmful(combatant) || !InLOS(combatant))
        return;

      if (Utility.Random(10) == 0)
        PoisonAttack(combatant);

      base.OnDamage(amount, from, willKill);
    }

    public void PoisonAttack(Mobile m)
    {
      DoHarmful(m);
      MovingParticles(m, 0x36D4, 1, 0, false, false, 0x3F, 0, 0x1F73, 1, 0, (EffectLayer)255, 0x100);
      m.ApplyPoison(this, Poison.Regular);
      m.SendLocalizedMessage(1070821, Name); // %s spits a poisonous substance at you!
    }

    public override void Serialize(GenericWriter writer)
    {
      base.Serialize(writer);
      writer.Write(0);
    }

    public override void Deserialize(GenericReader reader)
    {
      base.Deserialize(reader);
      int version = reader.ReadInt();
    }
  }
}
