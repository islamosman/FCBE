using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fly.Models
{
    public class WeAcceptTockenModelContainer
    {
        public WeAcceptTockenModel obj { get; set; }
        public string type { get; set; }
    }

    public class WeAcceptTockenModel
    {
        public int id { get; set; }
        public string token { get; set; }
        public string masked_pan { get; set; }
        public int merchant_id { get; set; }
        public string card_subtype { get; set; }
        public string created_at { get; set; }
        public string email { get; set; }
        public string order_id { get; set; }
        public bool user_added { get; set; }
    }


    ///////////////
    public class Merchant
    {
        public int id { get; set; }
        public DateTime created_at { get; set; }
        public List<string> phones { get; set; }
        public List<string> company_emails { get; set; }
        public string company_name { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string city { get; set; }
        public string postal_code { get; set; }
        public string street { get; set; }
    }

    public class ShippingData
    {
        public int id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string street { get; set; }
        public string building { get; set; }
        public string floor { get; set; }
        public string apartment { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string email { get; set; }
        public string phone_number { get; set; }
        public string postal_code { get; set; }
        public string extra_description { get; set; }
        public string shipping_method { get; set; }
        public int order_id { get; set; }
        public int order { get; set; }
    }

    public class Order
    {
        public int id { get; set; }
        public DateTime created_at { get; set; }
        public bool delivery_needed { get; set; }
        public Merchant merchant { get; set; }
        public object collector { get; set; }
        public int amount_cents { get; set; }
        public ShippingData shipping_data { get; set; }
        public object shipping_details { get; set; }
        public string currency { get; set; }
        public bool is_payment_locked { get; set; }
        public string merchant_order_id { get; set; }
        public object wallet_notification { get; set; }
        public int paid_amount_cents { get; set; }
        public bool notify_user_with_email { get; set; }
        public List<object> items { get; set; }
        public string order_url { get; set; }
        public int commission_fees { get; set; }
        public int delivery_fees { get; set; }
        public string payment_method { get; set; }
        public object merchant_staff_tag { get; set; }
        public string api_source { get; set; }
        public object pickup_data { get; set; }
    }

    public class SourceData
    {
        public string type { get; set; }
        public string pan { get; set; }
        public string sub_type { get; set; }
    }

    public class Data
    {
        public object card_num { get; set; }
        public string batch_no { get; set; }
        public string card_type { get; set; }
        public string command { get; set; }
        public string message { get; set; }
        public string order_info { get; set; }
        public string klass { get; set; }
        public string avs_acq_response_code { get; set; }
        public string receipt_no { get; set; }
        public string authorize_id { get; set; }
        public DateTime created_at { get; set; }
        public string currency { get; set; }
        public string txn_response_code { get; set; }
        public string merchant_txn_ref { get; set; }
        public string avs_result_code { get; set; }
        public string secure_hash { get; set; }
        public int gateway_integration_pk { get; set; }
        public string transaction_no { get; set; }
        public string acq_response_code { get; set; }
        public string merchant { get; set; }
        public string amount { get; set; }
    }

    public class BillingData
    {
        public string email { get; set; }
        public string apartment { get; set; }
        public string postal_code { get; set; }
        public string first_name { get; set; }
        public string phone_number { get; set; }
        public string building { get; set; }
        public string state { get; set; }
        public string street { get; set; }
        public string city { get; set; }
        public string floor { get; set; }
        public string extra_description { get; set; }
        public string last_name { get; set; }
        public string country { get; set; }
    }

    public class PaymentKeyClaims
    {
        public int order_id { get; set; }
        public int user_id { get; set; }
        public BillingData billing_data { get; set; }
        public string currency { get; set; }
        public bool lock_order_when_paid { get; set; }
        public string pmk_ip { get; set; }
        public int exp { get; set; }
        public int integration_id { get; set; }
        public int amount_cents { get; set; }
    }

    public class MainObj
    {
        public int id { get; set; }
        public bool pending { get; set; }
        public int amount_cents { get; set; }
        public bool success { get; set; }
        public bool is_auth { get; set; }
        public bool is_capture { get; set; }
        public bool is_standalone_payment { get; set; }
        public bool is_voided { get; set; }
        public bool is_refunded { get; set; }
        public bool is_3d_secure { get; set; }
        public int integration_id { get; set; }
        public int profile_id { get; set; }
        public bool has_parent_transaction { get; set; }
        public Order order { get; set; }
        public DateTime created_at { get; set; }
        public List<object> transaction_processed_callback_responses { get; set; }
        public string currency { get; set; }
        public SourceData source_data { get; set; }
        public string api_source { get; set; }
        public bool is_void { get; set; }
        public bool is_refund { get; set; }
        public Data data { get; set; }
        public bool is_hidden { get; set; }
        public PaymentKeyClaims payment_key_claims { get; set; }
        public bool error_occured { get; set; }
        public bool is_live { get; set; }
        public object other_endpoint_reference { get; set; }
        public int refunded_amount_cents { get; set; }
        public int source_id { get; set; }
        public bool is_captured { get; set; }
        public int captured_amount { get; set; }
        public object merchant_staff_tag { get; set; }
        public int owner { get; set; }
        public object parent_transaction { get; set; }
    }

    public class WeAcceptRootObject
    {
        public MainObj obj { get; set; }
        public string type { get; set; }
    }
}