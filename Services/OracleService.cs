//Nuget Package 'Oracle.ManagedDataAccess.Core'

using System.Data;
using hmgAPI.Interfaces;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace hmgAPI.Services;
public class OracleService : IOracleService
{
    private string GetConnectionString()
    {
        string provider = "212.35.72.130";
        string port = "1521";
        string dataSource = "orcl";
        string username = "web_app";
        string password = "web_app";
        string connectionString = "Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = " + provider + ")(PORT = " + port + "))(CONNECT_DATA = (SID = " + dataSource + ")));Password=" + password + ";User ID=" + username;

        return connectionString;
    }

    public async Task<Merchant> GetMerchantByMobileNumber(string mobileNumber)
    {
        Merchant merchant = null;

        OracleConnection conn;
        OracleCommand cmd;
        OracleDataReader reader;

        const string query = "PKG_MER.REC_QRY";

        using (conn = new OracleConnection())
        {
            conn.ConnectionString = GetConnectionString();

            try
            {
                await conn.OpenAsync();

                cmd = new OracleCommand(query, conn)
                {
                    CommandType = CommandType.StoredProcedure,
                    BindByName = true
                };

                OracleParameter telNo = new OracleParameter()
                {
                    ParameterName = "PARA_REC_TELNO",
                    Direction = ParameterDirection.Input,
                    OracleDbType = OracleDbType.Varchar2,
                    Value = mobileNumber
                };
                OracleParameter refCursor = new OracleParameter()
                {
                    ParameterName = "PARA_REC_CSRECTBL",
                    Direction = ParameterDirection.InputOutput,
                    OracleDbType = OracleDbType.RefCursor
                };
                cmd.Parameters.Add(telNo);
                cmd.Parameters.Add(refCursor);

                await cmd.ExecuteNonQueryAsync();

                using (reader = ((OracleRefCursor)refCursor.Value).GetDataReader())
                {
                    while (await reader.ReadAsync())
                    {
                        merchant = new Merchant
                        {
                            Code = reader["CS_REC_CODE"].ToString(),
                            AName1 = reader["CS_REC_ANAME1"].ToString(),
                            AName2 = reader["CS_REC_ANAME2"].ToString(),
                            AName3 = reader["CS_REC_ANAME3"].ToString(),
                            AName4 = reader["CS_REC_ANAME4"].ToString(),
                            Address = reader["CS_REC_ADD1"].ToString(),
                            Telephone = reader["CS_REC_TELNO"].ToString(),
                            Mobile = reader["CS_MOB_TELNO"].ToString(),
                            Email = reader["CS_REC_EMAIL"].ToString()
                        };
                    }

                    await reader.CloseAsync();
                }

                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                merchant = null;
            }
        }

        return merchant;
    }

    public async Task<List<Invoice>> GetMerchantInvoices(string merchantCode)
    {
        List<Invoice> invoices = new List<Invoice>();
        Invoice invoice = null;

        OracleConnection conn;
        OracleCommand cmd;
        OracleDataReader reader;

        const string query = "PKG_MER.INV_QRY_MAS";

        using (conn = new OracleConnection())
        {
            conn.ConnectionString = GetConnectionString();

            try
            {
                await conn.OpenAsync();

                cmd = new OracleCommand(query, conn)
                {
                    CommandType = CommandType.StoredProcedure,
                    BindByName = true
                };

                OracleParameter merCode = new OracleParameter()
                {
                    ParameterName = "PARA_REC_CODE",
                    Direction = ParameterDirection.Input,
                    OracleDbType = OracleDbType.Varchar2,
                    Value = merchantCode
                };
                OracleParameter refCursor = new OracleParameter()
                {
                    ParameterName = "PARA_INV_DATA_MAS",
                    Direction = ParameterDirection.InputOutput,
                    OracleDbType = OracleDbType.RefCursor
                };
                cmd.Parameters.Add(merCode);
                cmd.Parameters.Add(refCursor);

                await cmd.ExecuteNonQueryAsync();

                using (reader = ((OracleRefCursor)refCursor.Value).GetDataReader())
                {
                    while (await reader.ReadAsync())
                    {
                        invoice = new Invoice
                        {
                            BranchCode = reader["BRANCH_CODE"].ToString(),
                            BranchName = reader["BRANCH_NAME"].ToString(),
                            TransactionCode = reader["TRANSACTION_CODE"].ToString(),
                            InvoiceType = reader["INVOUCE_TYPE_DESCRIBTION"].ToString(),
                            InvoiceNumber = reader["INVOICE_NO"].ToString(),
                            InvoiceDate = DateTime.Parse(reader["INVOICE_DATE"].ToString()),
                            SubTotal = decimal.Parse(reader["INVOICE_TAX"].ToString()),
                            TaxAmount = decimal.Parse(reader["INVOICE_TOTAL_WOTAX"].ToString()),
                            DiscountAmount = decimal.Parse(reader["INVOICE_DISCOUNT"].ToString()),
                            GrandTotal = decimal.Parse(reader["INVOICE_TOTAL_TAX"].ToString())
                        };

                        invoices.Add(invoice);
                    }

                    await reader.CloseAsync();
                }

                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                invoices = null;
            }
        }

        return invoices;
    }

