using FluentValidation;
using FormsToolkit;
using MCHMIS.Mobile.Converters;
using MCHMIS.Mobile.Database;
using MCHMIS.Mobile.Models;
using MCHMIS.Mobile.Validators;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MCHMIS.Mobile.Pages;
using Org.BouncyCastle.Bcpg;
using Xamarin.Forms;

namespace MCHMIS.Mobile.ViewModels
{
    public class RegistrationMemberViewModel : LocalBaseViewModel
    {
        public ObservableRangeCollection<SelectableItemWrapper<SystemCodeDetail>> _loadedDisabilities =
            new ObservableRangeCollection<SelectableItemWrapper<SystemCodeDetail>>();

        public ObservableRangeCollection<RegistrationMember> CareGiver = new ObservableRangeCollection<RegistrationMember>();
        public ObservableRangeCollection<SystemCodeDetail> ChronicIllnessOption = new ObservableRangeCollection<SystemCodeDetail>();
        public ObservableRangeCollection<SystemCodeDetail> DisabilityCareStatus = new ObservableRangeCollection<SystemCodeDetail>();

        public ObservableRangeCollection<SystemCodeDetail> DisabilityType = new ObservableRangeCollection<SystemCodeDetail>();

        public ObservableRangeCollection<SystemCodeDetail> EducationLevel = new ObservableRangeCollection<SystemCodeDetail>();
        public ObservableRangeCollection<SystemCodeDetail> FatherAliveOption = new ObservableRangeCollection<SystemCodeDetail>();

        public DateTime fiveYearsAgo;
        public ObservableRangeCollection<SystemCodeDetail> IdentificationDocumentType = new ObservableRangeCollection<SystemCodeDetail>();

        public ObservableRangeCollection<SystemCodeDetail> JobOption = new ObservableRangeCollection<SystemCodeDetail>();
        public ObservableRangeCollection<SystemCodeDetail> LearningStatus = new ObservableRangeCollection<SystemCodeDetail>();
        public ObservableRangeCollection<SystemCodeDetail> SchoolType = new ObservableRangeCollection<SystemCodeDetail>();

        public ObservableRangeCollection<SystemCodeDetail> MaritalStatus = new ObservableRangeCollection<SystemCodeDetail>();

        public ObservableRangeCollection<SystemCodeDetail> MotherAliveOption = new ObservableRangeCollection<SystemCodeDetail>();

        public ObservableRangeCollection<SystemCodeDetail> Relationship = new ObservableRangeCollection<SystemCodeDetail>();

        public DateTime seventeenYearsAgo;
        public ObservableRangeCollection<SystemCodeDetail> Sex = new ObservableRangeCollection<SystemCodeDetail>();
        public DateTime seventyYearsAgo;
        public ObservableRangeCollection<RegistrationMember> Spouse = new ObservableRangeCollection<RegistrationMember>();
        public ObservableRangeCollection<SystemCodeDetail> SpouseInHousehold = new ObservableRangeCollection<SystemCodeDetail>();
        public DateTime threeYearsAgo;
        public ObservableRangeCollection<SystemCodeDetail> WorkType = new ObservableRangeCollection<SystemCodeDetail>();

        private readonly IValidator _validator;

        private ICommand _saveMemberCommand;
        private RegistrationMember _selectedCg;
        private SystemCodeDetail _selectedChronicIllnessOption;

        private SystemCodeDetail _selectedDisabilityCareStatus;

        private SystemCodeDetail _selectedDisabilityType;

        private SystemCodeDetail _selectedEducationLevel;
        private SystemCodeDetail _selectedFatherAliveOption;

        private SystemCodeDetail _selectedIdentificationDocumentType;

        private SystemCodeDetail _selectedJobOption;
        private SystemCodeDetail _selectedLearningStatus;

        private SystemCodeDetail _selectedMaritalStatus;
        private SystemCodeDetail _selectedSchoolType;

        private SystemCodeDetail _selectedMotherAliveOption;

        private SystemCodeDetail _selectedRelationship;

        private SystemCodeDetail _selectedSex;

        private RegistrationMember _selectedSpouse;
        private SystemCodeDetail _selectedSpouseInHousehold;
        private SystemCodeDetail _selectedWorkType;

        private bool isDisabled;
        private bool isDocument;

        private bool isMarried;

        private bool isNeedCare;

        private bool isNotDocument;

        private bool isNotMarried;

        private bool isNotNeedCare;

