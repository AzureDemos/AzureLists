using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureLists.TableStorage
{
    /// <summary>
    /// This is for demo purposes only as we dont have authenication in this example
    /// </summary>
    public class UserCredentials
    {
        /// <summary>
        /// We can use the ID to form part of a composite row key
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// Once we have authenication, we could use something from the users identity as the partition key 
        /// e.g. first letter of surname, country, or some form of hash to give an even distribution 
        /// </summary>
        public string PartionKey { get; set; }


        public static UserCredentials GetFirstUserFirstPartion()
        {
            return new UserCredentials() { UserID = "123", PartionKey = "P1" };
        }

        public static UserCredentials GetSecondtUserFirstPartion()
        {
            return new UserCredentials() { UserID = "456", PartionKey = "P1" };
        }

        public static UserCredentials GetFirstUserSecondPartion()
        {
            return new UserCredentials() { UserID = "789", PartionKey = "P2" };
        }
    }
}
