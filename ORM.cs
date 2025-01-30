using ModKit.ORM;
using SQLite;

namespace HueATM98
{
    public class Orm_RetirerMoney : ModEntity<Orm_RetirerMoney>
    {
        [AutoIncrement]
        [PrimaryKey] public int Id { get; set; }
        public int Amount { get; set; }
        public string Day {  get; set; }
        public string Month { get; set; }
        public int Year { get; set; }
        public int PlayerCharacterId { get; set; }
    }
    public class Orm_DéposerMoney : ModEntity<Orm_DéposerMoney>
    {
        [AutoIncrement]
        [PrimaryKey] public int Id { get; set; }
        public int Amount { get; set; }
        public string Day { get; set; }
        public string Month { get; set; }
        public int Year { get; set; }
        public int PlayerCharacterId { get; set; }
    }
}