        private bool isNotOver3;
        private bool isNotOver5;
        private DateTime dateOfBirth;

        private bool isNotSpouseInHH;

        private bool isOver3;

        private bool isOver5;

        private bool isBetween17And70;

        private bool isSpouseInHH;

        private bool wentToSchool;

        public RegistrationMemberViewModel(INavigation navigation, int hhId, string memberId) : base(navigation)
        {
            var today = DateTime.Now;
            threeYearsAgo = new DateTime(today.Year - 3, today.Month, today.Day);
            fiveYearsAgo = new DateTime(today.Year - 3, today.Month, today.Day);
            seventyYearsAgo = new DateTime(today.Year - 70, today.Month, today.Day);
            seventeenYearsAgo = new DateTime(today.Year - 17, today.Month, today.Day);

            if (!string.IsNullOrEmpty(memberId))
            {
                var data = App.Database.GetTableRow<RegistrationMember>("RegistrationMember", "Id", memberId);
                Member = data;
                DateOfBirth = Member.DateOfBirth;
            }
            else
            {
                Member = new RegistrationMember
                {
                    RegistrationId = hhId,
                    MemberId = Guid.NewGuid().ToString(),
                    DateOfBirth = DateTime.Now,
                    DateOfBirthDate = DateFormatter.ToSQLiteDateTimeString(DateTime.Now)
                };
                DateOfBirth = DateTime.Now;
            }
            DateOfBirth = Member.DateOfBirth;
            var sex = App.Database.SystemCodeDetailsGetByCode("Gender");
            var booleanOptions = App.Database.SystemCodeDetailsGetByCode("Household Option").Where(x => x.Code != "Don't Know");
            var aliveOptions = App.Database.SystemCodeDetailsGetByCode("Household Option");
            var members = App.Database.GetTableRows<RegistrationMember>("RegistrationMember", "RegistrationId", hhId.ToString());

            LoadedIdentificationDocumentTypes.AddRange(App.Database.SystemCodeDetailsGetByCode("Identification Documents"));
            LoadedRelationships.AddRange(App.Database.SystemCodeDetailsGetByCode("Relationship"));
            LoadedSexs.AddRange(sex);
            LoadedMaritalStatuses.AddRange(App.Database.SystemCodeDetailsGetByCode("Marital Status"));
            LoadedSpouseInHouseholds.AddRange(aliveOptions);
            LoadedSpouses.AddRange(members);
            LoadedCaregivers.AddRange(members);
            LoadedFatherAliveOptions.AddRange(aliveOptions);
            LoadedMotherAliveOptions.AddRange(aliveOptions);
            LoadedChronicIllnessOptions.AddRange(aliveOptions);
            LoadedDisabilityTypes.AddRange(booleanOptions);
            LoadedDisabilityCareStatuses.AddRange(aliveOptions);
            LoadedLearningStatuses.AddRange(App.Database.SystemCodeDetailsGetByCode("Education Attendance"));
            LoadedSchoolTypes.AddRange(App.Database.SystemCodeDetailsGetByCode("School Type"));

            LoadedEducationLevels.AddRange(App.Database.SystemCodeDetailsGetByCode("Education Level"));
            LoadedWorkTypes.AddRange(App.Database.SystemCodeDetailsGetByCode("Work Type"));
            LoadedJobOptions.AddRange(aliveOptions);

            SelectedIdentificationDocumentType = LoadedIdentificationDocumentTypes.SingleOrDefault(x => x.Id == Member.IdentificationDocumentTypeId);
            SelectedRelationship = LoadedRelationships.SingleOrDefault(x => x.Id == Member.RelationshipId);
            SelectedSex = LoadedSexs.SingleOrDefault(x => x.Id == Member.SexId);
            SelectedMaritalStatus = LoadedMaritalStatuses.SingleOrDefault(x => x.Id == Member.MaritalStatusId);
            SelectedSpouseInHousehold = LoadedSpouseInHouseholds.SingleOrDefault(x => x.Id == Member.SpouseInHouseholdId);
            SelectedSpouse = LoadedSpouses.SingleOrDefault(x => x.MemberId == Member.SpouseId);
            SelectedCg = LoadedCaregivers.SingleOrDefault(x => x.MemberId == Member.CareGiverId);

            SelectedFatherAliveOption = LoadedFatherAliveOptions.SingleOrDefault(x => x.Id == Member.FatherAliveStatusId);
            SelectedMotherAliveOption = LoadedMotherAliveOptions.SingleOrDefault(x => x.Id == Member.MotherAliveStatusId);
            SelectedChronicIllnessOption = LoadedChronicIllnessOptions.SingleOrDefault(x => x.Id == Member.ChronicIllnessStatusId);
            SelectedDisabilityType = LoadedDisabilityTypes.SingleOrDefault(x => x.Id == Member.DisabilityTypeId);
            SelectedDisabilityCareStatus = LoadedDisabilityCareStatuses.SingleOrDefault(x => x.Id == Member.DisabilityCareStatusId);
            SelectedLearningStatus = LoadedLearningStatuses.SingleOrDefault(x => x.Id == Member.LearningStatusId);
            SelectedEducationLevel = LoadedEducationLevels.SingleOrDefault(x => x.Id == Member.EducationLevelId);
            SelectedWorkType = LoadedWorkTypes.SingleOrDefault(x => x.Id == Member.WorkTypeId);
            SelectedSchoolType = LoadedSchoolTypes.SingleOrDefault(x => x.Id == Member.SchoolTypeId);
            SelectedJobOption = LoadedJobOptions.SingleOrDefault(x => x.Id == Member.FormalJobNgoId);

            IsIprsed = Member.IPRSed;
            IsNotIprsed = !Member.IPRSed;

            var disabilities = App.Database.SystemCodeDetailsGetByCode("Disability").ToList();

            var disabilityTypes = App.Database.GetTableRows<RegistrationMemberDisability>("RegistrationMemberDisability", "RegistrationMemberId", Member.MemberId);
            var list = new List<SelectableItemWrapper<SystemCodeDetail>>();

            if (!disabilityTypes.Any())
            {
                foreach (var item in disabilities)
                {
                    list.Add(new SelectableItemWrapper<SystemCodeDetail> { Item = item, IsSelected = false });
                }
            }
            else
            {
                foreach (var item in disabilities)
                {
                    list.Add(new SelectableItemWrapper<SystemCodeDetail>
                    { Item = item, IsSelected = disabilityTypes.Any(x => x.DisabilityId == item.Id) });
                }
            }

            LoadedDisabilities.AddRange(list);

            _validator = new RegistrationMemberValidator();
        }