    public async Task<List<MerchantSerial>> GetMerchantSerials(string merchantCode)
    {
        List<MerchantSerial> serials = new List<MerchantSerial>();
        MerchantSerial serial = null;

        OracleConnection conn;
        OracleCommand cmd;
        OracleDataReader reader;

        const string query = "PKG_MER.SER_QRY";

        using (conn = new OracleConnection())
        {
            conn.ConnectionString = GetConnectionString();

            try
            {
                await conn.OpenAsync();

                cmd = new OracleCommand(query, conn)
                {
                    CommandType = CommandType.StoredProcedure,
                    BindByName = true
                };

                OracleParameter merCode = new OracleParameter()
                {
                    ParameterName = "PARA_REC_CODE",
                    Direction = ParameterDirection.Input,
                    OracleDbType = OracleDbType.Varchar2,
                    Value = merchantCode
                };
                OracleParameter refCursor = new OracleParameter()
                {
                    ParameterName = "PARA_SER_DATA",
                    Direction = ParameterDirection.InputOutput,
                    OracleDbType = OracleDbType.RefCursor
                };
                cmd.Parameters.Add(merCode);
                cmd.Parameters.Add(refCursor);

                await cmd.ExecuteNonQueryAsync();

                using (reader = ((OracleRefCursor)refCursor.Value).GetDataReader())
                {
                    while (await reader.ReadAsync())
                    {
                        serial = new MerchantSerial
                        {
                            BranchCode = reader["BRANCH_CODE"].ToString(),
                            BranchName = reader["BRANCH_NAME"].ToString(),
                            TransactionCode = reader["TRANSACTION_CODE"].ToString(),
                            InvoiceType = reader["INVOUCE_TYPE_DESCRIBTION"].ToString(),
                            InvoiceNumber = reader["INVOICE_NO"].ToString(),
                            InvoiceDate = DateTime.Parse(reader["INVOICE_DATE"].ToString()),
                            CategoryCode = reader["CAT_CLASS"].ToString(),
                            ItemCode = reader["ITEM_NO"].ToString(),
                            ColorCode = reader["COLOR_CODE"].ToString(),
                            ItemAName = reader["ITEM_A_NAME"].ToString(),
                            ItemEName = reader["ITEM_E_NAME"].ToString(),
                            ItemModelNumber = reader["MODEL_NO"].ToString(),
                            SerialNumber1 = reader["SER_NO1"].ToString(),
                            SerialNumber2 = reader["SER_NO2"].ToString(),
                            SerialNumber3 = reader["SER_NO3"].ToString(),
                            SerialNumber4 = reader["SER_NO4"].ToString(),
                            StartDate = DateTime.Parse(reader["START_DATE"].ToString()),
                            EndDate = DateTime.Parse(reader["END_DATE"].ToString()),
                            Status = int.Parse(reader["MO_GRNT_STATUS"].ToString())
                        };

                        serials.Add(serial);
                    }

                    await reader.CloseAsync();
                }

                await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                serials = null;
            }
        }

        return serials;
    }
}