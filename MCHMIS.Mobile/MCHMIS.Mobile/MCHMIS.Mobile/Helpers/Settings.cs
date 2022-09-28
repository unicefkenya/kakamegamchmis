using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
namespace MCHMIS.Mobile.Helpers
{
    public class Settings
    {
        private const string AccessTokenExpirationDateKey = "token_expiry_key";
        private const string AccessTokenIssuedDateKey = "token_issued_key";
        private const string AccessTokenTypeKey = "token_type_key";
        private const string AccessTokenKey = "token_key";
        //private const string TokenKey = "token_key";
        //private const string TokenExpirationDateKey = "token_expiration_key";

        private const string EnumeratorIdKey = "enumerator_id_key";

        private const string AttemptedPushKey = "attempted_push";

        private const string DatabaseIdKey = "azure_database";

        private const string EmailKey = "email_key";

        private const string FavoriteModeKey = "favorites_only";

        private const string FirstNameKey = "firstname_key";

        private const string FirstRunKey = "first_run";

        private const string GooglePlayCheckedKey = "play_checked";

        private const string HasSetReminderKey = "set_a_reminder";

        private const bool HasSyncedDataDefault = false;

        private const string HasSyncedDataKey = "has_synced_down";
        private const string HasSyncedDataUpwardsKey = "has_synced_upwards";
        private const string HasSyncedDataDownwardsKey = "has_synced_downwards";

        private const string LastFavoriteTimeKey = "last_favorite_time";

        private const string LastNameKey = "lastname_key";

        private const string LastSyncKey = "last_sync";
        private const string LastSyncDownKey = "last_sync_down";
        private const string LastSyncUpKey = "last_sync_up";

        private const string LoggedInKey = "logged_in";

        private const int LoginAttemptsDefault = 0;

        private const string LoginAttemptsKey = "login_attempts";

        private const bool NeedsSyncDefault = true;

        private const string NeedsSyncKey = "needs_sync";

        private const string PasswordKey = "password_key";

        private const string PositionKey = "position_key";

        private const string PushNotificationsEnabledKey = "push_enabled";

        private const string PushRegisteredKey = "push_registered";

        private static readonly bool AttemptedPushDefault = false;

        private static readonly int DatabaseIdDefault = 0;
        private static readonly int EnumeratorIdDefault = 0;

        private static readonly bool FavoriteModeDefault = false;

        private static readonly bool FirstRunDefault = true;

        private static readonly bool GooglePlayCheckedDefault = false;

        private static readonly bool HasSetReminderDefault = false;

        private static readonly DateTime LastSyncDefault = DateTime.Now.AddDays(-30);

        private static readonly bool LoggedInDefault = true;

        private static readonly bool PushNotificationsEnabledDefault = false;

        private static readonly bool PushRegisteredDefault = false;

        private static Settings settings;

        private readonly string emailDefault = "demo@system.com";
        private readonly string passwordDefault = "password";

        private readonly string firstNameDefault = string.Empty;

        private readonly string lastNameDefault = string.Empty;

        private readonly string positionDefault = string.Empty;

        private readonly string tokenDefault = string.Empty;

        //private readonly string access_token = string.Empty;

        //private readonly string token_expires = string.Empty;

        //private readonly string token_issued = string.Empty;

        //private const string ShowAllCategoriesKey = "all_categories";
        //private static readonly bool ShowAllCategoriesDefault = true;

        //public bool ShowAllCategories
        //{
        //    get { return AppSettings.GetValueOrDefault(ShowAllCategoriesKey, ShowAllCategoriesDefault); }
        //    set
        //    {
        //        if (AppSettings.AddOrUpdateValue(ShowAllCategoriesKey, value))
        //            OnPropertyChanged();
        //    }
        //}

        //private const string FilteredCategoriesKey = "filtered_categories";
        //private static readonly string FilteredCategoriesDefault = string.Empty;

        //public string FilteredCategories
        //{
        //    get { return AppSettings.GetValueOrDefault(FilteredCategoriesKey, FilteredCategoriesDefault); }
        //    set
        //    {
        //        if (AppSettings.AddOrUpdateValue(FilteredCategoriesKey, value))
        //            OnPropertyChanged();
        //    }
        //}

        //private const string ShowPastRegistrationsKey = "show_past_Registrations";
        //private static readonly bool ShowPastRegistrationsDefault = false;
        //public static readonly DateTime EndOfRegistration = new DateTime(2016, 4, 29, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Gets or sets a value indicating whether the user wants show past Registrations.
        /// </summary>
        /// <value><c>true</c> if show past Registrations; otherwise, <c>false</c>.</value>
        //public bool ShowPastRegistrations
        //{
        //    get
        //    {
        //        //if end of conference
        //        if (DateTime.UtcNow > EndOfRegistration)
        //            return true;

