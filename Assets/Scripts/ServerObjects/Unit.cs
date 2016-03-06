using System.Collections;

public class Unit 
{
    public string unitId { get; set; }
    public string unitType  { get; set; }
    public int men  { get;  set; }
    public string experience  { get; set; }

    public Unit (string unitId, string unitType, int men, string experience)
    {
        this.unitId = unitId;
        this.unitType = unitType;
        this.men = men;
        this.experience = experience;
    }

}

