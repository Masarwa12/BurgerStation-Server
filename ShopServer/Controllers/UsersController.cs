using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ShopServer.Models;

namespace ShopServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Post([FromBody] UserDTO userdto)
        {
            try
            {
                if (userdto == null)
                    return BadRequest();
                User user = DBServices.Login(userdto);
                if (user != null)
                {
                    return Ok(user);
                }
                else return NoContent();

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Register([FromBody] User user)
        {
            Console.WriteLine(user.ToString());
            try
            {
                if (user == null)
                    return BadRequest("user data is null");

                bool success = DBServices.Register(user);
                Console.WriteLine(success);
                if (success)
                {
                    Console.WriteLine("User registered successfully");
                    return StatusCode(StatusCodes.Status201Created, "User registered successfully");

                }
                else
                {
                    Console.WriteLine("Registration failed - user might already exist");
                    return BadRequest("Registration failed - user might already exist");
                }

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpPost("Item/Add")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult AddItem([FromBody] Item newItem)
        {
            Console.WriteLine(newItem.ToString());
            try
            {
                if (newItem == null)
                    return BadRequest("Item data is null");

                bool success = DBServices.AddItem(newItem);
                Console.WriteLine(success);
                if (success)
                {
                    Console.WriteLine("Item Add successfully");
                    return StatusCode(StatusCodes.Status201Created, "Item Add successfully");

                }
                else
                {
                    Console.WriteLine("Registration failed - user might already exist");
                    return BadRequest("Item failed - Item might already exist");
                }

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet("items")]
        public IActionResult GetItemsPaged(int page = 1, int pageSize = 10)
        {
            try
            {
                var items = DBServices.ShowItemsPaged(page, pageSize);
                return Ok(items);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpGet("categories")]
        public IActionResult GetCategories()
        {
            List<string> categories = new List<string>();

            using (SqlConnection con = new SqlConnection(DBServices.conStr))
            {
                string query = "SELECT Name FROM Categories";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    categories.Add(reader["Name"].ToString());
                }
            }

            return Ok(categories);
        }

        [HttpPost]
        [Route("api/Users/purchase")]
        public IActionResult AddPurchases([FromBody] List<Purchase> purchases)
        {
            try
            {
                bool result = DBServices.AddPurchases(purchases);
                if (result)
                    return Ok("Purchase saved");
                return BadRequest("Failed to save purchase");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("api/Users/purchase/history/{email}")]
        public IActionResult GetPurchaseHistory(string email)
        {
            try
            {
                var history = DBServices.GetPurchases(email);
                return Ok(history);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



    }
}