        //        return AppSettings.GetValueOrDefault(ShowPastRegistrationsKey, ShowPastRegistrationsDefault);
        //    }
        //    set
        //    {
        //        if (AppSettings.AddOrUpdateValue(ShowPastRegistrationsKey, value))
        //            OnPropertyChanged();
        //    }
        //}

        private bool isConnected;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the current settings. This should always be used
        /// </summary>
        /// <value>The current.</value>
        public static Settings Current
        {
            get
            {
                return settings ?? (settings = new Settings());
            }
        }

        public static int DatabaseId
        {
            get
            {
                return AppSettings.GetValueOrDefault(DatabaseIdKey, DatabaseIdDefault);
            }

            set
            {
                AppSettings.AddOrUpdateValue(DatabaseIdKey, value);
            }
        }

        public int EnumeratorId
        {
            get
            {
                return AppSettings.GetValueOrDefault(EnumeratorIdKey, EnumeratorIdDefault);
            }

            set
            {
                AppSettings.AddOrUpdateValue(EnumeratorIdKey, value);
            }
        }

        public DateTime AccessTokenIssuedDate
        {
            get => AppSettings.GetValueOrDefault(AccessTokenIssuedDateKey, DateTime.Now);
            set
            {
                if (AppSettings.AddOrUpdateValue(AccessTokenIssuedDateKey, value))
                {
                    OnPropertyChanged();
                }
            }
        }

        public DateTime AccessTokenExpirationDate
        {
            get => AppSettings.GetValueOrDefault(AccessTokenExpirationDateKey, DateTime.Now);
            set
            {
                if (AppSettings.AddOrUpdateValue(AccessTokenExpirationDateKey, value))
                {
                    OnPropertyChanged();
                }
            }
        }

        public string AccessToken
        {
            get => AppSettings.GetValueOrDefault(AccessTokenKey, this.tokenDefault);

            set
            {
                if (AppSettings.AddOrUpdateValue(AccessTokenKey, value))
                {
                    OnPropertyChanged();
                }
            }
        }

        public string AccessTokenType
        {
            get => AppSettings.GetValueOrDefault(AccessTokenTypeKey, this.tokenDefault);

            set
            {
                if (AppSettings.AddOrUpdateValue(AccessTokenTypeKey, value))
                {
                    OnPropertyChanged();
                }
            }
        }

        public bool AttemptedPush
        {
            get
            {
                return AppSettings.GetValueOrDefault(AttemptedPushKey, AttemptedPushDefault);
            }

            set
            {
                AppSettings.AddOrUpdateValue(AttemptedPushKey, value);
            }
        }