        public DateTime DateOfBirth
        {
            get { return dateOfBirth; }
            set
            {
                if (this.dateOfBirth == value)
                {
                    return;
                }

                dateOfBirth = value;
                var dob = dateOfBirth;
                IsOver5 = dob < fiveYearsAgo;
                IsOver3 = dob < threeYearsAgo;
                IsBetween17And70 = dob < seventeenYearsAgo && dob > seventyYearsAgo;
                this.OnPropertyChanged();
            }
        }

        public bool IsIprsed
        {
            get { return isIprsed; }
            set { SetProperty(ref isIprsed, value); }
        }

        private bool isIprsed;

        private bool isNotIprsed;

        public bool IsNotIprsed
        {
            get => isNotIprsed;
            set { SetProperty(ref isNotIprsed, value); }
        }

        public bool IsDisabled
        {
            get { return isDisabled; }
            set { SetProperty(ref isDisabled, value); }
        }

        public bool WentToSchool
        {
            get { return wentToSchool; }
            set { SetProperty(ref wentToSchool, value); }
        }

        public bool IsDocument
        {
            get { return isDocument; }
            set { SetProperty(ref isDocument, value); }
        }

        public bool IsMarried
        {
            get { return isMarried; }
            set { SetProperty(ref isMarried, value); }
        }

        public bool IsNeedCare
        {
            get { return isNeedCare; }
            set { SetProperty(ref isNeedCare, value); }
        }

        public bool IsNotDocument
        {
            get => isNotDocument;
            set
            {
                if (SetProperty(ref isNotDocument, value))
                {
                    isDocument = !isNotDocument;
                }
            }
        }

        public bool IsNotMarried
        {
            get => isNotMarried;
            set
            {
                if (SetProperty(ref isNotMarried, value))
                {
                    isMarried = !isNotMarried;
                }
            }
        }

        public bool IsNotNeedCare
        {
            get => isNotNeedCare;
            set
            {
                if (SetProperty(ref isNotNeedCare, value))
                {
                    isNeedCare = !isNotNeedCare;
                }
            }
        }

