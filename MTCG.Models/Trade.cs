namespace MTCG.Models
{
    public class Trade : DataObject
    {
        public Guid UserOneID { get; }
        public Guid UserTwoID { get; }
        public Guid CardOneID { get; }
        public Guid CardTwoID { get; }

        public Trade(Guid id, User u1, CardInstance c1, User u2, CardInstance c2) : base(id)
        {
            UserOneID = u1.ID;
            UserTwoID = u2.ID;
            CardOneID = c1.ID;
            CardTwoID = c2.ID;
        }
    }
}
