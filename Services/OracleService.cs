//Nuget Package 'Oracle.ManagedDataAccess.Core'

using System.Data;
using System.Dynamic;
using System.Globalization;
using hmgAPI.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
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

    private string FormateDate(string str)
    {
        DateTime dt = DateTime.ParseExact(str, "dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
        return dt.ToString("yyyy-MM-dd");
    }

    private string FormateDate(DateTime date) {
        return date.ToString("yyyy-MM-dd");
    }

    private string FormateDateTime(string str)
    {
        DateTime dt = DateTime.ParseExact(str, "dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
        return dt.ToString("yyyy-MM-dd HH:mm:ss");
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
            catch
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
                    int _index = 0;
                    while (await reader.ReadAsync())
                    {
                        _index++;

                        invoice = new Invoice
                        {
                            _index = _index,
                            BranchCode = reader["BRANCH_CODE"].ToString(),
                            BranchName = reader["BRANCH_NAME"].ToString(),
                            TransactionCode = reader["TRANSACTION_CODE"].ToString(),
                            InvoiceType = reader["INVOUCE_TYPE_DESCRIBTION"].ToString(),
                            InvoiceNumber = reader["INVOICE_NO"].ToString(),
                            // InvoiceDate = DateTime.Parse(reader["INVOICE_DATE"].ToString()),
                            InvoiceDate = FormateDate((DateTime) reader["INVOICE_DATE"]),
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
            catch
            {
                invoices = null;
            }
        }

        return invoices;
    }

    public async Task<List<InvoiceDetail>> GetMerchantInvoiceDetails(string branchCode, string transactionCode, string invoiceCode)
    {
        List<InvoiceDetail> invoiceDetails = new List<InvoiceDetail>();
        InvoiceDetail invoiceDetail = null;

        OracleConnection conn;
        OracleCommand cmd;
        OracleDataReader reader;

        const string query = "PKG_MER.INV_QRY_DTL";

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

                OracleParameter brnCode = new OracleParameter()
                {
                    ParameterName = "PARA_BRN_CODE",
                    Direction = ParameterDirection.Input,
                    OracleDbType = OracleDbType.Varchar2,
                    Value = branchCode
                };
                OracleParameter trxCode = new OracleParameter()
                {
                    ParameterName = "PARA_TRX_CODE",
                    Direction = ParameterDirection.Input,
                    OracleDbType = OracleDbType.Varchar2,
                    Value = transactionCode
                };
                OracleParameter invCode = new OracleParameter()
                {
                    ParameterName = "PARA_INV_CODE",
                    Direction = ParameterDirection.Input,
                    OracleDbType = OracleDbType.Varchar2,
                    Value = invoiceCode
                };
                OracleParameter refCursor = new OracleParameter()
                {
                    ParameterName = "PARA_INV_DATA_DTL",
                    Direction = ParameterDirection.InputOutput,
                    OracleDbType = OracleDbType.RefCursor
                };
                cmd.Parameters.Add(brnCode);
                cmd.Parameters.Add(trxCode);
                cmd.Parameters.Add(invCode);
                cmd.Parameters.Add(refCursor);

                await cmd.ExecuteNonQueryAsync();

                using (reader = ((OracleRefCursor)refCursor.Value).GetDataReader())
                {
                    while (await reader.ReadAsync())
                    {
                        invoiceDetail = new InvoiceDetail
                        {
                            BranchCode = reader["BRANCH_CODE"].ToString(),
                            TransactionCode = reader["TRANSACTION_CODE"].ToString(),
                            InvoiceNumber = reader["INVOICE_NO"].ToString(),
                            CategoryClass = reader["CAT_CLASS"].ToString(),
                            ItemNumber = reader["ITEM_NO"].ToString(),
                            ColorCode = reader["COLOR_CODE"].ToString(),
                            ItemAName = reader["ITEM_A_NAME"].ToString(),
                            ItemEName = reader["ITEM_E_NAME"].ToString(),
                            ItemModelNumber = reader["MODEL_NO"].ToString(),
                            InvoiceQuantity = int.Parse(reader["INV_QTY"].ToString()),
                            ItemPrice = decimal.Parse(reader["ITM_PRICE"].ToString()),
                            ItemPriceTotal = decimal.Parse(reader["ITM_PRICE_TOTAL"].ToString()),
                            ItemPriceWithoutTax = decimal.Parse(reader["ITM_ACT"].ToString()),
                            ItemTaxAmount = decimal.Parse(reader["ITM_TAX"].ToString()),
                            ItemTotalTaxAmount = decimal.Parse(reader["ITM_TAX_TOTAL"].ToString()),
                            ItemDiscountAmount = decimal.Parse(reader["ITM_DISCOUNT"].ToString()),
                            ItemTotalDiscountAmount = decimal.Parse(reader["ITM_DISCOUNT_TOTAL"].ToString())
                        };

                        invoiceDetails.Add(invoiceDetail);
                    }

                    await reader.CloseAsync();
                }

                await conn.CloseAsync();
            }
            catch
            {
                invoiceDetails = null;
            }
        }

        return invoiceDetails;
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
                            InvoiceDate = FormateDate((DateTime)reader["INVOICE_DATE"]),
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
                            StartDate = FormateDate((DateTime)reader["START_DATE"]),
                            EndDate = FormateDate((DateTime)reader["END_DATE"]),
                            Status = int.Parse(reader["MO_GRNT_STATUS"].ToString())
                        };

                        serials.Add(serial);
                    }

                    await reader.CloseAsync();
                }

                await conn.CloseAsync();
            }
            catch
            {
                serials = null;
            }
        }

        return serials;
    }

    public async Task<List<object>> GetMerchantSOA(string merchantCode, string fromDate, string toDate)
    {
        List<object> SOAList = new List<object>();


        try
        {
            using (var connection = new OracleConnection(GetConnectionString()))
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "PKG_MER.REC_SOA";

                    OracleParameter merCode = new OracleParameter()
                    {
                        ParameterName = "PARA_REC_CODE",
                        Direction = ParameterDirection.Input,
                        OracleDbType = OracleDbType.Varchar2,
                        Value = merchantCode
                    };
                    OracleParameter fDate = new OracleParameter()
                    {
                        ParameterName = "PARA_F_DATE",
                        Direction = ParameterDirection.Input,
                        OracleDbType = OracleDbType.Date,
                        Value = DateTime.Parse(fromDate)
                    };
                    OracleParameter tDate = new OracleParameter()
                    {
                        ParameterName = "PARA_T_DATE",
                        Direction = ParameterDirection.Input,
                        OracleDbType = OracleDbType.Date,
                        Value = DateTime.Parse(toDate)
                    };
                    OracleParameter refCursor = new OracleParameter()
                    {
                        ParameterName = "PARA_SOA_DATA",
                        Direction = ParameterDirection.InputOutput,
                        OracleDbType = OracleDbType.RefCursor
                    };

                    command.Parameters.Add(merCode);
                    command.Parameters.Add(fDate);
                    command.Parameters.Add(tDate);
                    command.Parameters.Add(refCursor);

                    await connection.OpenAsync();
                    var reader = await command.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        var dict = new Dictionary<string, object>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            dict["_index"] = (i + 1).ToString();
                            var value = reader.GetValue(i);
                            if(value.GetType().Equals(DateTime.Now.GetType())) {
                                value = FormateDate((DateTime) value);
                            }
                            dict[reader.GetName(i)] = value;
                        }
                        SOAList.Add(dict);

                    }
                    return SOAList;
                }
            }
        }
        catch (Exception ex)
        {
            List<object> errList = new List<object>();
            errList.Add(ex.Message);
            return errList;
        }
    }

    public async Task<List<object>> GetMerchantCheques(string merchantCode, string fromDate, string toDate)
    {
        List<object> ChequesList = new List<object>();
        try
        {
            using (var connection = new OracleConnection(GetConnectionString()))
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "PKG_MER.REC_CHEQUES";

                    OracleParameter merCode = new OracleParameter()
                    {
                        ParameterName = "PARA_REC_CODE",
                        Direction = ParameterDirection.Input,
                        OracleDbType = OracleDbType.Varchar2,
                        Value = merchantCode
                    };
                    OracleParameter fDate = new OracleParameter()
                    {
                        ParameterName = "PARA_F_DATE",
                        Direction = ParameterDirection.Input,
                        OracleDbType = OracleDbType.Date,
                        Value = DateTime.Parse(fromDate)
                    };
                    OracleParameter tDate = new OracleParameter()
                    {
                        ParameterName = "PARA_T_DATE",
                        Direction = ParameterDirection.Input,
                        OracleDbType = OracleDbType.Date,
                        Value = DateTime.Parse(toDate)
                    };
                    OracleParameter refCursor = new OracleParameter()
                    {
                        ParameterName = "PARA_CHQ_DATA",
                        Direction = ParameterDirection.InputOutput,
                        OracleDbType = OracleDbType.RefCursor
                    };

                    command.Parameters.Add(merCode);
                    command.Parameters.Add(fDate);
                    command.Parameters.Add(tDate);
                    command.Parameters.Add(refCursor);

                    await connection.OpenAsync();
                    var reader = await command.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        var dict = new Dictionary<string, object>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            dict["_index"] = (i + 1).ToString();
                            var value = reader.GetValue(i);
                            if(value.GetType().Equals(DateTime.Now.GetType())) {
                                value = FormateDate((DateTime) value);
                            }
                            dict[reader.GetName(i)] = value;
                        }
                        ChequesList.Add(dict);

                    }
                    return ChequesList;
                }
            }

        }
        catch (Exception ex)
        {
            List<object> errList = new List<object>();
            errList.Add(ex.Message);
            return errList;
        }

    }
}