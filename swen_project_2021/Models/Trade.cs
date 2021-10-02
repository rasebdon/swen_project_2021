using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Models
{
    public class Trade : DataObject
    {
        public Guid UserOneID { get; }
        public Guid UserTwoID { get; }
        public Guid CardOneID { get; }
        public Guid CardTwoID { get; }

        public Trade(User u1, CardInstance c1, User u2, CardInstance c2) : base()
        {
            UserOneID = u1.ID;
            UserTwoID = u2.ID;
            CardOneID = c1.ID;
            CardTwoID = c2.ID;
        }

        public Trade(OrderedDictionary row) : base(row)
        {
            UserOneID = (Guid)row["user_one_id"];
            UserTwoID = (Guid)row["user_tow_id"];
            CardOneID = (Guid)row["card_one_id"];
            CardTwoID = (Guid)row["card_two_id"];
        }
    }
}
