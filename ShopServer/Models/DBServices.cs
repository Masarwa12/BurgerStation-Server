
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ShopServer.Models
{
    public class DBServices
    {
       //public static string conStr = @"Data Source=DESKTOP-1I0SRL4\SQLEXPRESS;Initial Catalog=DBUsers;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";
        public static readonly string conStr = "workstation id=DBUsers.mssql.somee.com;packet size=4096;user id=masarwa_SQLLogin_1;pwd=32f4rmy6e9;data source=DBUsers.mssql.somee.com;persist security info=False;initial catalog=DBUsers;TrustServerCertificate=True";
        private static User ExcQUser(string command)
        {
            User user = null;
            SqlConnection con = new SqlConnection(conStr);
            SqlCommand comm = new SqlCommand(command, con);

            comm.Connection.Open();
            SqlDataReader reader = comm.ExecuteReader();
            if (reader.Read())
            {

                user = new User()
                {
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"],
                    Email = (string)reader["Email"],
                    Password = (string)reader["Password"],
                    isAdmin = (bool)reader["isAdmin"]
                };
            }
            comm.Connection.Close();

            return user; 
        }
        internal static User Login(UserDTO userdto)
        {
            return ExcQUser(
                $"SELECT * " +
                $"FROM TBUsers " +
                $"Where Email= '{userdto.Email}' and Password= '{userdto.Password}'");
        }
        internal static bool Register(User user)
        {
            using (SqlConnection con = new SqlConnection(conStr))
            {
                // בדיקה אם המשתמש כבר קיים לפי אימייל
                string checkQuery = "SELECT COUNT(*) FROM TBUsers WHERE Email = @Email";
                SqlCommand checkCmd = new SqlCommand(checkQuery, con);
                checkCmd.Parameters.AddWithValue("@Email", user.Email);

                con.Open();
                int count = (int)checkCmd.ExecuteScalar();
                if (count > 0)
                {
                    // המשתמש כבר קיים
                    return false;
                }

                // הוספת המשתמש
                string insertQuery = @"
            INSERT INTO TBUsers (FirstName, LastName, Email, Password, isAdmin)
            VALUES (@FirstName, @LastName, @Email, @Password, @IsAdmin)";
                SqlCommand insertCmd = new SqlCommand(insertQuery, con);

                insertCmd.Parameters.AddWithValue("@FirstName", user.FirstName);
                insertCmd.Parameters.AddWithValue("@LastName", user.LastName);
                insertCmd.Parameters.AddWithValue("@Email", user.Email);
                insertCmd.Parameters.AddWithValue("@Password", user.Password);
                insertCmd.Parameters.AddWithValue("@IsAdmin", user.isAdmin);

                int rowsAffected = insertCmd.ExecuteNonQuery();
                Console.WriteLine(rowsAffected + "DB");
                return rowsAffected > 0;
            }
        }

        internal static bool AddItem(Item newItem)
        {          
              using (SqlConnection con = new SqlConnection(conStr))
                {
                // בדיקה אם המשתמש כבר קיים לפי אימייל
                string checkQuery = "SELECT COUNT(*) FROM Items WHERE Name = @Name";
                SqlCommand checkCmd = new SqlCommand(checkQuery, con);
                checkCmd.Parameters.AddWithValue("@Name", newItem.Name);

                con.Open();
                int count = (int)checkCmd.ExecuteScalar();
                if (count > 0)
                {
                    // המוצר כבר קיים
                    return false;
                }
                    string query = "INSERT INTO Items (Name,Price , Category , Image , Description) VALUES (@Name, @Price , @Category , @Image,@Description)";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Name", newItem.Name);
                    cmd.Parameters.AddWithValue("@Price", newItem.Price);
                    cmd.Parameters.AddWithValue("@Category", newItem.Category);
                    cmd.Parameters.AddWithValue("@Image", newItem.Image);
                    cmd.Parameters.AddWithValue("@Description", newItem.Description);
                int rowsAffected = cmd.ExecuteNonQuery();
                Console.WriteLine(rowsAffected + "DB");
                return rowsAffected > 0;
              }
              
            }

        public static List<Item> ShowItemsPaged(int page, int pageSize)
        {
            List<Item> items = new List<Item>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                // חישוב ההיסט של שורות שיש לדלג עליהן
                int offset = (page - 1) * pageSize;

                string query = @"
            SELECT * FROM Items
            ORDER BY Name
            OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY";

                SqlCommand comm = new SqlCommand(query, con);
                comm.Parameters.AddWithValue("@Offset", offset);
                comm.Parameters.AddWithValue("@PageSize", pageSize);

                con.Open();
                SqlDataReader reader = comm.ExecuteReader();

                while (reader.Read())
                {
                    Item item = new Item()
                    {
                        Name = (string)reader["Name"],
                        Price = (int)reader["Price"],
                        Category = (string)reader["Category"],
                        Image = (string)reader["Image"],
                        Description = reader["Description"] != DBNull.Value ? (string)reader["Description"] : ""
                    };
                    items.Add(item);
                }
                con.Close();
            }

            return items;
        }
        public static bool AddPurchases(List<Purchase> purchases)
        {
            using (SqlConnection con = new SqlConnection(conStr))
            {
                con.Open();
                foreach (var p in purchases)
                {
                    string query = "INSERT INTO Purchases (UserEmail, ItemName, Price, Amount, PurchaseDate) VALUES (@UserEmail, @ItemName, @Price, @Amount, @Date)";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@UserEmail", p.UserEmail);
                    cmd.Parameters.AddWithValue("@ItemName", p.ItemName);
                    cmd.Parameters.AddWithValue("@Price", p.Price);
                    cmd.Parameters.AddWithValue("@Amount", p.Amount);
                    cmd.Parameters.AddWithValue("@Date", p.PurchaseDate);
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
        }
        public static List<Purchase> GetPurchases(string userEmail)
        {
            List<Purchase> purchases = new List<Purchase>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                string query = "SELECT * FROM Purchases WHERE UserEmail = @UserEmail ORDER BY PurchaseDate DESC";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@UserEmail", userEmail);
                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    purchases.Add(new Purchase
                    {
                        UserEmail = reader["UserEmail"].ToString(),
                        ItemName = reader["ItemName"].ToString(),
                        Price = Convert.ToInt32(reader["Price"]),
                        Amount = Convert.ToInt32(reader["Amount"]),
                        PurchaseDate = Convert.ToDateTime(reader["PurchaseDate"])
                    });
                }
            }

            return purchases;
        }



    }
}
