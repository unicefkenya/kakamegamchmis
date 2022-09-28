using MCHMIS.Mobile.Database;
using Newtonsoft.Json;
using Plugin.DeviceInfo.Abstractions;
using System.Collections.Generic;

namespace MCHMIS.Mobile.Models
{
    public class Item
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
    }

    public class ApiStatus
    {
        public string Description { get; set; }
        public int? StatusId { get; set; }
        public int? Id { get; set; }
    }

    public class LocalDeviceInfo
    {
        public string DeviceId { get; set; }
        public string DeviceModel { get; set; }
        public string DeviceManufacturer { get; set; }
        public string DeviceName { get; set; }
        public string Version { get; set; }
        public string VersionNumber { get; set; }
        public string AppVersion { get; set; }
        public string AppBuild { get; set; }
        public Platform Platform { get; set; }
        public Idiom Idiom { get; set; }
        public bool IsDevice { get; set; }
    }

    public class AuthKeyResponse
    {
        public int Success { get; set; }
    }

    public class AccountResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("userName")]
        public string Username { get; set; }

        [JsonProperty(".issued")]
        public string IssuedAt { get; set; }

        [JsonProperty(".expires")]
        public string ExpiresAt { get; set; }

        public string Error { get; set; }
        public bool Success { get; set; }
    }

    public class EnumeratorLoginResponse : ListingOptionsResponse
    {
        public Enumerator Enumerator { get; set; }
        public List<Ward> Wards { get; set; }
        public List<Village> Villages { get; set; }
        public List<CommunityArea> CommunityAreas { get; set; }
        public List<SystemCode> SystemCodes { get; set; }
        public List<SystemCodeDetail> SystemCodeDetails { get; set; }
        // public List<EnumeratorLocation> EnumeratorLocations { get; set; }
    }

    public class ListingOptionsResponse
    {
        public string Error { get; set; }
    }

    public class EnumeratorCVResponse : EnumeratorLoginResponse
    {
        public List<Registration> Registrations { get; set; }
        public List<RegistrationMember> RegistrationMembers { get; set; }
    }

    public class SelectableItemWrapper<T>
    {
        public bool IsSelected { get; set; }
        public T Item { get; set; }
    }

    //public class NsnpProgramme
    //{
    //    public bool IsSelected { get; set; }
    //    public SystemCodeDetail Item { get; set; }
    //}
}