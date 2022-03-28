using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Attributes;

namespace StartFinance.Models
{
    public class ContactDetails
    {
        [PrimaryKey, AutoIncrement]
        public int ContactID { get; set; }

        [NotNull]
        public string FirstName { get; set; }

        [NotNull]
        public string LastName { get; set; }

        [NotNull]
        public string CompanyName { get; set; }

        [NotNull]
        public string MobilePhone { get; set; }
    }
}