        public bool IsNotOver3
        {
            get => isNotOver3;
            set
            {
                if (SetProperty(ref isNotOver3, value))
                {
                    IsOver3 = !isNotOver3;
                }
            }
        }

        public bool IsNotOver5
        {
            get => isNotOver5;
            set
            {
                if (SetProperty(ref isNotOver5, value))
                {
                    isOver5 = !isNotOver5;
                }
            }
        }

        public bool IsNotSpouseInHH
        {
            get => isNotSpouseInHH;
            set
            {
                if (SetProperty(ref isNotSpouseInHH, value))
                {
                    isSpouseInHH = !isNotSpouseInHH;
                }
            }
        }

        public bool IsOver3
        {
            get { return isOver3; }
            set { SetProperty(ref isOver3, value); }
        }

        public bool IsBetween17And70
        {
            get { return isBetween17And70; }
            set { SetProperty(ref isBetween17And70, value); }
        }

        public bool IsOver5
        {
            get { return isOver5; }
            set { SetProperty(ref isOver5, value); }
        }

        public bool IsSpouseInHH
        {
            get { return isSpouseInHH; }
            set { SetProperty(ref isSpouseInHH, value); }
        }

        public ObservableRangeCollection<RegistrationMember> LoadedCaregivers
        {
            get => CareGiver;
            set => SetProperty(ref CareGiver, value);
        }

        public ObservableRangeCollection<SystemCodeDetail> LoadedChronicIllnessOptions
        {
            get => ChronicIllnessOption;
            set => SetProperty(ref ChronicIllnessOption, value);
        }

        public ObservableRangeCollection<SelectableItemWrapper<SystemCodeDetail>> LoadedDisabilities
        {
            get { return _loadedDisabilities; }
            set => SetProperty(ref _loadedDisabilities, value);
        }

        public ObservableRangeCollection<SystemCodeDetail> LoadedDisabilityCareStatuses
        {
            get => DisabilityCareStatus;
            set => SetProperty(ref DisabilityCareStatus, value);
        }

        public ObservableRangeCollection<SystemCodeDetail> LoadedDisabilityTypes
        {
            get => DisabilityType;
            set => SetProperty(ref DisabilityType, value);
        }

        public ObservableRangeCollection<SystemCodeDetail> LoadedEducationLevels
        {
            get => EducationLevel;
            set => SetProperty(ref EducationLevel, value);
        }

        public ObservableRangeCollection<SystemCodeDetail> LoadedFatherAliveOptions
        {
            get => FatherAliveOption;
            set => SetProperty(ref FatherAliveOption, value);
        }

        public ObservableRangeCollection<SystemCodeDetail> LoadedIdentificationDocumentTypes
        {
            get => IdentificationDocumentType;
            set => SetProperty(ref IdentificationDocumentType, value);
        }

        public ObservableRangeCollection<SystemCodeDetail> LoadedJobOptions
        {
            get => JobOption;
            set => SetProperty(ref JobOption, value);
        }

        public ObservableRangeCollection<SystemCodeDetail> LoadedLearningStatuses
        {
            get => LearningStatus;
            set => SetProperty(ref LearningStatus, value);
        }

        public ObservableRangeCollection<SystemCodeDetail> LoadedSchoolTypes
        {
            get => SchoolType;
            set => SetProperty(ref SchoolType, value);
        }

        public ObservableRangeCollection<SystemCodeDetail> LoadedMaritalStatuses
        {
            get => MaritalStatus;
            set => SetProperty(ref MaritalStatus, value);
        }

        public ObservableRangeCollection<SystemCodeDetail> LoadedMotherAliveOptions
        {
            get => MotherAliveOption;
            set => SetProperty(ref MotherAliveOption, value);
        }

        public ObservableRangeCollection<SystemCodeDetail> LoadedRelationships
        {
            get => Relationship;
            set => SetProperty(ref Relationship, value);
        }

        public ObservableRangeCollection<SystemCodeDetail> LoadedSexs
        {
            get => Sex;
            set => SetProperty(ref Sex, value);
        }

        public ObservableRangeCollection<SystemCodeDetail> LoadedSpouseInHouseholds
        {
            get => SpouseInHousehold;
            set => SetProperty(ref SpouseInHousehold, value);
        }

        public ObservableRangeCollection<RegistrationMember> LoadedSpouses
        {
            get => Spouse;
            set => SetProperty(ref Spouse, value);
        }

        public ObservableRangeCollection<SystemCodeDetail> LoadedWorkTypes
        {
            get => WorkType;
            set => SetProperty(ref WorkType, value);
        }

