using Microsoft.AspNetCore.Mvc;
using hmgAPI.DTOs;
using Microsoft.EntityFrameworkCore;
using hmgAPI.Interfaces;
using Microsoft.AspNetCore.Identity;
using hmgAPI.Entities;
using AutoMapper;
using hmgAPI.Controllers;
using hmgAPI.Data;
using Microsoft.AspNetCore.Authorization;


namespace API.Controllers;
public class MerchantsController : BaseApiController
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IOracleService _oracleService;
    public MerchantsController(UserManager<AppUser> userManager, IOracleService oracleService)
    {
        _userManager = userManager;
        _oracleService = oracleService;
    }


    [HttpGet("merchant")]
    public async Task<ActionResult> GetMerchantByMobileNumber([FromQuery(Name = "mobileNumber")] string mobileNumber)
    {
        Merchant merchant = await _oracleService.GetMerchantByMobileNumber(mobileNumber);
        return Ok(merchant);
    }

    /*
    instead of recieving (merchantCode) from queryParams or json body
    to look for it in database 
    we exploited (TokenService) to GetMerchantCode from toke
    [Authorize] verifies that user is loggedIn (has a token),
    and what he can do if he has a token
    */

    //GET Merchant Invoices by merchantCode
    [Authorize]
    [HttpGet("invoices")]
    public async Task<ActionResult> GetMerchantInvoices()
    {
        // Retrieve the merchantCode from the JWT token
        var merchantCodeClaim = HttpContext.User.FindFirst("merchantCode");

        if (merchantCodeClaim == null) return Unauthorized();

        // Extract the merchant code from the claim
        var merchantCode = merchantCodeClaim.Value;

        List<Invoice> invoices = await _oracleService.GetMerchantInvoices(merchantCode);
        return Ok(invoices);
    }

    //GET Merchant Invoice Details
    [Authorize]
    [HttpGet("invoiceDetails")]
    public async Task<ActionResult> GetMerchantInvoiceDetails([FromQuery(Name = "brnCode")] string branchCode, [FromQuery(Name = "trxCode")] string transactionCode, [FromQuery(Name = "invCode")] string invoiceCode)
    {
        // Retrieve the merchantCode from the JWT token
        var merchantCodeClaim = HttpContext.User.FindFirst("merchantCode");

        if (merchantCodeClaim == null) return Unauthorized();

        // Extract the merchant code from the claim
        var merchantCode = merchantCodeClaim.Value;

        List<InvoiceDetail> invoiceDetails = await _oracleService.GetMerchantInvoiceDetails(branchCode, transactionCode, invoiceCode);
        return Ok(invoiceDetails);
    }

    //GET Merchant Serials by merchantCode
    [Authorize]
    [HttpGet("serials")]
    public async Task<ActionResult> GetMerchantSerials()
    {
        // Retrieve the merchantCode from the JWT token
        var merchantCodeClaim = HttpContext.User.FindFirst("merchantCode");

        if (merchantCodeClaim == null) return Unauthorized();

        // Extract the merchant code from the claim
        string merchantCode = merchantCodeClaim.Value;

        List<MerchantSerial> serials = await _oracleService.GetMerchantSerials(merchantCode);
        return Ok(serials);
    }

    [Authorize]
    [HttpGet("SOA")]
    public async Task<ActionResult> GetMerchantSOA([FromQuery(Name = "fromDate")] string fromDate, [FromQuery(Name = "toDate")] string toDate)
    {
        // Retrieve the merchantCode from the JWT token
        var merchantCodeClaim = HttpContext.User.FindFirst("merchantCode");

        if (merchantCodeClaim == null) return Unauthorized();

        // Extract the merchant code from the claim
        string merchantCode = merchantCodeClaim.Value;

        List<object> obj = await _oracleService.GetMerchantSOA(merchantCode, fromDate, toDate);

        return Ok(obj);
    }


    [Authorize]
    [HttpGet("Cheques")]
    public async Task<ActionResult> GetMerchantCheques([FromQuery(Name = "fromDate")] string fromDate, [FromQuery(Name = "toDate")] string toDate)
    {
        // Retrieve the merchantCode from the JWT token
        var merchantCodeClaim = HttpContext.User.FindFirst("merchantCode");

        if (merchantCodeClaim == null) return Unauthorized();

        // Extract the merchant code from the claim
        string merchantCode = merchantCodeClaim.Value;

        List<object> obj = await _oracleService.GetMerchantCheques(merchantCode, fromDate, toDate);

        return Ok(obj);
    }
}