        public string Email
        {
            get
            {
                return AppSettings.GetValueOrDefault(EmailKey, this.emailDefault);
            }

            set
            {
                if (AppSettings.AddOrUpdateValue(EmailKey, this.emailDefault))
                {
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user wants to see favorites only.
        /// </summary>
        /// <value><c>true</c> if favorites only; otherwise, <c>false</c>.</value>
        //public bool FavoritesOnly
        //{
        //    get
        //    {
        //        return AppSettings.GetValueOrDefault(FavoriteModeKey, FavoriteModeDefault);
        //    }

        //    set
        //    {
        //        if (AppSettings.AddOrUpdateValue(FavoriteModeKey, value))
        //            OnPropertyChanged();
        //    }
        //}

        public string FirstName
        {
            get
            {
                return AppSettings.GetValueOrDefault(FirstNameKey, this.firstNameDefault);
            }

            set
            {
                if (AppSettings.AddOrUpdateValue(FirstNameKey, value))
                {
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(UserDisplayName));
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user wants to see favorites only.
        /// </summary>
        /// <value><c>true</c> if favorites only; otherwise, <c>false</c>.</value>
        public bool FirstRun
        {
            get
            {
                return AppSettings.GetValueOrDefault(FirstRunKey, FirstRunDefault);
            }

            set
            {
                if (AppSettings.AddOrUpdateValue(FirstRunKey, value))
                    OnPropertyChanged();
            }
        }

        public bool GooglePlayChecked
        {
            get
            {
                return AppSettings.GetValueOrDefault(GooglePlayCheckedKey, GooglePlayCheckedDefault);
            }

            set
            {
                AppSettings.AddOrUpdateValue(GooglePlayCheckedKey, value);
            }
        }

        public bool HasSetReminder
        {
            get
            {
                return AppSettings.GetValueOrDefault(HasSetReminderKey, HasSetReminderDefault);
            }

            set
            {
                AppSettings.AddOrUpdateValue(HasSetReminderKey, value);
            }
        }

        public bool HasSyncedDataDownwards
        {
            get
            {
                return AppSettings.GetValueOrDefault(HasSyncedDataDownwardsKey, HasSyncedDataDefault);
            }

            set
            {
                AppSettings.AddOrUpdateValue(HasSyncedDataDownwardsKey, value);
            }
        }

        public bool HasSyncedDataUpwards
        {
            get
            {
                return AppSettings.GetValueOrDefault(HasSyncedDataUpwardsKey, HasSyncedDataDefault);
            }

            set
            {
                AppSettings.AddOrUpdateValue(HasSyncedDataUpwardsKey, value);
            }
        }

        public bool IsConnected
        {
            get
            {
                return isConnected;
            }

            set
            {
                if (isConnected == value)
                    return;
                isConnected = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoggedIn => !string.IsNullOrWhiteSpace(Email);

        public DateTime LastFavoriteTime
        {
            get => AppSettings.GetValueOrDefault(LastFavoriteTimeKey, DateTime.UtcNow);
            set
            {
                AppSettings.AddOrUpdateValue(LastFavoriteTimeKey, value);
            }
        }

        public string LastName
        {
            get
            {
                return AppSettings.GetValueOrDefault(LastNameKey, this.lastNameDefault);
            }

            set
            {
                if (AppSettings.AddOrUpdateValue(LastNameKey, value))
                {
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(UserDisplayName));
                }
            }
        }

        public DateTime LastSyncDown
        {
            get
            {
                return AppSettings.GetValueOrDefault(LastSyncDownKey, LastSyncDefault);
            }

            set
            {
                if (AppSettings.AddOrUpdateValue(LastSyncDownKey, value))
                    OnPropertyChanged();
            }
        }

        public DateTime LastSync
        {
            get
            {
                return AppSettings.GetValueOrDefault(LastSyncKey, LastSyncDefault);
            }

            set
            {
                if (AppSettings.AddOrUpdateValue(LastSyncKey, value))
                    OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user wants to see favorites only.
        /// </summary>
        /// <value><c>true</c> if favorites only; otherwise, <c>false</c>.</value>
        public bool LoggedIn
        {
            get
            {
                return AppSettings.GetValueOrDefault(LoggedInKey, LoggedInDefault);
            }

            set
            {
                if (AppSettings.AddOrUpdateValue(LoggedInKey, value))
                    OnPropertyChanged();
            }
        }

        public int LoginAttempts
        {
            get
            {
                return AppSettings.GetValueOrDefault(LoginAttemptsKey, LoginAttemptsDefault);
            }

            set
            {
                AppSettings.AddOrUpdateValue(LoginAttemptsKey, value);
            }
        }

        public bool NeedsSync
        {
            get
            {
                return AppSettings.GetValueOrDefault(NeedsSyncKey, NeedsSyncDefault)
                       || LastSync < DateTime.Now.AddDays(-1);
            }

            set
            {
                AppSettings.AddOrUpdateValue(NeedsSyncKey, value);
            }
        }

        public string Password
        {
            get
            {
                return AppSettings.GetValueOrDefault(PasswordKey, this.passwordDefault);
            }

            set
            {
                if (AppSettings.AddOrUpdateValue(PasswordKey, value))
                {
                    // OnPropertyChanged();
                    // OnPropertyChanged(nameof(UserAvatar));
                }
            }
        }

        public string Position
        {
            get
            {
                return AppSettings.GetValueOrDefault(PositionKey, this.positionDefault);
            }

            set
            {
                if (AppSettings.AddOrUpdateValue(PositionKey, value))
                {
                    OnPropertyChanged();
                }
            }
        }

        public bool PushNotificationsEnabled
        {
            get
            {
                return AppSettings.GetValueOrDefault(PushNotificationsEnabledKey, PushNotificationsEnabledDefault);
            }

            set
            {
                if (AppSettings.AddOrUpdateValue(PushNotificationsEnabledKey, value))
                    OnPropertyChanged();
            }
        }

        public bool PushRegistered
        {
            get
            {
                return AppSettings.GetValueOrDefault(PushRegisteredKey, PushRegisteredDefault);
            }

            set
            {
                AppSettings.AddOrUpdateValue(PushRegisteredKey, value);
            }
        }

      //  public string UserAvatar => IsLoggedIn ? Gravatar.GetUrl(Email) : "profile_generic.png";

        public string UserDisplayName => IsLoggedIn ? $"{FirstName} {LastName}" : "Sign In";

        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        public static int UpdateDatabaseId()
        {
            return DatabaseId++;
        }

        public void FinishHack(string id)
        {
            AppSettings.AddOrUpdateValue("minihack_" + id, true);
        }

        public bool IsHackFinished(string id)
        {
            return AppSettings.GetValueOrDefault("minihack_" + id, false);
        }

        private void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