        public RegistrationMember Member { get; set; }
        public ICommand SaveMemberCommand => _saveMemberCommand ?? (_saveMemberCommand = new Command(async () => await ExecuteSaveMember()));

        public RegistrationMember SelectedCg
        {
            get => _selectedCg;
            set
            {
                if (this._selectedCg == value)
                {
                    return;
                }

                this._selectedCg = value;
                this.OnPropertyChanged();
            }
        }

        public SystemCodeDetail SelectedChronicIllnessOption
        {
            get => _selectedChronicIllnessOption;
            set
            {
                if (this._selectedChronicIllnessOption == value)
                {
                    return;
                }

                this._selectedChronicIllnessOption = value;

                this.OnPropertyChanged();
            }
        }

        public SystemCodeDetail SelectedDisabilityCareStatus
        {
            get => _selectedDisabilityCareStatus;
            set
            {
                if (this._selectedDisabilityCareStatus == value)
                {
                    return;
                }

                this._selectedDisabilityCareStatus = value;
                if (Member != null)
                {
                    var date = dateOfBirth;
                    IsNeedCare = _selectedDisabilityCareStatus.Code == "Yes";
                    ;// && (date > seventyYearsAgo || date < seventeenYearsAgo);
                }

                this.OnPropertyChanged();
            }
        }

        public SystemCodeDetail SelectedDisabilityType
        {
            get => _selectedDisabilityType;
            set
            {
                if (this._selectedDisabilityType == value)
                {
                    return;
                }

                this._selectedDisabilityType = value;
                IsDisabled = _selectedDisabilityType.Code == "Yes";
                this.OnPropertyChanged();
            }
        }

        public SystemCodeDetail SelectedEducationLevel
        {
            get => _selectedEducationLevel;
            set
            {
                if (this._selectedEducationLevel == value)
                {
                    return;
                }

                this._selectedEducationLevel = value;
                this.OnPropertyChanged();
            }
        }

        public SystemCodeDetail SelectedFatherAliveOption
        {
            get => _selectedFatherAliveOption;
            set
            {
                if (this._selectedFatherAliveOption == value)
                {
                    return;
                }

                this._selectedFatherAliveOption = value;

                this.OnPropertyChanged();
            }
        }

        public SystemCodeDetail SelectedSchoolType
        {
            get => _selectedSchoolType;
            set
            {
                if (this._selectedSchoolType == value)
                {
                    return;
                }

                this._selectedSchoolType = value;

                this.OnPropertyChanged();
            }
        }

        public SystemCodeDetail SelectedIdentificationDocumentType
        {
            get => _selectedIdentificationDocumentType;
            set
            {
                if (this._selectedIdentificationDocumentType == value)
                {
                    return;
                }

                this._selectedIdentificationDocumentType = value;
                IsDocument = _selectedIdentificationDocumentType.Code != "None";
                this.OnPropertyChanged();
            }
        }

        public SystemCodeDetail SelectedJobOption
        {
            get => _selectedJobOption;
            set
            {
                if (this._selectedJobOption == value)
                {
                    return;
                }

                this._selectedJobOption = value;
                this.OnPropertyChanged();
            }
        }

        public SystemCodeDetail SelectedLearningStatus
        {
            get => _selectedLearningStatus;
            set
            {
                try
                {
                    WentToSchool = value.Id != 141 && value.Id != 193;
                }
                catch (Exception ex)
                {
                }

                if (this._selectedLearningStatus == value)
                {
                    return;
                }

                this._selectedLearningStatus = value;
                this.OnPropertyChanged();
            }
        }

        public SystemCodeDetail SelectedMaritalStatus
        {
            get => _selectedMaritalStatus;
            set
            {
                if (this._selectedMaritalStatus == value)
                {
                    return;
                }

                this._selectedMaritalStatus = value;

                var possibleCodes = "Married Monogamous,Married Polygamous";
                IsMarried = possibleCodes.Contains(_selectedMaritalStatus.Code);

                this.OnPropertyChanged();
            }
        }

        public SystemCodeDetail SelectedMotherAliveOption
        {
            get => _selectedMotherAliveOption;
            set
            {
                if (this._selectedMotherAliveOption == value)
                {
                    return;
                }

                this._selectedMotherAliveOption = value;
                this.OnPropertyChanged();
            }
        }

