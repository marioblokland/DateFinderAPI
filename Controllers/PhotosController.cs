using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.Data;
using DatingApp.API.Dto;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IDatingRepository _repository;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;

        public PhotosController(IDatingRepository repository, IMapper mapper,
            IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _repository = repository;
            _mapper = mapper;
            _cloudinaryConfig = cloudinaryConfig;

            Account account = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(account);
        }

        /**
         * Adds a new user photo
         */
        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, PhotoForCreationDto photoForCreationDto)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var user = await _repository.GetUser(userId);
            var file = photoForCreationDto.File;
            var uploadResult = await UploadImage(file);

            photoForCreationDto.Url = uploadResult.Uri.ToString();
            photoForCreationDto.PublicId = uploadResult.PublicId;

            var photo = _mapper.Map<Photo>(photoForCreationDto);

            user.Photos.Add(photo);
            if (!HasMainPhoto(user))
            {
                photo.IsMain = true;
            }

            if (await _repository.SaveAll())
            {
                return Ok();
            }

            return BadRequest("Could not add the photo");
        }

        /**
         * Uploads the provided image to Cloudinary
         */
        private async Task<ImageUploadResult> UploadImage(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();

            if (file.Length == 0)
            {
                return uploadResult;
            }

            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.Name, stream),
                    Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                };

                uploadResult = _cloudinary.Upload(uploadParams);
            }

            return uploadResult;
        }

        /**
         * Returns true, if the provided user has a main photo.
         */
        private bool HasMainPhoto(User user)
        {
            return user.Photos.Any(photo => photo.IsMain);
        }
    }
}
