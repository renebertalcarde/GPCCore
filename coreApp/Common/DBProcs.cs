using coreLib.Extensions;
using Module.Core;
using Module.DB;
using Module.DB.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace coreApp
{
    public static class DBProcs
    {

        //public static void save_StakeholderInfo(tblStakeholder_Info model)
        //{
        //    using (dalDataContext context = new dalDataContext())
        //    {
        //        List<tblStakeholder_Info> infos = context.tblStakeholder_Infos.Where(x => x.InfoId == model.InfoId).ToList();

        //        if (infos.Count > 1)
        //        {
        //            tblStakeholder_Info newInfo = new tblStakeholder_Info
        //            {
        //                StakeholderId = model.StakeholderId,
        //                Email = model.Email,
        //                Home_CountryId = ModuleConstants.PHILIPPINES_COUNTRY_ID,
        //                Nationality_CountryId = ModuleConstants.PHILIPPINES_COUNTRY_ID,
        //                Perm_CountryId = ModuleConstants.PHILIPPINES_COUNTRY_ID,
        //                POB_CountryId = ModuleConstants.PHILIPPINES_COUNTRY_ID,
        //                Spouse_Employer_CountryId = ModuleConstants.PHILIPPINES_COUNTRY_ID,
        //                BIRStatus = "S"
        //            };

        //            context.tblStakeholder_Infos.InsertOnSubmit(newInfo);
        //            context.tblStakeholder_Infos.DeleteAllOnSubmit(infos);
        //            context.SubmitChanges();                    
        //        }


        //        tblStakeholder_Info Info = context.tblStakeholder_Infos.Where(x => x.InfoId == model.InfoId).SingleOrDefault();
        //        if (Info == null)
        //        {
        //            throw new Exception(ModuleConstants.ID_NOT_FOUND);
        //        }
        //        else
        //        {
        //            Info.StakeholderId = model.StakeholderId;
        //            Info.DOB = model.DOB;
        //            Info.POB_Address = model.POB_Address;
        //            Info.POB_CountryId = model.POB_CountryId;
        //            Info.POB_PostalCode = model.POB_PostalCode;
        //            Info.Gender = model.Gender;
        //            Info.CivilStatus = model.CivilStatus;
        //            Info.Nationality_CountryId = model.Nationality_CountryId;
        //            Info.Citizenship_Dual = model.Citizenship_Dual;
        //            Info.Citizenship_ByBirth = model.Citizenship_ByBirth;
        //            Info.Citizenship_ByNaturalization = model.Citizenship_ByNaturalization;
        //            Info.Citizenship_Details = model.Citizenship_Details;
        //            Info.BIRStatus = model.BIRStatus;
        //            Info.TIN = model.TIN;
        //            Info.SSS = model.SSS;
        //            Info.GSIS = model.GSIS;
        //            Info.PAGIBIG = model.PAGIBIG;
        //            Info.PHILHEALTH = model.PHILHEALTH;
        //            Info.Perm_Address = model.Perm_Address;
        //            Info.Perm_BrgyId = model.Perm_BrgyId;
        //            Info.Perm_CountryId = model.Perm_CountryId;
        //            Info.Perm_PostalCode = model.Perm_PostalCode;
        //            Info.Home_Address = model.Home_Address;
        //            Info.Home_BrgyId = model.Home_BrgyId;
        //            Info.Home_CountryId = model.Home_CountryId;
        //            Info.Home_PostalCode = model.Home_PostalCode;
        //            Info.Home_Longitude = model.Home_Longitude;
        //            Info.Home_Latitude = model.Home_Latitude;
        //            Info.Home_TelephoneNo = model.Home_TelephoneNo;
        //            Info.MobileNo = model.MobileNo;
        //            Info.Email = model.Email;
        //            Info.Height = model.Height;
        //            Info.Weight = model.Weight;
        //            Info.BloodType = model.BloodType;
        //            Info.Spouse_LastName = model.Spouse_LastName;
        //            Info.Spouse_FirstName = model.Spouse_FirstName;
        //            Info.Spouse_MiddleName = model.Spouse_MiddleName;
        //            Info.Spouse_NameExt = model.Spouse_NameExt;
        //            Info.Spouse_Occupation = model.Spouse_Occupation;
        //            Info.Spouse_Employer = model.Spouse_Employer;
        //            Info.Spouse_Employer_Address = model.Spouse_Employer_Address;
        //            Info.Spouse_Employer_CountryId = model.Spouse_Employer_CountryId;
        //            Info.Spouse_Employer_PostalCode = model.Spouse_Employer_PostalCode;
        //            Info.Spouse_Employer_TelephoneNo = model.Spouse_Employer_TelephoneNo;
        //            Info.Father_LastName = model.Father_LastName;
        //            Info.Father_FirstName = model.Father_FirstName;
        //            Info.Father_MiddleName = model.Father_MiddleName;
        //            Info.Father_NameExt = model.Father_NameExt;
        //            Info.Mother_LastName = model.Mother_LastName;
        //            Info.Mother_FirstName = model.Mother_FirstName;
        //            Info.Mother_MiddleName = model.Mother_MiddleName;

        //            Info.PDS_GovIssuedId = model.PDS_GovIssuedId;
        //            Info.PDS_IDLIcensePPNo = model.PDS_IDLIcensePPNo;
        //            Info.PDS_DateOfIssuance = model.PDS_DateOfIssuance;
        //            Info.PDS_PlaceOfIssuance = model.PDS_PlaceOfIssuance;

        //            context.SubmitChanges();
        //        }

        //        Cache.Update_Tables(_stakeholder_infos: true);
        //    }
        //}

        public static void upload_StakeholderPhotos(tblStakeholder stakeholder, HttpPostedFileBase ProfilePhoto, string ProfilePhoto_Remove, ref List<string> msg, ref List<string> err)
        {
            bool removeProfilePhoto = ProfilePhoto_Remove == "True";
            
            if (removeProfilePhoto)
            {
                using (dalDataContext context = new dalDataContext())
                {
                    tblUserPhoto user = context.tblUserPhotos.Where(x => x.StakeholderId == stakeholder.StakeholderId).SingleOrDefault();
                    if (user == null)
                    {
                        err.Add("No Profile photo to remove");
                    }
                    else
                    {
                        context.tblUserPhotos.DeleteOnSubmit(user);
                        context.SubmitChanges();

                        msg.Add("Profile photo was successfully removed");
                    }
                }
            }
            else if (ProfilePhoto != null)
            {
                bool proceed = true;

                if (!FixedSettings.PhotoFileTypes.Split(',').Contains(ProfilePhoto.ContentType))
                {
                    err.Add("Invalid file type");
                    proceed = false;
                }

                if (FixedSettings.PhotoFileSize < ProfilePhoto.ContentLength)
                {
                    err.Add("File size exceeded file-size limit (" + FixedSettings.PhotoFileSize.ToBytes() + ")");
                    proceed = false;
                }

                if (proceed)
                {

                    try
                    {
                        using (dalDataContext context = new dalDataContext())
                        {
                            tblUserPhoto user = context.tblUserPhotos.Where(x => x.StakeholderId == stakeholder.StakeholderId).SingleOrDefault();
                            if (user == null)
                            {
                                if (stakeholder.UserId == null)
                                {
                                    throw new Exception("Cannot upload profile photo. Stakeholder has no login account");
                                }
                                else
                                {
                                    user = new tblUserPhoto { StakeholderId = stakeholder.StakeholderId };
                                    context.tblUserPhotos.InsertOnSubmit(user);
                                }
                            }

                            byte[] imgByte = new Byte[ProfilePhoto.ContentLength];
                            ProfilePhoto.InputStream.Read(imgByte, 0, ProfilePhoto.ContentLength);

                            user.Photo = imgByte;

                            context.SubmitChanges();

                            msg.Add("Profile photo was successfully uploaded");
                        }
                    }
                    catch (Exception ex)
                    {
                        err.Add(coreProcs.ShowErrors(ex));
                    }
                }
            }

        }
    }
}