        public SystemCodeDetail SelectedRelationship
        {
            get => _selectedRelationship;
            set
            {
                if (this._selectedRelationship == value)
                {
                    return;
                }

                this._selectedRelationship = value;
                this.OnPropertyChanged();
            }
        }

        public SystemCodeDetail SelectedSex
        {
            get => _selectedSex;
            set
            {
                if (this._selectedSex == value)
                {
                    return;
                }

                this._selectedSex = value;
                this.OnPropertyChanged();
            }
        }

        public RegistrationMember SelectedSpouse
        {
            get => _selectedSpouse;
            set
            {
                if (this._selectedSpouse == value)
                {
                    return;
                }

                this._selectedSpouse = value;
                this.OnPropertyChanged();
            }
        }

        public SystemCodeDetail SelectedSpouseInHousehold
        {
            get => _selectedSpouseInHousehold;
            set
            {
                if (this._selectedSpouseInHousehold == value)
                {
                    return;
                }

                this._selectedSpouseInHousehold = value;
                IsSpouseInHH = _selectedSpouseInHousehold.Code == "Yes";

                this.OnPropertyChanged();
            }
        }

        public SystemCodeDetail SelectedWorkType
        {
            get => _selectedWorkType;
            set
            {
                if (this._selectedWorkType == value)
                {
                    return;
                }

                this._selectedWorkType = value;
                this.OnPropertyChanged();
            }
        }

