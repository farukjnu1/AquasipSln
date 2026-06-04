using Aquasip.EF;
using Aquasip.Models;
using Aquasip.Utilities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.Data;
using System.Net.Mail;

namespace Aquasip.Repositories
{
    public class SalesOrderRepository
    {
        private readonly string _connectionString = "";
        #region constructor
        public SalesOrderRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        #endregion

        // Create
        public ResponseVM? Add(SalesOrderVM order)
        {
            ResponseVM response = new ResponseVM();
            using (var _context = new AquasipContext())
            {
                // Begin Transaction
                using var transaction = _context.Database.BeginTransaction();
                try
                {
                    #region customer
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
                    #endregion
                    #region shipping-address
                    /*var oShippingAddress = (from x in _context.ShippingAddresses
                                            where x.CustomerId == oCustomer.CustomerId
                                            && x.City == order.ShippingAddress.City
                                            && x.StateProvince == order.ShippingAddress.StateProvince
                                            && x.PostalCode == order.ShippingAddress.PostalCode
                                            && x.CountryCode == order.ShippingAddress.CountryCode
                                            select x).FirstOrDefault();*/
                    var oShippingAddress = (from x in _context.ShippingAddresses
                                            where x.CustomerId == oCustomer.CustomerId
                                            select x).FirstOrDefault();
                    if (oShippingAddress == null)
                    {
                        oShippingAddress = new ShippingAddress
                        {
                            CustomerId = oCustomer.CustomerId,
                            City = order.ShippingAddress.City,
                            StateProvince = order.ShippingAddress.StateProvince,
                            PostalCode = order.ShippingAddress.PostalCode,
                            CountryCode = order.ShippingAddress.CountryCode,
                            StreetAddress = order.ShippingAddress.StreetAddress,
                            FullName = oCustomer.FullName ?? "",
                            EmailAddress = oCustomer.Email,
                            PhoneNumber = oCustomer.PhoneNumber
                        };
                        _context.ShippingAddresses.Add(oShippingAddress);
                        _context.SaveChanges();
                    }
                    else 
                    {
                        oShippingAddress.City = order.ShippingAddress.City;
                        oShippingAddress.StateProvince = order.ShippingAddress.StateProvince;
                        oShippingAddress.PostalCode = order.ShippingAddress.PostalCode;
                        oShippingAddress.CountryCode = order.ShippingAddress.CountryCode;
                        oShippingAddress.StreetAddress = order.ShippingAddress.StreetAddress;
                        oShippingAddress.FullName = oCustomer.FullName ?? "";
                        oShippingAddress.EmailAddress = oCustomer.Email;
                        oShippingAddress.PhoneNumber = oCustomer.PhoneNumber;
                    }
                    #endregion
                    #region order-summery
                    order.OrderNumber = CodeGenerate.SalesOrderNumber(DateTime.Now, oCustomer.PhoneNumber ?? "");
                    var oOrder = new SalesOrder
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
                        IsActive = true
                    };
                    // =========================
                    // Save Order Header
                    // =========================
                    _context.SalesOrders.Add(oOrder);
                    _context.SaveChanges();
                    #endregion
                    #region order-details
                    // =========================
                    // Save Order Details
                    // =========================
                    foreach (var item in order.OrderDetails)
                    {
                        item.OrderId = oOrder.OrderId;
                        var orderDetail = new SalesOrderDetail
                        {
                            OrderId = item.OrderId,
                            ProductId = item.ProductId,
                            Qty = item.Qty,
                            UnitPrice = item.UnitPrice,
                            TotalPrice = item.TotalPrice
                        };
                        _context.SalesOrderDetails.Add(orderDetail);
                    }
                    _context.SaveChanges();
                    #endregion
                    // =========================
                    // Commit Transaction
                    // =========================
                    transaction.Commit();
                    response.Success = true;
                    response.Message = "Order has been submit successfully.";
                }
                catch (Exception ex)
                {
                    // =========================
                    // Rollback Transaction
                    // =========================
                    transaction.Rollback();
                    response.Message = "order submition failed.";
                }
            }
            return response;
        }

        public ResponseVM? Update(SalesOrderVM order)
        {
            return null;
        }

        // Delete
        public ResponseVM? Delete(int userId)
        {
            return null;
        }

        // Read all
        public List<SalesOrderVM> GetAll(int orderStateId = 0, int pageSize = 10)
        {
            var list = new List<SalesOrderVM>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SalesOrder_Read", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    //@OrderStateId
                    cmd.Parameters.AddWithValue("@OrderStateId", orderStateId);
                    cmd.Parameters.AddWithValue("@PageSize", pageSize);
                    cmd.Parameters.AddWithValue("@QueryType", SalesOrderVM.QueryType.GetAll);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            SalesOrderVM model = new SalesOrderVM();
                            model.GrandTotal = reader.GetValue("GrandTotal") == DBNull.Value ? 0: reader.GetDecimal("GrandTotal");
                            model.Notes = reader.GetValue("Notes") == DBNull.Value ? "" : reader.GetString("Notes");
                            model.OrderId = reader.GetValue("OrderId") == DBNull.Value ? 0 : reader.GetInt64("OrderId");
                            model.OrderNumber = reader.GetValue("OrderNumber") == DBNull.Value ? "" : reader.GetString("OrderNumber");
                            model.OrderDate = reader.GetValue("OrderDate") == DBNull.Value ? DateTime.Now : reader.GetDateTime("OrderDate");
                            
                            model.Customer = reader.GetValue("Customer") == DBNull.Value ? new CustomerVM() : JsonConversion.DeserializeObject<CustomerVM>(reader.GetString("Customer")); 
                            

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
                    cmd.Parameters.AddWithValue("@QueryType", SalesOrderVM.QueryType.GetById);

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
