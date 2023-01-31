using System;

namespace Dark_Age_of_Valheim.Specalizations;


[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class SpecializationAttribute : Attribute
{
    protected byte _id;
    protected string _name;
    protected string _description;
    protected int _bonusHp;
    protected int _bonusStamina;
    protected int _bonusEitr;

    public SpecializationAttribute(byte id, string name, string description, int bonusHp, int bonusStamina, int bonusEitr) { 
        _id = id;
        _name = name;
        _description = description;
        _bonusHp = bonusHp;
        _bonusStamina = bonusStamina;
        _bonusEitr = bonusEitr;
    }

    public byte Id { get { return _id; } }
    public string Name { get { return _name; } }
    public string Description { get { return _description; } }
    public int BonusHp { get { return _bonusHp; } }
    public int BonusStamina { get { return _bonusStamina;  } }
    public int BonusEitr { get { return _bonusEitr; } }

}
