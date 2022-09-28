namespace MCHMIS.ViewModels
{
    public class SRAPI
    {
        public string Remarks { get; set; }
        public string Status { get; set; }
    }

    public class Authentication : SRAPI
    {
        public string TokenAuth { get; set; }
        public string access_token { get; set; }
        public string expires_in { get; set; }
    }

    public class LoginVm
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }

    public class SendSMSView
    {
        public string grant_type { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
    }

    public class VerificationSrPostVm
    {
        public string TokenCode { get; set; }

        public string IDNumber { get; set; }

        public string Names { get; set; }
    }

    public class SrVerificationVm : SRAPI
    {
        public string ProgrammeCode { get; set; }
        public string NamesOnSR { get; set; }
        public string IDNumber { get; set; }
        public string Sex { get; set; }
        public string DateOfBirth { get; set; }
        public string CountyName { get; set; }
        public string ConstituencyName { get; set; }
        public string LocationName { get; set; }
        public string SubLocationName { get; set; }
        public string Village { get; set; }
    }

    public class IprsVerificationVm : SRAPI
    {
        public string First_Name { get; set; }
        public string Surname { get; set; }
        public string Middle_Name { get; set; }
        public string ID_Number { get; set; }
        public string Gender { get; set; }
        public string Date_of_Birth { get; set; }
        public string Date_of_Issue { get; set; }
        public string Place_of_Birth { get; set; }
        public string Serial_Number { get; set; }
        public string Address { get; set; }
    }

    public class ChangeSrPasswordPostVm
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string TokenCode { get; set; }
    }
}