        private async Task ExecuteSaveMember()
        {
            try
            {
                IsBusy = true;
                var member = this.Member;
                member.AddTime = DateFormatter.ToSQLiteDateTimeString(DateTime.Now);
                member.DateOfBirth = DateOfBirth;
                member.DateOfBirthDate = DateFormatter.ToSQLiteDateTimeString(member.DateOfBirth);

                Message = "Validating . ";

                var errorMessage = "";
                if (SelectedIdentificationDocumentType == null)
                {
                    errorMessage += "Identification Document Type is required (3.02)\n";
                }
                else
                {
                    member.IdentificationDocumentTypeId = SelectedIdentificationDocumentType.Id;
                }

                if (SelectedRelationship == null)
                {
                    errorMessage += "Member's relationship to the head of the household is required (3.03)\n";
                }
                else
                {
                    member.RelationshipId = SelectedRelationship.Id;
                }

                if (SelectedSex == null)
                {
                    errorMessage += "Member's sex is required (3.04)\n";
                }
                else
                {
                    member.SexId = SelectedSex.Id;
                }

                if (IsOver5 && SelectedMaritalStatus == null)
                {
                    errorMessage += "Member's marital Status is required (3.06)\n";
                }
                else if (SelectedMaritalStatus != null)
                {
                    member.MaritalStatusId = SelectedMaritalStatus.Id;
                }
                else
                {
                    member.MaritalStatusId = 125;// Never Married for kids
                }

                if (IsMarried)
                {
                    if (SelectedSpouseInHousehold == null)
                    {
                        errorMessage += "(3.07) is required.\n";
                    }
                    else
                    {
                        member.SpouseInHouseholdId = SelectedSpouseInHousehold.Id;
                    }
                }

                if (isSpouseInHH)
                {
                    if (SelectedSpouse == null)
                    {
                        errorMessage += "(3.07 B) is required.\n";
                    }
                    else
                    {
                        member.SpouseId = SelectedSpouse.MemberId;
                    }
                }

                if (SelectedFatherAliveOption == null)
                {
                    errorMessage += "(3.08) is required\n";
                }
                else
                {
                    member.FatherAliveStatusId = SelectedFatherAliveOption.Id;
                }

                if (SelectedMotherAliveOption == null)
                {
                    errorMessage += "(3.09) is required.\n";
                }
                else
                {
                    member.MotherAliveStatusId = SelectedMotherAliveOption.Id;
                }

                if (SelectedChronicIllnessOption == null)
                {
                    errorMessage += "(3.10) is required.\n";
                }
                else
                {
                    member.ChronicIllnessStatusId = SelectedChronicIllnessOption.Id;
                }

                if (SelectedDisabilityType == null)
                {
                    errorMessage += "Check (3.11)\n";
                }
                else
                {
                    member.DisabilityTypeId = SelectedDisabilityType.Id;
                }

                if (SelectedDisabilityCareStatus == null && SelectedDisabilityType != null)
                {
                    if (SelectedDisabilityType.Code == "Yes")
                    {
                        errorMessage += "Select the disabilities the members suffers from (3.12)\n";
                    }
                }
                member.DisabilityCareStatusId = SelectedDisabilityCareStatus?.Id;
                if (IsNeedCare)
                {
                    if (SelectedCg == null)
                    {
                        errorMessage += "Member's main caregiver is required. (3.13)\n";
                    }
                    else
                    {
                        member.CareGiverId = SelectedCg.MemberId;
                    }
                }
                if (IsOver3)
                {
                    if (SelectedLearningStatus == null)
                    {
                        errorMessage += "School or learning institution attendance status is required (3.14)\n";
                    }
                    else
                    {
                        member.LearningStatusId = SelectedLearningStatus.Id;

                        if (member.LearningStatusId != 141 && member.LearningStatusId != 193)
                        {
                            if (SelectedEducationLevel == null)
                            {
                                errorMessage += "Highest Std/Form/Level reached by Member is required (3.15)\n";
                            }
                            else
                            {
                                member.EducationLevelId = SelectedEducationLevel.Id;
                            }

                            if (SelectedSchoolType == null)
                            {
                                errorMessage += "Select whether the school is Public or Private (3.15 b)\n";
                            }
                            else
                            {
                                member.SchoolTypeId = SelectedSchoolType.Id;
                            }
                        }
                    }
                }
                if (IsOver3)
                {
                    if (SelectedWorkType == null)
                    {
                        errorMessage += "(3.16) is required.\n";
                    }
                    else
                    {
                        member.WorkTypeId = SelectedWorkType.Id;
                    }

                    if (SelectedJobOption == null)
                    {
                        errorMessage += "(3.16) is required.\n";
                    }
                    else
                    {
                        member.FormalJobNgoId = SelectedJobOption.Id;
                    }
                }

                var validationResult = _validator.Validate(member);

                if (validationResult.IsValid && errorMessage == "")
                {
                    Message = "Saving to Database Locally . ";
                    App.Database.AddOrUpdate(member);

                    var existingDisabilities = App.Database.GetTableRows<RegistrationMemberDisability>("RegistrationMemberDisability", "RegistrationMemberId", Member.Id.ToString());

                    if (existingDisabilities.Any())
                    {
                        foreach (var item in existingDisabilities)
                        {
                            App.Database.Delete(item);
                        }
                    }

                    var disabilities = GetDisabilities();

                    if (disabilities.Any())
                    {
                        foreach (var item in disabilities)
                        {
                            var memberDisability = new RegistrationMemberDisability
                            {
                                RegistrationMemberId = Member.MemberId,
                                DisabilityId = item.Id,
                                RegistrationId = member.RegistrationId
                            };
                            App.Database.Create(memberDisability);
                        }
                    }
                    MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.Message, new MessagingServiceAlert
                    {
                        Title = "Success",
                        Message = "Member information saved successfully.",
                        Cancel = "OK"
                    });
                    await Navigation.PopToRootAsync(true);
                    await ((MainPage)App.Current.MainPage).Detail.Navigation.PushAsync(new RegistrationDetailPage(member.RegistrationId, 1));
                }
                else
                {
                    ValidateMessage = GetErrorListFromValidationResult(validationResult).Replace(" id'", "");

                    if (errorMessage.Length > 0 || ValidateMessage.Length > 0)
                    {
                        ValidateMessage = $"{ValidateMessage}\n{errorMessage}";
                        MessagingService.Current.SendMessage<MessagingServiceAlert>(
                            MessageKeys.Error,
                            new MessagingServiceAlert
                            {
                                Title = "Please Check the Data and Try Again!!",
                                Message = ValidateMessage,
                                Cancel = "OK"
                            });
                        IsBusy = false;
                    }
                }
            }
            catch (Exception e)
            {
                MessagingService.Current.SendMessage<MessagingServiceAlert>(
                    MessageKeys.Error,
                    new MessagingServiceAlert
                    {
                        Title = "Please Correct the Data and Try Again!!",
                        Message = e.Message,
                        Cancel = "OK"
                    });
            }
            finally
            {
                Message = string.Empty;
                IsBusy = false;
            }
            return;
        }

        private ObservableCollection<SystemCodeDetail> GetDisabilities()
        {
            var selected = LoadedDisabilities
                .Where(p => p.IsSelected)
                .Select(p => p.Item)
                .ToList();
            return new ObservableCollection<SystemCodeDetail>(selected);
        }
    }
}