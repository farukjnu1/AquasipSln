using Aquasip.EF;
using Aquasip.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.Data;

namespace Aquasip.Repositories
{
    public class OrderRepository
    {
        private readonly string _connectionString = "";
        #region constructor
        public OrderRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        #endregion

        // Create
        public string? Add(OrderVM order)
        {
            string message = "operation failed.";
            
            using (var _context = new AquasipContext())
            {
                // Begin Transaction
                using var transaction = _context.Database.BeginTransaction();

                try
                {
                    var oCustomer = (from x in _context.Customers where x.Email == order.Customer.Email select x).FirstOrDefault();
                    if (oCustomer == null)
                    {
                        oCustomer = new Customer
                        {
                            Email = order.Customer.Email,
                            FullName = order.Customer.FullName,
                            PhoneNumber = order.Customer.PhoneNumber
                        };
                        _context.Customers.Add(oCustomer);
                        _context.SaveChanges();
                    }

                    var oShippingAddress = (from x in _context.ShippingAddresses
                                            where x.CustomerId == oCustomer.CustomerId
                                            && x.City == order.ShippingAddress.City
                                            && x.StateProvince == order.ShippingAddress.StateProvince
                                            && x.PostalCode == order.ShippingAddress.PostalCode
                                            && x.CountryCode == order.ShippingAddress.CountryCode
                                            select x).FirstOrDefault();
                    if (oShippingAddress == null)
                    {
                        oShippingAddress = new ShippingAddress
                        {
                            CustomerId = oCustomer.CustomerId,
                            City = order.ShippingAddress.City,
                            StateProvince = order.ShippingAddress.StateProvince,
                            PostalCode = order.ShippingAddress.PostalCode,
                            CountryCode = order.ShippingAddress.CountryCode
                        };
                        _context.ShippingAddresses.Add(oShippingAddress);
                        _context.SaveChanges();
                    }

                    var oOrder = new Order
                    {
                        CustomerId = oCustomer.CustomerId,
                        DeliveryCharge = order.DeliveryCharge,
                        GatewayCharge = order.GatewayCharge,
                        GrandTotal = order.GrandTotal,
                        Notes = order.Notes,
                        OrderDate = order.OrderDate,
                        OrderNumber = order.OrderNumber,
                        OrderStateId = order.OrderStateId,
                        PaymentMethodId = order.PaymentMethodId,
                        ShippingAddressId = oShippingAddress.ShippingAddressId,
                        SubTotal = order.SubTotal,
                        VatAmount = order.VatAmount,
                        VatPercent = order.VatPercent,
                    };

                    // =========================
                    // Save Order Header
                    // =========================
                    _context.Orders.Add(oOrder);
                    _context.SaveChanges();

                    // =========================
                    // Save Order Details
                    // =========================
                    foreach (var item in order.OrderDetails)
                    {
                        item.OrderId = oOrder.OrderId;

                        var orderDetail = new OrderDetail
                        {
                            OrderId = item.OrderId,
                            ProductId = item.ProductId,
                            Qty = item.Qty,
                            UnitPrice = item.UnitPrice,
                            TotalPrice = item.TotalPrice
                        };
                        _context.OrderDetails.Add(orderDetail);

                    }

                    _context.SaveChanges();

                    // =========================
                    // Commit Transaction
                    // =========================
                    transaction.Commit();

                    message = "data has been added successfully.";
                }
                catch (Exception)
                {
                    // =========================
                    // Rollback Transaction
                    // =========================
                    transaction.Rollback();

                    throw;
                }

            }
            return message;
        }

        public string? Update(OrderVM order)
        {
            return "";
        }

        // Delete
        public string? Delete(int userId)
        {
            return "";
        }

        // Read all
        public List<UserVM> GetAll()
        {
            var list = new List<UserVM>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("User_Read", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@QueryType", UserVM.QueryType.GetAll);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            UserVM model = new UserVM();
                            model.CreatedAt = reader.GetValue("CreatedAt") == DBNull.Value ? (DateTime?)null : reader.GetDateTime("CreatedAt");
                            model.Email = reader.GetValue("Email") == DBNull.Value ? string.Empty : reader.GetString("Email");
                            model.UserID = reader.GetValue("UserID") == DBNull.Value ? 0 : reader.GetInt32("UserID");
                            model.Username = reader.GetValue("Username") == DBNull.Value ? string.Empty : reader.GetString("Username");
                            model.IsActive = reader.GetValue("IsActive") == DBNull.Value ? false : reader.GetBoolean("IsActive");
                            model.RoleName = reader.GetValue("Description") == DBNull.Value ? string.Empty : reader.GetString("Description");

                            list.Add(model);
                        }
                    }
                    conn.Close();
                }
            }
            return list;
        }

        // Read one
        public UserVM? GetById(int userId)
        {
            UserVM? model = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("User_Read", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@QueryType", UserVM.QueryType.GetById);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            model = new UserVM();
                            model.CreatedAt = reader.GetValue("CreatedAt") == DBNull.Value ? (DateTime?)null : reader.GetDateTime("CreatedAt");
                            model.Email = reader.GetValue("Email") == DBNull.Value ? string.Empty : reader.GetString("Email");
                            model.UserID = reader.GetValue("UserID") == DBNull.Value ? 0 : reader.GetInt32("UserID");
                            model.Username = reader.GetValue("Username") == DBNull.Value ? string.Empty : reader.GetString("Username");
                            model.IsActive = reader.GetValue("IsActive") == DBNull.Value ? false : reader.GetBoolean("IsActive");
                            model.RoleName = reader.GetValue("Description") == DBNull.Value ? string.Empty : reader.GetString("Description");
                            model.RoleId = reader.GetValue("RoleId") == DBNull.Value ? 0 : reader.GetInt32("RoleId");
                        }
                    }
                    conn.Close();
                }
            }
            return model;
        }

    }
}
