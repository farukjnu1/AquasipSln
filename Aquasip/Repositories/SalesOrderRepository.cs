using Aquasip.EF;
using Aquasip.Models;
using Aquasip.Services.TokenServices;
using Aquasip.Utilities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.Collections.Generic;
using System.Data;
using System.Net.Mail;
using static Aquasip.Models.SalesOrderVM;

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
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SalesOrder_Read", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@OrderStateId", orderStateId);
                        cmd.Parameters.AddWithValue("@PageSize", pageSize);
                        cmd.Parameters.AddWithValue("@QueryType", SalesOrderVM.QueryType.GetAll);

                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                SalesOrderVM model = new SalesOrderVM();
                                model.GrandTotal = reader.GetValue("GrandTotal") == DBNull.Value ? 0 : reader.GetDecimal("GrandTotal");
                                model.Notes = reader.GetValue("Notes") == DBNull.Value ? "" : reader.GetString("Notes");
                                model.OrderId = reader.GetValue("OrderId") == DBNull.Value ? 0 : reader.GetInt64("OrderId");
                                model.OrderNumber = reader.GetValue("OrderNumber") == DBNull.Value ? "" : reader.GetString("OrderNumber");
                                model.OrderDate = reader.GetValue("OrderDate") == DBNull.Value ? DateTime.Now : reader.GetDateTime("OrderDate");
                                model.CustomerId = reader.GetValue("OrderId") == DBNull.Value ? 0 : reader.GetInt64("CustomerId");
                                model.Customer = reader.GetValue("Customer") == DBNull.Value ? new CustomerVM() : JsonConversion.DeserializeObject<List<CustomerVM>>(reader.GetString("Customer")).FirstOrDefault();
                                model.SalesOrderState = reader.GetValue("SalesOrderState") == DBNull.Value ? new SalesOrderStateVM() : JsonConversion.DeserializeObject<List<SalesOrderStateVM>>(reader.GetString("SalesOrderState")).FirstOrDefault();
                                list.Add(model);
                            }
                        }
                        conn.Close();
                    }
                }
            }
            catch(Exception ex)
            {
            }
            return list;
        }

        // Read one
        public SalesOrderVM? GetById(long id)
        {
            SalesOrderVM? model = null;
            try 
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SalesOrder_Read", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@OrderID", id);
                        cmd.Parameters.AddWithValue("@QueryType", SalesOrderVM.QueryType.GetById);
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                model = new SalesOrderVM();
                                model.GrandTotal = reader.GetValue("GrandTotal") == DBNull.Value ? 0 : reader.GetDecimal("GrandTotal");
                                model.Notes = reader.GetValue("Notes") == DBNull.Value ? "" : reader.GetString("Notes");
                                model.OrderId = reader.GetValue("OrderId") == DBNull.Value ? 0 : reader.GetInt64("OrderId");
                                model.OrderNumber = reader.GetValue("OrderNumber") == DBNull.Value ? "" : reader.GetString("OrderNumber");
                                model.DeliveryCharge = reader.GetValue("DeliveryCharge") == DBNull.Value ? 0 : reader.GetDecimal("DeliveryCharge");
                                model.GatewayCharge = reader.GetValue("GatewayCharge") == DBNull.Value ? 0 : reader.GetDecimal("GatewayCharge");
                                model.VatPercent = reader.GetValue("VatPercent") == DBNull.Value ? 0 : reader.GetDecimal("VatPercent");
                                model.VatAmount = reader.GetValue("VatAmount") == DBNull.Value ? 0 : reader.GetDecimal("VatAmount");
                                model.SubTotal = reader.GetValue("SubTotal") == DBNull.Value ? 0 : reader.GetDecimal("GatewayCharge");
                                model.OrderStateId = reader.GetValue("OrderStateId") == DBNull.Value ? 0 : reader.GetInt32("OrderStateId");
                                model.OrderDate = reader.GetValue("OrderDate") == DBNull.Value ? DateTime.Now : reader.GetDateTime("OrderDate");
                                model.IsActive = reader.GetValue("IsActive") == DBNull.Value ? false : reader.GetBoolean("IsActive");
                                model.Customer = reader.GetValue("Customer") == DBNull.Value ? new CustomerVM() : JsonConversion.DeserializeObject<List<CustomerVM>>(reader.GetString("Customer")).FirstOrDefault();
                                model.SalesOrderState = reader.GetValue("SalesOrderState") == DBNull.Value ? new SalesOrderStateVM() : JsonConversion.DeserializeObject<List<SalesOrderStateVM>>(reader.GetString("SalesOrderState")).FirstOrDefault();
                                model.PaymentMethod = reader.GetValue("PaymentMethod") == DBNull.Value ? new PaymentMethodVM() : JsonConversion.DeserializeObject<List<PaymentMethodVM>>(reader.GetString("PaymentMethod")).FirstOrDefault();
                                model.ShippingAddress = reader.GetValue("ShippingAddress") == DBNull.Value ? new ShippingAddressVM() : JsonConversion.DeserializeObject<List<ShippingAddressVM>>(reader.GetString("ShippingAddress")).FirstOrDefault();
                                model.CustomerPayments = reader.GetValue("CustomerPayments") == DBNull.Value ? new List<CustomerPaymentVM>() : JsonConversion.DeserializeObject<List<CustomerPaymentVM>>(reader.GetString("CustomerPayments"));
                                model.OrderDetails = reader.GetValue("SalesOrderDetails") == DBNull.Value ? new List<SalesOrderDetailVM>() : JsonConversion.DeserializeObject<List<SalesOrderDetailVM>>(reader.GetString("SalesOrderDetails"));
                                model.CustomerPayment = new CustomerPaymentVM { OrderId = model.OrderId };
                                foreach (var item in model.OrderDetails)
                                {
                                    item.Product = new ProductVM { ProductId = item.ProductId, ProductName = item.ProductName };
                                }
                                foreach (var item in model.CustomerPayments)
                                {
                                    item.PaymentMethod = new PaymentMethodVM { PaymentMethodName = item.PaymentMethod1 };
                                    item.PaymentStatus = new PaymentStatusVM { PaymentStatus1 = item.PaymentStatus1 };
                                }
                            }
                        }
                        conn.Close();
                    }
                }
            }
            catch(Exception ex)
            {
            }
            return model;
        }

    }
}
