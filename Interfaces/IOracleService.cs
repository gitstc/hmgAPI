namespace hmgAPI.Interfaces;

public class Merchant
{
    public string Code { get; set; } = "";
    public string AName1 { get; set; } = "";
    public string AName2 { get; set; } = "";
    public string AName3 { get; set; } = "";
    public string AName4 { get; set; } = "";
    public string Address { get; set; } = "";
    public string Telephone { get; set; } = "";
    public string Mobile { get; set; } = "";
    public string Email { get; set; } = "";
}

public class Invoice
{
    public string BranchCode { get; set; } = "";
    public string BranchName { get; set; } = "";
    public string TransactionCode { get; set; } = "";
    public string InvoiceType { get; set; } = "";
    public string InvoiceNumber { get; set; } = "";
    public DateTime InvoiceDate { get; set; }
    public decimal SubTotal { get; set; } = 0;
    public decimal TaxAmount { get; set; } = 0;
    public decimal DiscountAmount { get; set; } = 0;
    public decimal GrandTotal { get; set; } = 0;
}

public class MerchantSerial
{
    public string BranchCode { get; set; } = "";
    public string BranchName { get; set; } = "";
    public string TransactionCode { get; set; } = "";
    public string InvoiceType { get; set; } = "";
    public string InvoiceNumber { get; set; } = "";
    public DateTime InvoiceDate { get; set; }
    public string CategoryCode { get; set; } = "";
    public string ItemCode { get; set; } = "";
    public string ColorCode { get; set; } = "";
    public string ItemAName { get; set; } = "";
    public string ItemEName { get; set; } = "";
    public string ItemModelNumber { get; set; } = "";
    public string SerialNumber1 { get; set; } = "";
    public string SerialNumber2 { get; set; } = "";
    public string SerialNumber3 { get; set; } = "";
    public string SerialNumber4 { get; set; } = "";
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Status { get; set; } = 0;
}

public interface IOracleService
{
    Task<Merchant> GetMerchantByMobileNumber(string mobileNumber);
    Task<List<Invoice>> GetMerchantInvoices(string merchantCode);
    Task<List<MerchantSerial>> GetMerchantSerials(string merchantCode);
}