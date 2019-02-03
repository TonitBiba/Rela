using Microsoft.AspNet.Identity.Owin;
using Rela_project.Models;
using Rela_project.Models.CheckSimilarity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Rela_project.Controllers
{
    [Authorize(Roles ="User")]
    [RoutePrefix("api/History")]
    public class HistoryController : ApiController
    {
        RelaEntities rela = new RelaEntities();
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set
            {
                _userManager = value;
            }
        }

        [HttpPost]
        [Route("GetImages")]
        public async Task<IHttpActionResult> GetImageHistory([FromBody]HistoryRequstModel historyRequstModel) {
            var user = await UserManager.FindByIdAsync(historyRequstModel.userId);
            var imagesProcesed = (from img in rela.ImagesProceseds
                                  where img.UserId == historyRequstModel.userId
                                        select new HistoryCheckSimilarity
                                  {
                                      image = img.image,
                                      dateImage = (DateTime)img.date
                                  }).ToList();
            return Ok(imagesProcesed);
        }

        [HttpPost]
        [Route("GetVoiceImage")]
        public async Task<IHttpActionResult> getImagesWithVoice([FromBody]HistoryRequstModel historyRequstModel)
        {
            var user = await UserManager.FindByIdAsync(historyRequstModel.userId);
            var imagesVoice = (from img in rela.ImagesProceseds
                                     join voice in rela.Voices on img.imageId equals voice.imageId
                                     where img.UserId == historyRequstModel.userId
                                     select new VoiceHistory
                                     {
                                         imageId = img.imageId,
                                         date = img.date,
                                         image = img.image,
                                         GoogleVoice = voice.GoogleVoice,
                                         voiceId = voice.voiceId
                                     }).ToList();
            return Ok(imagesVoice);
        }

    }
}
