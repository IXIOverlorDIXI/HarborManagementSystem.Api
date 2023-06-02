using System.Linq;
using Application.DTOs;
using Domain.Consts;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using NetTopologySuite.Geometries;

namespace Application.Core
{
    public class MappingProfiles : AutoMapper.Profile
    {
        public MappingProfiles()
        {
            CreateMap<Domain.Entities.File, FileDto>()
                .ForMember(dto => dto.Url, option =>
                    option.MapFrom(entity => entity.Url))
                .ReverseMap();

            CreateMap<AppUser, SettingsDto>()
                .ForMember(dto => dto.Settings, option =>
                    option.MapFrom(entity => entity.Settings))
                .ReverseMap();

            CreateMap<AppUser, ProfileDto>()
                .ForMember(dto => dto.Username, option =>
                    option.MapFrom(entity => entity.UserName))
                .ForMember(dto => dto.DisplayName, option =>
                    option.MapFrom(entity => entity.DisplayName))
                .ForMember(dto => dto.Email, option =>
                    option.MapFrom(entity => entity.Email))
                .ForMember(dto => dto.PhoneNumber, option =>
                    option.MapFrom(entity => entity.PhoneNumber))
                .ReverseMap();

            CreateMap<Subscription, SubscriptionDto>()
                .ForMember(dto => dto.DisplayName, option =>
                    option.MapFrom(entity => entity.DisplayName))
                .ForMember(dto => dto.Description, option =>
                    option.MapFrom(entity => entity.Description))
                .ForMember(dto => dto.TaxOnBooking, option =>
                    option.MapFrom(entity => entity.TaxOnBooking))
                .ForMember(dto => dto.TaxOnServices, option =>
                    option.MapFrom(entity => entity.TaxOnServices))
                .ForMember(dto => dto.Price, option =>
                    option.MapFrom(entity => entity.Price))
                .ForMember(dto => dto.MaxHarborAmount, option =>
                    option.MapFrom(entity => entity.MaxHarborAmount))
                .ForMember(dto => dto.Id, option =>
                    option.MapFrom(entity => entity.Id))
                .ForMember(dto => dto.SubscriberAmount, option =>
                    option.MapFrom(entity => entity.Users.Count));

            CreateMap<SubscriptionDto, Subscription>()
                .ForMember(entity => entity.DisplayName, option =>
                    option.MapFrom(dto => dto.DisplayName))
                .ForMember(entity => entity.Description, option =>
                    option.MapFrom(dto => dto.Description))
                .ForMember(entity => entity.TaxOnBooking, option =>
                    option.MapFrom(dto => dto.TaxOnBooking))
                .ForMember(entity => entity.TaxOnServices, option =>
                    option.MapFrom(dto => dto.TaxOnServices))
                .ForMember(entity => entity.Price, option =>
                    option.MapFrom(dto => dto.Price))
                .ForMember(entity => entity.MaxHarborAmount, option =>
                    option.MapFrom(dto => dto.MaxHarborAmount))
                .ForMember(entity => entity.Id, option =>
                    option.MapFrom(dto => dto.Id));

            CreateMap<SubscriptionСheck, SubscriptionCheckDto>()
                .ForMember(dto => dto.Description, option =>
                    option.MapFrom(entity => entity.Description))
                .ForMember(dto => dto.BankTransactionId, option =>
                    option.MapFrom(entity => entity.BankTransactionId))
                .ForMember(dto => dto.Date, option =>
                    option.MapFrom(entity => entity.Date))
                .ForMember(dto => dto.TotalCost, option =>
                    option.MapFrom(entity => entity.TotalCost))
                .ForMember(dto => dto.SubscriptionId, option =>
                    option.MapFrom(entity => entity.SubscriptionId))
                .ReverseMap();

            CreateMap<IFormFile, ProfilePhotoDataDto>()
                .ForMember(dto => dto.FileStream,
                    option => option.MapFrom(entity => entity.OpenReadStream()))
                .ForMember(dto => dto.FileNameWithExtension,
                    option => option.MapFrom(entity => entity.FileName));

            CreateMap<ShipType, ShipTypeDto>()
                .ForMember(dto => dto.Id, option =>
                    option.MapFrom(entity => entity.Id))
                .ForMember(dto => dto.TypeName, option =>
                    option.MapFrom(entity => entity.TypeName))
                .ForMember(dto => dto.Description, option =>
                    option.MapFrom(entity => entity.Description))
                .ReverseMap();

            CreateMap<Ship, ShipPreviewDataDto>()
                .ForMember(dto => dto.Id, option =>
                    option.MapFrom(entity => entity.Id))
                .ForMember(dto => dto.DisplayName, option =>
                    option.MapFrom(entity => entity.DisplayName))
                .ForMember(dto => dto.PhotoUrl, option =>
                    option.MapFrom(entity => entity.Photo == null 
                        ? DefaultFileLinks.DefaultImage 
                        : entity.Photo.Url ))
                .ForMember(dto => dto.ShipTypeName, option =>
                    option.MapFrom(entity => entity.ShipType.TypeName))
                .ForMember(dto => dto.ShipTypeDescription, option =>
                    option.MapFrom(entity => entity.ShipType.Description));

            CreateMap<Ship, ShipDataDto>()
                .ForMember(dto => dto.Id, option =>
                    option.MapFrom(entity => entity.Id))
                .ForMember(dto => dto.DisplayName, option =>
                    option.MapFrom(entity => entity.DisplayName))
                .ForMember(dto => dto.ShipTypeId, option =>
                    option.MapFrom(entity => entity.ShipTypeId))
                .ReverseMap();

            CreateMap<IFormFile, ShipPhotoDataDto>()
                .ForMember(dto => dto.FileStream,
                    option => option.MapFrom(entity => entity.OpenReadStream()))
                .ForMember(dto => dto.FileNameWithExtension,
                    option => option.MapFrom(entity => entity.FileName));

            CreateMap<IFormFile, HarborDocumentDataDto>()
                .ForMember(dto => dto.FileStream,
                    option => option.MapFrom(entity => entity.OpenReadStream()))
                .ForMember(dto => dto.FileNameWithExtension,
                    option => option.MapFrom(entity => entity.FileName));

            CreateMap<IFormFile, HarborPhotoDataDto>()
                .ForMember(dto => dto.FileStream,
                    option => option.MapFrom(entity => entity.OpenReadStream()))
                .ForMember(dto => dto.FileNameWithExtension,
                    option => option.MapFrom(entity => entity.FileName));

            CreateMap<Harbor, HarborDataDto>()
                .ForMember(dto => dto.Id,
                    option => option.MapFrom(entity => entity.Id))
                .ForMember(dto => dto.DisplayName,
                    option => option.MapFrom(entity => entity.DisplayName))
                .ForMember(dto => dto.SupportEmail,
                    option => option.MapFrom(entity => entity.SupportEmail))
                .ForMember(dto => dto.Description,
                    option => option.MapFrom(entity => entity.Description))
                .ForMember(dto => dto.SupportPhoneNumber,
                    option => option.MapFrom(entity => entity.SupportPhoneNumber))
                .ForMember(dto => dto.BIC,
                    option => option.MapFrom(entity => entity.BIC))
                .ForMember(dto => dto.IBAN,
                    option => option.MapFrom(entity => entity.IBAN))
                .ForMember(dto => dto.GeolocationLongitude,
                    option => option.MapFrom(entity => entity.Geolocation.X))
                .ForMember(dto => dto.GeolocationLatitude,
                    option => option.MapFrom(entity => entity.Geolocation.Y));

            CreateMap<HarborDataDto, Harbor>()
                .ForMember(entity => entity.Id,
                    option => option.MapFrom(dto => dto.Id))
                .ForMember(entity => entity.DisplayName,
                    option => option.MapFrom(dto => dto.DisplayName))
                .ForMember(entity => entity.SupportEmail,
                    option => option.MapFrom(dto => dto.SupportEmail))
                .ForMember(entity => entity.Description,
                    option => option.MapFrom(dto => dto.Description))
                .ForMember(entity => entity.SupportPhoneNumber,
                    option => option.MapFrom(dto => dto.SupportPhoneNumber))
                .ForMember(entity => entity.BIC,
                    option => option.MapFrom(dto => dto.BIC))
                .ForMember(entity => entity.IBAN,
                    option => option.MapFrom(dto => dto.IBAN))
                .ForMember(entity => entity.Geolocation,
                    option => option
                        .MapFrom(dto => new Point(dto.GeolocationLongitude, dto.GeolocationLatitude)));

            CreateMap<Harbor, HarborPreviewDataDto>()
                .ForMember(dto => dto.Id, option =>
                    option.MapFrom(entity => entity.Id))
                .ForMember(dto => dto.OwnerUserName, option =>
                    option.MapFrom(entity => entity.Owner.UserName))
                .ForMember(dto => dto.DisplayName, option =>
                    option.MapFrom(entity => entity.DisplayName))
                .ForMember(dto => dto.Description, option =>
                    option.MapFrom(entity => entity.Description))
                .ForMember(dto => dto.SupportEmail, option =>
                    option.MapFrom(entity => entity.SupportEmail))
                .ForMember(dto => dto.SupportPhoneNumber, option =>
                    option.MapFrom(entity => entity.SupportPhoneNumber))
                .ForMember(dto => dto.GeolocationLongitude,
                    option => option.MapFrom(entity => entity.Geolocation.X))
                .ForMember(dto => dto.GeolocationLatitude,
                    option => option.MapFrom(entity => entity.Geolocation.Y))
                .ForMember(dto => dto.Photos, option =>
                    option.MapFrom(entity => entity.HarborPhotos.Select(x => x.Photo.Url).ToList()))
                .ForMember(dto => dto.IBAN, option =>
                    option.MapFrom(entity => entity.IBAN))
                .ForMember(dto => dto.BIC, option =>
                    option.MapFrom(entity => entity.BIC));

            CreateMap<HarborPhoto, HarborPhotoDto>()
                .ForMember(dto => dto.PhotoId, option =>
                    option.MapFrom(entity => entity.Id))
                .ForMember(dto => dto.Url, option =>
                    option.MapFrom(entity => entity.Photo.Url));

            CreateMap<HarborDocument, HarborDocumentDto>()
                .ForMember(dto => dto.DocumentId, option =>
                    option.MapFrom(entity => entity.Id))
                .ForMember(dto => dto.Url, option =>
                    option.MapFrom(entity => entity.Document.Url));

            CreateMap<Domain.Entities.Service, ServiceDto>()
                .ForMember(dto => dto.Id, option =>
                    option.MapFrom(entity => entity.Id))
                .ForMember(dto => dto.HarborId, option =>
                    option.MapFrom(entity => entity.HarborId))
                .ForMember(dto => dto.Description, option =>
                    option.MapFrom(entity => entity.Description))
                .ForMember(dto => dto.DisplayName, option =>
                    option.MapFrom(entity => entity.DisplayName))
                .ForMember(dto => dto.Price, option =>
                    option.MapFrom(entity => entity.Price))
                .ReverseMap();
            
            CreateMap<Domain.Entities.Service, ServicePreviewDto>()
                .ForMember(dto => dto.Id, option =>
                    option.MapFrom(entity => entity.Id))
                .ForMember(dto => dto.Description, option =>
                    option.MapFrom(entity => entity.Description))
                .ForMember(dto => dto.DisplayName, option =>
                    option.MapFrom(entity => entity.DisplayName))
                .ForMember(dto => dto.Price, option =>
                    option.MapFrom(entity => entity.Price));
            
            CreateMap<Domain.Entities.AdditionalService, ServicePreviewDto>()
                .ForMember(dto => dto.Id, option =>
                    option.MapFrom(entity => entity.ServiceId))
                .ForMember(dto => dto.Description, option =>
                    option.MapFrom(entity => entity.Service.Description))
                .ForMember(dto => dto.DisplayName, option =>
                    option.MapFrom(entity => entity.Service.DisplayName))
                .ForMember(dto => dto.Price, option =>
                    option.MapFrom(entity => entity.Service.Price));

            CreateMap<Domain.Entities.Berth, BerthPreviewDataDto>()
                .ForMember(dto => dto.Id, option =>
                    option.MapFrom(entity => entity.Id))
                .ForMember(dto => dto.DisplayName, option =>
                    option.MapFrom(entity => entity.DisplayName))
                .ForMember(dto => dto.Description, option =>
                    option.MapFrom(entity => entity.Description))
                .ForMember(dto => dto.Price, option =>
                    option.MapFrom(entity => entity.Price))
                .ForMember(dto => dto.Photos, option =>
                    option.MapFrom(entity => entity.BerthPhotos.Select(x => x.Photo.Url).ToList()))
                .ForMember(dto => dto.GeolocationLongitude,
                    option => option.MapFrom(
                        entity => entity.Harbor.Geolocation.X))
                .ForMember(dto => dto.GeolocationLatitude,
                    option => option.MapFrom(
                        entity => entity.Harbor.Geolocation.Y))
                .ForMember(dto => dto.IsActive, option =>
                    option.MapFrom(entity => entity.IsActive))
                .ForMember(dto => dto.SuitableShipTypes, option =>
                    option.MapFrom(entity => entity.SuitableShipTypes
                        .Select(x => new ShipTypeDto
                        {
                            Description = x.ShipType.Description,
                            Id = x.ShipTypeId,
                            TypeName = x.ShipType.TypeName
                        })));

            CreateMap<Domain.Entities.Berth, BerthDataDto>()
                .ForMember(dto => dto.Id, option =>
                    option.MapFrom(entity => entity.Id))
                .ForMember(dto => dto.HarborId, option =>
                    option.MapFrom(entity => entity.HarborId))
                .ForMember(dto => dto.Description, option =>
                    option.MapFrom(entity => entity.Description))
                .ForMember(dto => dto.DisplayName, option =>
                    option.MapFrom(entity => entity.DisplayName))
                .ForMember(dto => dto.Price, option =>
                    option.MapFrom(entity => entity.Price))
                .ForMember(dto => dto.SuitableShipTypes, option => option.Ignore());

            CreateMap<BerthDataDto, Domain.Entities.Berth>()
                .ForMember(entity => entity.Id, option =>
                    option.MapFrom(dto => dto.Id))
                .ForMember(entity => entity.HarborId, option =>
                    option.MapFrom(dto => dto.HarborId))
                .ForMember(entity => entity.Description, option =>
                    option.MapFrom(dto => dto.Description))
                .ForMember(entity => entity.DisplayName, option =>
                    option.MapFrom(dto => dto.DisplayName))
                .ForMember(entity => entity.Price, option =>
                    option.MapFrom(dto => dto.Price))
                .ForMember(entity => entity.SuitableShipTypes, option => option.Ignore());
            
            CreateMap<BerthPhoto, BerthPhotoDto>()
                .ForMember(dto => dto.PhotoId, option =>
                    option.MapFrom(entity => entity.Id))
                .ForMember(dto => dto.Url, option =>
                    option.MapFrom(entity => entity.Photo.Url));
            
            CreateMap<IFormFile, BerthPhotoDataDto>()
                .ForMember(dto => dto.FileStream,
                    option => option.MapFrom(entity => entity.OpenReadStream()))
                .ForMember(dto => dto.FileNameWithExtension,
                    option => option.MapFrom(entity => entity.FileName));
            
            CreateMap<Domain.Entities.RelativePositionMetering, RelativePositionMeteringDto>()
                .ForMember(dto => dto.LeftDistance, option =>
                    option.MapFrom(entity => entity.LeftDistance))
                .ForMember(dto => dto.BackDistance, option =>
                    option.MapFrom(entity => entity.BackDistance))
                .ForMember(dto => dto.RightDistance, option =>
                    option.MapFrom(entity => entity.RightDistance))
                .ForMember(dto => dto.FrontDistance, option =>
                    option.MapFrom(entity => entity.FrontDistance))
                .ForMember(dto => dto.RotationAngle, option => 
                    option.MapFrom(entity => entity.RotationAngle))
                .ForMember(dto => dto.HeightHeadAboveWater, option => 
                    option.MapFrom(entity => entity.HeightHeadAboveWater))
                .ForMember(dto => dto.HeightTailAboveWater, option => 
                    option.MapFrom(entity => entity.HeightTailAboveWater))
                .ForMember(dto => dto.TiltAngle, option => 
                    option.MapFrom(entity => entity.TiltAngle))
                .ForMember(dto => dto.RollAngle, option => 
                    option.MapFrom(entity => entity.RollAngle))
                .ForMember(dto => dto.BerthId, option => 
                    option.MapFrom(entity => entity.BerthId))
                .ForMember(dto => dto.MeteringDate, option =>
                    option.MapFrom(entity => entity.MeteringDate))
                .ReverseMap();
            
            CreateMap<Domain.Entities.EnvironmentalCondition, EnvironmentalConditionDto>()
                .ForMember(dto => dto.BerthId, option =>
                    option.MapFrom(entity => entity.BerthId))
                .ForMember(dto => dto.Temperature, option =>
                    option.MapFrom(entity => entity.Temperature))
                .ForMember(dto => dto.AtmospherePressure, option =>
                    option.MapFrom(entity => entity.AtmospherePressure))
                .ForMember(dto => dto.WindSpeed, option =>
                    option.MapFrom(entity => entity.WindSpeed))
                .ForMember(dto => dto.ShipRelativeWindDirection, option => 
                    option.MapFrom(entity => entity.ShipRelativeWindDirection))
                .ForMember(dto => dto.WaveSpeed, option => 
                    option.MapFrom(entity => entity.WaveSpeed))
                .ForMember(dto => dto.WaveForce, option => 
                    option.MapFrom(entity => entity.WaveForce))
                .ForMember(dto => dto.MeteringDate, option =>
                    option.MapFrom(entity => entity.MeteringDate))
                .ReverseMap();
            
            CreateMap<Domain.Entities.StorageEnvironmentalCondition, StorageEnvironmentalConditionDto>()
                .ForMember(dto => dto.BerthId, option =>
                    option.MapFrom(entity => entity.BerthId))
                .ForMember(dto => dto.AirPollution, option =>
                    option.MapFrom(entity => entity.AirPollution))
                .ForMember(dto => dto.WaterPollution, option =>
                    option.MapFrom(entity => entity.WaterPollution))
                .ForMember(dto => dto.RadiationLevel, option =>
                    option.MapFrom(entity => entity.RadiationLevel))
                .ForMember(dto => dto.ShipTemperature, option =>
                    option.MapFrom(entity => entity.ShipTemperature))
                .ForMember(dto => dto.MeteringDate, option =>
                    option.MapFrom(entity => entity.MeteringDate))
                .ReverseMap();
            
            CreateMap<Domain.Entities.Review, ReviewPreviewDataDto>()
                .ForMember(dto => dto.Id, option =>
                    option.MapFrom(entity => entity.Id))
                .ForMember(dto => dto.BerthId, option =>
                    option.MapFrom(entity => entity.BerthId))
                .ForMember(dto => dto.ReviewMark, option =>
                    option.MapFrom(entity => entity.ReviewMark))
                .ForMember(dto => dto.ReviewBody, option =>
                    option.MapFrom(entity => entity.ReviewBody))
                .ForMember(dto => dto.ReviewMinuses, option =>
                    option.MapFrom(entity => entity.ReviewMinuses))
                .ForMember(dto => dto.ReviewPluses, option =>
                    option.MapFrom(entity => entity.ReviewPluses))
                .ForMember(dto => dto.Date, option =>
                    option.MapFrom(entity => entity.Date))
                .ForMember(dto => dto.AuthorDisplayName, option =>
                    option.MapFrom(entity => entity.Reviewer.DisplayName))
                .ForMember(dto => dto.AuthorPhotoUrl, option =>
                    option.MapFrom(entity => entity.Reviewer.Photo == null 
                        ? DefaultFileLinks.DefaultUserPhoto
                        : entity.Reviewer.Photo.Url))
                .ForMember(dto => dto.DoesAuthorPay, option =>
                    option.MapFrom(entity => entity.Reviewer.Ships
                        .Any(x => x.Bookings.Any(y => y.BerthId.Equals(entity.BerthId)))));

            CreateMap<Domain.Entities.Review, ReviewDataDto>()
                .ForMember(dto => dto.Id, option =>
                    option.MapFrom(entity => entity.Id))
                .ForMember(dto => dto.BerthId, option =>
                    option.MapFrom(entity => entity.BerthId))
                .ForMember(dto => dto.ReviewMark, option =>
                    option.MapFrom(entity => entity.ReviewMark))
                .ForMember(dto => dto.ReviewBody, option =>
                    option.MapFrom(entity => entity.ReviewBody))
                .ForMember(dto => dto.ReviewMinuses, option =>
                    option.MapFrom(entity => entity.ReviewMinuses))
                .ForMember(dto => dto.ReviewPluses, option =>
                    option.MapFrom(entity => entity.ReviewPluses))
                .ForMember(dto => dto.Date, option =>
                    option.MapFrom(entity => entity.Date))
                .ReverseMap();
            
            CreateMap<Domain.Entities.BookingCheck, BookingCheckDataDto>()
                .ForMember(dto => dto.BookingId, option =>
                    option.MapFrom(entity => entity.BookingId))
                .ForMember(dto => dto.BankTransactionId, option =>
                    option.MapFrom(entity => entity.BankTransactionId))
                .ForMember(dto => dto.TotalCost, option =>
                    option.MapFrom(entity => entity.TotalCost))
                .ForMember(dto => dto.Description, option =>
                    option.MapFrom(entity => entity.Description))
                .ForMember(dto => dto.Date, option =>
                    option.MapFrom(entity => entity.Date))
                .ReverseMap();

            CreateMap<Domain.Entities.Booking, BookingDataDto>()
                .ForMember(dto => dto.Id, option =>
                    option.MapFrom(entity => entity.Id))
                .ForMember(dto => dto.StartDate, option =>
                    option.MapFrom(entity => entity.StartDate))
                .ForMember(dto => dto.EndDate, option =>
                    option.MapFrom(entity => entity.EndDate))
                .ForMember(dto => dto.ShipId, option =>
                    option.MapFrom(entity => entity.ShipId))
                .ForMember(dto => dto.BerthId, option =>
                    option.MapFrom(entity => entity.BerthId))
                .ForMember(dto => dto.AdditionalServices, option => option.Ignore());
            
            CreateMap<Domain.Entities.Booking, BookingEditDataDto>()
                .ForMember(dto => dto.Id, option =>
                    option.MapFrom(entity => entity.Id))
                .ForMember(dto => dto.StartDate, option =>
                    option.MapFrom(entity => entity.StartDate))
                .ForMember(dto => dto.EndDate, option =>
                    option.MapFrom(entity => entity.EndDate))
                .ForMember(dto => dto.ShipId, option =>
                    option.MapFrom(entity => entity.ShipId))
                .ForMember(dto => dto.BerthId, option =>
                    option.MapFrom(entity => entity.BerthId))
                .ForMember(dto => dto.HarborId, option =>
                    option.MapFrom(entity => entity.Berth.HarborId))
                .ForMember(dto => dto.AdditionalServices, option => option.Ignore());

            
            CreateMap<BookingDataDto, Booking>()
                .ForMember(entity => entity.Id, option =>
                    option.MapFrom(dto => dto.Id))
                .ForMember(entity => entity.StartDate, option =>
                    option.MapFrom(dto => dto.StartDate))
                .ForMember(entity => entity.EndDate, option =>
                    option.MapFrom(dto => dto.EndDate))
                .ForMember(entity => entity.ShipId, option =>
                    option.MapFrom(dto => dto.ShipId))
                .ForMember(entity => entity.BerthId, option =>
                    option.MapFrom(dto => dto.BerthId))
                .ForMember(entity => entity.AdditionalServices, option => option.Ignore());

            CreateMap<AdditionalService, ServiceDto>()
                .ForMember(dto => dto.Id, option =>
                    option.MapFrom(entity => entity.Service.Id))
                .ForMember(dto => dto.Price, option =>
                    option.MapFrom(entity => entity.Service.Price))
                .ForMember(dto => dto.Description, option =>
                    option.MapFrom(entity => entity.Service.Description))
                .ForMember(dto => dto.DisplayName, option =>
                    option.MapFrom(entity => entity.Service.DisplayName))
                .ForMember(dto => dto.HarborId, option =>
                    option.MapFrom(entity => entity.Service.HarborId));
            
            CreateMap<Domain.Entities.Booking, BookingPreviewDataDto>()
                .ForMember(dto => dto.Id, option =>
                    option.MapFrom(entity => entity.Id))
                .ForMember(dto => dto.StartDate, option =>
                    option.MapFrom(entity => entity.StartDate))
                .ForMember(dto => dto.EndDate, option =>
                    option.MapFrom(entity => entity.EndDate))
                .ForMember(dto => dto.ShipName, option =>
                    option.MapFrom(entity => entity.Ship.DisplayName))
                .ForMember(dto => dto.BerthName, option =>
                    option.MapFrom(entity => entity.Berth.DisplayName))
                .ForMember(dto => dto.HarborName, option =>
                    option.MapFrom(entity => entity.Berth.Harbor.DisplayName))
                .ForMember(dto => dto.IsPayed, option =>
                    option.MapFrom(entity => entity.BookingCheck != null))
                .ForMember(dto => dto.AdditionalServices, option =>
                    option.MapFrom(entity => entity.AdditionalServices));

            CreateMap<Domain.Entities.Booking, BookingDataForCheckDto>()
                .ForMember(dto => dto.StartDate, option =>
                    option.MapFrom(entity => entity.StartDate))
                .ForMember(dto => dto.EndDate, option =>
                    option.MapFrom(entity => entity.EndDate))
                .ForMember(dto => dto.BerthName, option =>
                    option.MapFrom(entity => entity.Berth.DisplayName))
                .ForMember(dto => dto.HarborName, option =>
                    option.MapFrom(entity => entity.Berth.Harbor.DisplayName))
                .ForMember(dto => dto.IBAN, option =>
                    option.MapFrom(entity => entity.Berth.Harbor.IBAN))
                .ForMember(dto => dto.BIC, option =>
                    option.MapFrom(entity => entity.Berth.Harbor.BIC))
                .ForMember(dto => dto.Services, option =>
                    option.MapFrom(entity => entity.AdditionalServices.Select(x => new AdditionalServiceDto()
                    {
                        Description = x.Service.Description,
                        DisplayName = x.Service.DisplayName,
                        Price = x.Service.Price
                    }).ToList()))
                .ForMember(dto => dto.TotalCost, option =>
                    option.MapFrom(entity => entity.AdditionalServices.Sum(x => x.Service.Price) + entity.Berth.Price));

        }
    }
}