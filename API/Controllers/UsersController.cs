using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Services.Inerfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseController
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public UsersController( IUnitOfWork unitOfWork,IMapper mapper, IPhotoService photoService)
        {

       
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _photoService = photoService;
        }



        [HttpGet]
        public async Task<ActionResult<PagedList<MemberDTO>>> GetUsers([FromQuery] UserParams userParams)
        {
            var gender = await _unitOfWork.UserRepository.GetUserGender(User.GetUsername());
            userParams.CurrentUserName = User.GetUsername();

            if (string.IsNullOrEmpty(gender))
            {
                userParams.Gender = gender == "male" ? "female" : "male";
            }

            var users = await _unitOfWork.UserRepository.GetMemberAsync(userParams);
            Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount,
             users.TotalPages));


            return new JsonResult(users);
        }


        [HttpGet("{userName}")]
        public async Task<ActionResult<MemberDTO>> GetUsers(string userName)
        {

            return await _unitOfWork.UserRepository.GetMemberAsync(userName);

        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            //var userName=User.FindFirst(ClaimTypes.NameIdentifier)?.Value;         
            var user = await _unitOfWork.UserRepository.GetUserBynameAsync(User.GetUsername());
            if (user == null) return NotFound();

            _mapper.Map(memberUpdateDto, user);

            if (await _unitOfWork.Complete()) return NoContent();

            return BadRequest("Failed to update user ");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDTO>> AddPhoto(IFormFile file)
        {
            var user = await _unitOfWork.UserRepository.GetUserBynameAsync(User.GetUsername());

            if (file == null) return NotFound();

            var result = await _photoService.AddPhotoAsync(file);
            if (result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };
            if (user.Photos.Count == 0) photo.IsMain = true;

            user.Photos.Add(photo);
            if (await _unitOfWork.Complete())
            {

                return CreatedAtAction(nameof(GetUsers),
                new { username = user.UserName }, _mapper.Map<PhotoDTO>(photo));
            }

            return BadRequest("Problem adding Photo!");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await _unitOfWork.UserRepository.GetUserBynameAsync(User.GetUsername());
            if (user == null) return NotFound();

            var photo = user.Photos.FirstOrDefault(photo => photo.Id == photoId);
            if (photo == null) return NotFound();

            if (photo.IsMain) return BadRequest("this is already your main  photo");


            var currentMain = user.Photos.FirstOrDefault(photo => photo.IsMain);
            if (currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;
            if (await _unitOfWork.Complete()) return NoContent();

            return BadRequest("Problem setting Main");

        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _unitOfWork.UserRepository.GetUserBynameAsync(User.GetUsername());
            var photo = user.Photos.FirstOrDefault(photo => photo.Id == photoId);

            if (photo == null) return NotFound();

            if (photo.IsMain) return BadRequest("you cannot delete your main photo");
            if (photo.PublicId != null)
            {
                var results = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (results.Error != null) return BadRequest(results.Error.Message);
            }

            user.Photos.Remove(photo);
            if (await _unitOfWork.Complete()) return Ok();

            return BadRequest("Problem deleting photos!");

        }
    }




}