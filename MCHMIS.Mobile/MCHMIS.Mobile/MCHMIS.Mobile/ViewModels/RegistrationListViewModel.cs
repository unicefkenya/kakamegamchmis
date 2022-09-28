using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MCHMIS.Mobile.Database;
using MCHMIS.Mobile.Pages;
using MvvmHelpers;
using Xamarin.Forms;

namespace MCHMIS.Mobile.ViewModels
{
    public class RegistrationListViewModel : LocalBaseViewModel
    {
       // public INavigation Navigation;

        public RegistrationListViewModel(INavigation navigation) : base(navigation)
        {
            PendingInterview = App.Database.SystemCodeDetailGetByCode("Interview Status", "Pending").Id;
            OngoingInterview = App.Database.SystemCodeDetailGetByCode("Interview Status", "Ongoing").Id;
            CompleteInterview = App.Database.SystemCodeDetailGetByCode("Interview Status", "Completed").Id;


            DownloadedRegistrations.ReplaceRange(GetDownloadedRegistrations());
            OngoingRegistrations.ReplaceRange(GetOngoingRegistrations());
            CompleteRegistrations.ReplaceRange(GetCompleteRegistrations());
          //  Navigation = navigation;

            }

        public int PendingInterview;
        public int OngoingInterview;
        public int CompleteInterview;

        public ObservableCollection<Registration> GetCompleteRegistrations()
        {
            var items = App.Database.GetTable("Registration");
            var hh = new ObservableCollection<Registration>();
            foreach (var item in items)
            {
                var hhitem = (Registration)item;
                if (hhitem.InterviewStatusId == CompleteInterview)
                    hh.Add(hhitem);
            }
            return hh;
        }

        public ObservableCollection<Registration> GetOngoingRegistrations()
        {
            var items = App.Database.GetTable("Registration");
            var hh = new ObservableCollection<Registration>();
            foreach (var item in items)
            {
                var hhitem = (Registration)item;
                if (hhitem.InterviewStatusId == OngoingInterview)
                    hh.Add(hhitem);
            }
            return hh;
        }

        public ObservableCollection<Registration> GetDownloadedRegistrations()
        {
            var items = App.Database.GetTable("Registration");
            var hh = new ObservableCollection<Registration>();
            foreach (var item in items)
            {
                var hhitem = (Registration)item;
                if (hhitem.InterviewStatusId == PendingInterview)
                    hh.Add(hhitem);
            }
            return hh;
        }

        public ObservableRangeCollection<Registration> CompleteRegistrations { get; } = new ObservableRangeCollection<Registration>();
        public ObservableRangeCollection<Registration> OngoingRegistrations { get; } = new ObservableRangeCollection<Registration>();
        public ObservableRangeCollection<Registration> DownloadedRegistrations { get; } = new ObservableRangeCollection<Registration>();











        private ICommand forceRefreshDownloadedCommand;

        public ICommand ForceRefreshDownloadedCommand => forceRefreshDownloadedCommand ?? (forceRefreshDownloadedCommand = new Command(async () => await ExecuteForceRefreshDownloadedCommandAsync()));

        private async Task ExecuteForceRefreshDownloadedCommandAsync()
        {
            ExecuteCompleteRegistrations();
        }
        private void ExecuteCompleteRegistrations()
        {
            IsBusy = true;
            if (!string.IsNullOrEmpty(Filter))
            {
                var loaded = GetCompleteRegistrations().Where(x => x.Haystack.ToLower().Contains(filter.ToLower()));
                CompleteRegistrations.ReplaceRange(loaded);
            }
            else
            {
                CompleteRegistrations.ReplaceRange(GetCompleteRegistrations());
            }

            IsBusy = false;
        }


        private ICommand forceRefreshOngoingCommand;

        public ICommand ForceRefreshOngoingCommand => forceRefreshOngoingCommand ?? (forceRefreshOngoingCommand = new Command(async () => await ExecuteForceRefreshOngoingCommandAsync()));

        private async Task ExecuteForceRefreshOngoingCommandAsync()
        {
            ExecuteOngoingRegistrations();
        }
        private void ExecuteOngoingRegistrations()
        {
            IsBusy = true;
            if (!string.IsNullOrEmpty(Filter))
            {
                var loaded = GetOngoingRegistrations().Where(x => x.Haystack.ToLower().Contains(filter.ToLower()));
                OngoingRegistrations.ReplaceRange(loaded);
            }
            else
            {
                OngoingRegistrations.ReplaceRange(GetOngoingRegistrations());
            }

            IsBusy = false;
        }

        private ICommand forceRefreshCompleteCommand;

        public ICommand ForceRefreshCompleteCommand => forceRefreshCompleteCommand ?? (forceRefreshCompleteCommand = new Command(async () => await ExecuteForceRefreshCompleteCommandAsync()));

        private async Task ExecuteForceRefreshCompleteCommandAsync()
        {
            ExecuteDownloadedRegistrations();
        }




        private void ExecuteDownloadedRegistrations()
        {
            IsBusy = true;
            if (!string.IsNullOrEmpty(Filter))
            {
                var loaded = GetDownloadedRegistrations().Where(x => x.Haystack.ToLower().Contains(filter.ToLower()));
                DownloadedRegistrations.ReplaceRange(loaded);
            }
            else
            {
                DownloadedRegistrations.ReplaceRange(GetDownloadedRegistrations());
            }

            IsBusy = false;
        }


        #region Properties

        private Registration selectedRegistration;

        public Registration SelectedRegistration
        {
            get { return selectedRegistration; }
            set
            {
                selectedRegistration = value;
                OnPropertyChanged();
                if (selectedRegistration == null)
                    return;
                Navigation.PushAsync(new RegistrationDetailPage(selectedRegistration.Id));

                SelectedRegistration = null;
            }
        }



        #endregion Properties

        #region Filtering and Sorting



        private bool noRegistrationsFound;

        public bool NoRegistrationsFound
        {
            get { return noRegistrationsFound; }
            set { SetProperty(ref noRegistrationsFound, value); }
        }

        private string noRegistrationsFoundMessage;

        public string NoRegistrationsFoundMessage
        {
            get { return noRegistrationsFoundMessage; }
            set { SetProperty(ref noRegistrationsFoundMessage, value); }
        }

        #endregion Filtering and Sorting

        #region Commands








        #endregion Commands

    //    private readonly INavigation navigation;

        #region Properties

        private Registration selectedDownloaded;

        public Registration SelectedDownloaded
        {
            get { return selectedDownloaded; }
            set
            {
                selectedDownloaded = value;
                OnPropertyChanged();
                if (selectedDownloaded != null)
                {
                    Navigation.PushAsync(new RegistrationDetailPage(selectedDownloaded.Id));
                    return;
                }
            }
        }

        private Registration selectedOngoing;

        public Registration SelectedOngoing
        {
            get { return selectedOngoing; }
            set
            {
                selectedOngoing = value;
                OnPropertyChanged();
                if (selectedOngoing != null)
                {
                    Navigation.PushAsync(new RegistrationDetailPage(selectedOngoing.Id));
                    return;
                }
            }
        }

        private Registration selectedComplete;

        public Registration SelectedComplete
        {
            get { return selectedComplete; }
            set
            {
                selectedComplete = value;
                OnPropertyChanged();
                if (selectedComplete != null)
                {
                    Navigation.PushAsync(new RegistrationDetailPage(selectedOngoing.Id));
                    return;
                }
            }
        }

        #endregion Properties
    }


}
