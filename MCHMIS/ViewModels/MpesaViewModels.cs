using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace MCHMIS.ViewModels
{
    public class MPESAViewModel
    {
        public string InitiatorName { get; set; }
        public string SecurityCredential { get; set; }
        public string CommandID { get; set; }

        [DisplayFormat(DataFormatString = "{0:###,##0}")]
        public string Amount { get; set; }

        public string PartyA { get; set; }
        public string PartyB { get; set; }
        public string Remarks { get; set; }
        public string QueueTimeOutURL { get; set; }
        public string ResultURL { get; set; }
        public string Occasion { get; set; }
    }

    public class MPESAReversalViewModel
    {
        public string CommandID { get; set; }
        public string ReceiverParty { get; set; }
        public int ReceiverIdentifierType { get; set; }
        public string Remarks { get; set; }
        public string Initiator { get; set; }

        public string SecurityCredential { get; set; }

        public string QueueTimeOutURL { get; set; }
        public string ResultURL { get; set; }
        public string TransactionID { get; set; }

        public string Occasion { get; set; }
    }

    public class MPESAConfirmPaymentViewModel
    {
        public string BusinessShortCode { get; set; }
        public string Password { get; set; }
        public string Timestamp { get; set; }
        public string CheckoutRequestID { get; set; }
    }

    public class MPESARegisterURLViewModel
    {
        public string Password { get; set; }
        public string ValidationURL { get; set; }
        public string ConfirmationURL { get; set; }
        public string ResponseType { get; set; }
        public string ShortCode { get; set; }
    }

    public class MPESAResponseViewModel
    {
        public string MerchantRequestID { get; set; }
        public string CheckoutRequestID { get; set; }
        public string ResponseCode { get; set; }
        public string ResultDesc { get; set; }
        public string ResponseDescription { get; set; }
        public int ResultCode { get; set; }
        public int BeneficiaryId { get; set; }
        public string RawResponse { get; set; }
    }

    public class TokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessCode { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiryDate { get; set; }
    }

    public class Item
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class CallbackMetadata
    {
        public List<Item> Item { get; set; }
    }

    public class StkCallback
    {
        public string MerchantRequestID { get; set; }
        public string CheckoutRequestID { get; set; }
        public int ResultCode { get; set; }
        public string ResultDesc { get; set; }
        public CallbackMetadata CallbackMetadata { get; set; }
    }

    public class Body
    {
        public StkCallback stkCallback { get; set; }
    }

    public class TestData
    {
        public int Id { get; set; }
        public string Value { get; set; }
    }

    public class TempTable
    {
        public int Id { get; set; }
        public string Col1 { get; set; }
        public string Col2 { get; set; }
        public string Col3 { get; set; }
    }

    public class ResultParameter
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class ResultParameters
    {
        public List<ResultParameter> ResultParameter { get; set; }
    }

    public class ReferenceItem
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class ReferenceData
    {
        public ReferenceItem ReferenceItem { get; set; }
    }

    public class Result
    {
        public int ResultType { get; set; }
        public int ResultCode { get; set; }
        public string ResultDesc { get; set; }
        public string OriginatorConversationID { get; set; }
        public string ConversationID { get; set; }
        public string TransactionID { get; set; }
        public ResultParameters ResultParameters { get; set; }
        public ReferenceData ReferenceData { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class RootObject
    {
        public Result Result { get; set; }
    }

    public class MPesaFeedBack
    {
        public int Id { get; set; }
        public int PaymentId { get; set; }
        public string Result { get; set; }
    }
}