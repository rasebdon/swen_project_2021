namespace MTCG.Models
{
    public class Trade : DataObject
    {
        public Guid UserOneID { get; }
        public Guid UserTwoID { get; }
        public Guid CardOneID { get; }
        public Guid CardTwoID { get; }

        public Trade(Guid id, Guid u1, Guid c1, Guid u2, Guid c2) : base(id)
        {
            UserOneID = u1;
            UserTwoID = u2;
            CardOneID = c1;
            CardTwoID = c2;
        }
    }
}
