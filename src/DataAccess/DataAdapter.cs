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
    }
}
