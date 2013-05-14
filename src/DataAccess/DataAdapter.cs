using System.Linq;

namespace DataAccess
{
    public class DataAdapter
    {
        public bool SaveClient(Client client)
        {
            using (var db = new DataContext())
            {
                db.Client.Add(client);
                return db.SaveChanges() > 0;
            }
        }

        public Client FindClient(int id)
        {
            using (var db = new DataContext())
            {
                return db.Client.Find(id);
            }
        }

        public int GetPolicyCount()
        {
            using (var db = new DataContext())
            {
                return db.Policy.Count();
            }
        }

        public bool SavePolicy(Policy policy)
        {
            using (var db = new DataContext())
            {
                db.InsuredObject.Add(policy.InsuredObject);
                db.Policy.Add(policy);
                return db.SaveChanges() > 0;
            }
        }

        public Policy[] FindClientPolicies(int clientId)
        {
            using (var db = new DataContext())
            {
                var client = db.Client.Include("Policy").FirstOrDefault(c => c.id == clientId);
                return client != null ? client.Policy.ToArray() : new Policy[0];
            }
        }
    }
}
